using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Blob.Integration.Extensions;

public static class BlobExtensions
{
    /// <summary>
    /// Gets the URI for a blob
    /// </summary>
    public static Uri GetBlobUri(this BlobClient blobClient)
    {
        return blobClient.Uri;
    }

    /// <summary>
    /// Checks if a blob exists
    /// </summary>
    public static async Task<bool> ExistsAsync(this BlobClient blobClient, CancellationToken cancellationToken = default)
    {
        try
        {
            Azure.Response<bool> response = await blobClient.ExistsAsync(cancellationToken);
            return response.Value;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets blob size in bytes
    /// </summary>
    public static async Task<long> GetSizeAsync(this BlobClient blobClient, CancellationToken cancellationToken = default)
    {
        Azure.Response<BlobProperties> properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        return properties.Value.ContentLength;
    }

    /// <summary>
    /// Gets blob content type
    /// </summary>
    public static async Task<string> GetContentTypeAsync(this BlobClient blobClient, CancellationToken cancellationToken = default)
    {
        Azure.Response<BlobProperties> properties = await blobClient.GetPropertiesAsync(cancellationToken: cancellationToken);
        return properties.Value.ContentType ?? "application/octet-stream";
    }

    /// <summary>
    /// Sets blob metadata
    /// </summary>
    public static async Task SetMetadataAsync(this BlobClient blobClient, Dictionary<string, string> metadata, CancellationToken cancellationToken = default)
    {
        await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Copies a blob to another location
    /// </summary>
    public static async Task<string> CopyToAsync(this BlobClient sourceBlobClient, BlobContainerClient destinationContainer, string? destinationBlobName = null, CancellationToken cancellationToken = default)
    {
        destinationBlobName ??= sourceBlobClient.Name;
        BlobClient destinationBlobClient = destinationContainer.GetBlobClient(destinationBlobName);

        CopyFromUriOperation copyOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri, cancellationToken: cancellationToken);
        await copyOperation.WaitForCompletionAsync(cancellationToken);

        return destinationBlobName;
    }

    /// <summary>
    /// Gets blob tags
    /// </summary>
    public static async Task<Dictionary<string, string>> GetTagsAsync(this BlobClient blobClient, CancellationToken cancellationToken = default)
    {
        Azure.Response<GetBlobTagResult> response = await blobClient.GetTagsAsync(cancellationToken: cancellationToken);
        return response.Value.Tags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    /// <summary>
    /// Sets blob tags
    /// </summary>
    public static async Task SetTagsAsync(this BlobClient blobClient, Dictionary<string, string> tags, CancellationToken cancellationToken = default)
    {
        await blobClient.SetTagsAsync(tags, cancellationToken: cancellationToken);
    }
}
