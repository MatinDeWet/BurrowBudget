using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Blob.Integration.Contracts;
using Blob.Integration.Extensions;
using Blob.Integration.Options;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Blob.Integration.Implementation;

public sealed class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _options;

    public BlobService(IOptions<BlobStorageOptions> options, ILogger<BlobService> logger)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            throw new InvalidOperationException("Blob storage connection string is not configured.");
        }

        _blobServiceClient = new BlobServiceClient(_options.ConnectionString);
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ValidateUploadInput(fileStream, fileName, containerName);

        BlobContainerClient containerClient = await GetOrCreateContainerAsync(containerName, cancellationToken);

        string blobName = Guid.CreateVersion7().ToString();
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        fileStream.ResetPosition();

        var uploadOptions = new BlobUploadOptions
        {
            Metadata = metadata,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(fileName)
            }
        };

        await blobClient.UploadAsync(fileStream, uploadOptions, cancellationToken);

        return blobName;
    }

    public async Task<Stream> DownloadBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ValidateDownloadInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> existsResponse = await blobClient.ExistsAsync(cancellationToken);
        if (!existsResponse.Value)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'");
        }

        Response<BlobDownloadStreamingResult> response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);

        return response.Value.Content;
    }

    public async Task DeleteBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ValidateDeleteInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.DeleteIfExistsAsync(
            DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: cancellationToken);
    }

    public async Task<Dictionary<string, string>> GetBlobMetadataAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ValidateMetadataInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> existsResponse = await blobClient.ExistsAsync(cancellationToken);
        if (!existsResponse.Value)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'");
        }

        Response<BlobProperties> propertiesResponse = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);

        return propertiesResponse.Value.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public async Task AddBlobMetadataAsync(
        string blobName,
        string containerName,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default)
    {
        ValidateMetadataInput(blobName, containerName);

        if (metadata == null || metadata.Count == 0)
        {
            throw new ArgumentException("Metadata cannot be null or empty.", nameof(metadata));
        }

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> existsResponse = await blobClient.ExistsAsync(cancellationToken);
        if (!existsResponse.Value)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'");
        }

        Response<BlobProperties> propertiesResponse = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        var existingMetadata = propertiesResponse.Value.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (KeyValuePair<string, string> kvp in metadata)
        {
            existingMetadata[kvp.Key] = kvp.Value;
        }

        await blobClient.SetMetadataAsync(existingMetadata, cancellationToken: cancellationToken);
    }

    public async Task<string> CreateEmptyBlobAsync(
        string containerName,
        string? blobName = null,
        string contentType = "application/octet-stream",
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
        }

        BlobContainerClient containerClient = await GetOrCreateContainerAsync(containerName, cancellationToken);

        string finalBlobName = string.IsNullOrWhiteSpace(blobName) ? Guid.CreateVersion7().ToString() : blobName;
        BlobClient blobClient = containerClient.GetBlobClient(finalBlobName);

        using var emptyStream = new MemoryStream([]);

        var uploadOptions = new BlobUploadOptions
        {
            Metadata = metadata,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };

        await blobClient.UploadAsync(emptyStream, uploadOptions, cancellationToken);

        return finalBlobName;
    }

    public async Task<Uri> CreateBlobSasTokenAsync(
        string blobName,
        string containerName,
        TimeSpan expiration,
        BlobSasPermissions permissions = BlobSasPermissions.Read,
        CancellationToken cancellationToken = default)
    {
        ValidateMetadataInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> existsResponse = await blobClient.ExistsAsync(cancellationToken);

        if (!existsResponse.Value)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'");
        }

        if (!blobClient.CanGenerateSasUri)
        {
            throw new InvalidOperationException(
                "Cannot generate SAS token. Ensure the BlobServiceClient is created with a connection string that includes the account key.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = blobName,
            Resource = "b", // "b" for blob
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start time with 5 min clock skew allowance
            ExpiresOn = DateTimeOffset.UtcNow.Add(expiration)
        };

        sasBuilder.SetPermissions(permissions);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

        return sasUri;
    }

    public async Task<long> GetBlobSizeAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ValidateMetadataInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        Response<bool> existsResponse = await blobClient.ExistsAsync(cancellationToken);
        if (!existsResponse.Value)
        {
            throw new FileNotFoundException($"Blob '{blobName}' not found in container '{containerName}'");
        }

        return await blobClient.GetSizeAsync(cancellationToken);
    }

    public async Task<bool> BlobExistsAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        ValidateMetadataInput(blobName, containerName);

        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        BlobClient blobClient = containerClient.GetBlobClient(blobName);

        return await blobClient.ExistsAsync(cancellationToken);
    }

    #region Private Methods

    private async Task<BlobContainerClient> GetOrCreateContainerAsync(string containerName, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        if (_options.AutoCreateContainers)
        {
            PublicAccessType publicAccessType = _options.DefaultPublicAccessLevel.ToUpperInvariant() switch
            {
                "BLOB" => PublicAccessType.Blob,
                "CONTAINER" => PublicAccessType.BlobContainer,
                _ => PublicAccessType.None
            };

            await containerClient.CreateIfNotExistsAsync(publicAccessType, cancellationToken: cancellationToken);
        }

        return containerClient;
    }

    private static string GetContentType(string fileName)
    {
        var provider = new FileExtensionContentTypeProvider();

        if (provider.TryGetContentType(fileName, out string? contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }

    private void ValidateUploadInput(Stream fileStream, string fileName, string containerName)
    {
        fileStream.ValidateForUpload(nameof(fileStream));

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
        }

        if (!fileName.IsValidFileExtension(_options.AllowedFileExtensions))
        {
            throw new ArgumentException(
                $"File extension not allowed. Allowed extensions: {string.Join(", ", _options.AllowedFileExtensions)}",
                nameof(fileName));
        }

        if (fileStream.CanSeek && fileStream.Length.ExceedsMaxSize(_options.MaxFileSizeMB))
        {
            throw new ArgumentException(
                $"File size exceeds maximum allowed size of {_options.MaxFileSizeMB} MB.",
                nameof(fileStream));
        }
    }

    private static void ValidateDownloadInput(string blobName, string containerName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentException("Blob name cannot be null or empty.", nameof(blobName));
        }

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
        }
    }

    private static void ValidateDeleteInput(string blobName, string containerName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentException("Blob name cannot be null or empty.", nameof(blobName));
        }

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
        }
    }

    private static void ValidateMetadataInput(string blobName, string containerName)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            throw new ArgumentException("Blob name cannot be null or empty.", nameof(blobName));
        }

        if (string.IsNullOrWhiteSpace(containerName))
        {
            throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
        }
    }

    #endregion
}
