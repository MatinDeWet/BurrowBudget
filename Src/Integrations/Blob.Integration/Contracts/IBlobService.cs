namespace Blob.Integration.Contracts;

public interface IBlobService
{
    /// <summary>
    /// Uploads a blob to the specified bucket
    /// </summary>
    Task<string> UploadBlobAsync(string bucketName, string objectName, Stream data, string contentType, Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a blob from the specified bucket
    /// </summary>
    Task<Stream> DownloadBlobAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a blob from the specified bucket
    /// </summary>
    Task DeleteBlobAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds metadata to an existing blob
    /// </summary>
    Task AddMetadataAsync(string bucketName, string objectName, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes metadata from an existing blob
    /// </summary>
    Task RemoveMetadataAsync(string bucketName, string objectName, IEnumerable<string> metadataKeys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates metadata on an existing blob (replaces existing metadata with new values)
    /// </summary>
    Task UpdateMetadataAsync(string bucketName, string objectName, Dictionary<string, string> metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metadata from an existing blob
    /// </summary>
    Task<Dictionary<string, string>> GetMetadataAsync(string bucketName, string objectName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an empty blob (0 bytes) in the specified bucket
    /// </summary>
    Task CreateEmptyBlobAsync(string bucketName, string objectName, string contentType = "application/octet-stream", Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for uploading to a blob (PUT)
    /// </summary>
    Task<string> GetPresignedUploadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a presigned URL for downloading a blob (GET)
    /// </summary>
    Task<string> GetPresignedDownloadUrlAsync(string bucketName, string objectName, TimeSpan expiry, CancellationToken cancellationToken = default);
}
