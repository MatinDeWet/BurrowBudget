namespace Blob.Integration.Contracts;

public interface IBlobService
{
    Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);

    Task DeleteBlobAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, string>> GetBlobMetadataAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);

    Task AddBlobMetadataAsync(
        string blobName,
        string containerName,
        Dictionary<string, string> metadata,
        CancellationToken cancellationToken = default);

    Task<string> CreateEmptyBlobAsync(
        string containerName,
        string? blobName = null,
        string contentType = "application/octet-stream",
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    Task<Uri> CreateBlobSasTokenAsync(
        string blobName,
        string containerName,
        TimeSpan expiration,
        Azure.Storage.Sas.BlobSasPermissions permissions = Azure.Storage.Sas.BlobSasPermissions.Read,
        CancellationToken cancellationToken = default);

    Task<long> GetBlobSizeAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);

    Task<bool> BlobExistsAsync(
        string blobName,
        string containerName,
        CancellationToken cancellationToken = default);
}
