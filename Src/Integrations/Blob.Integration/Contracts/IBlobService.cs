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
}
