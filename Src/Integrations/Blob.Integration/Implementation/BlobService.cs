using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
