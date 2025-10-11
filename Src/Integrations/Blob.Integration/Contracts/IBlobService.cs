namespace Blob.Integration.Contracts;

public interface IBlobService
{
    /// <summary>
    /// Uploads a blob to the specified container
    /// </summary>
    Task<string> UploadBlobAsync(string containerName, string blobName, Stream data, string contentType, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob from the specified container
    /// </summary>
    Task<Stream> DownloadBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob from the specified container
    /// </summary>
    Task DeleteBlobAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds metadata to an existing blob
    /// </summary>
    Task AddMetadataAsync(string containerName, string blobName, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes metadata from an existing blob
    /// </summary>
    Task RemoveMetadataAsync(string containerName, string blobName, IEnumerable<string> metadataKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates metadata on an existing blob (replaces existing metadata with new values)
    /// </summary>
    Task UpdateMetadataAsync(string containerName, string blobName, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metadata from an existing blob
    /// </summary>
    Task<Dictionary<string, string>> GetMetadataAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an empty blob (0 bytes) in the specified container
    /// </summary>
    Task CreateEmptyBlobAsync(string containerName, string blobName, string contentType = "application/octet-stream", Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for uploading to a blob (PUT)
    /// </summary>
    Task<string> GetPresignedUploadUrlAsync(string containerName, string blobName, TimeSpan expiry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for downloading a blob (GET)
    /// </summary>
    Task<string> GetPresignedDownloadUrlAsync(string containerName, string blobName, TimeSpan expiry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a blob exists in the specified container
    /// </summary>
    Task<bool> BlobExistsAsync(string containerName, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the size of a blob in bytes
    /// </summary>
    Task<long> GetBlobSizeAsync(string containerName, string blobName, CancellationToken cancellationToken = default);
}
