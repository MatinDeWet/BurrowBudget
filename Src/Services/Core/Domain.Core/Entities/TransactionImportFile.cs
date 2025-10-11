using Domain.Base.Implementation;

namespace Domain.Core.Entities;

public class TransactionImportFile : Entity<Guid>
{
    public Guid ImportBatchId { get; private set; }
    public virtual TransactionImportBatch ImportBatch { get; private set; } = null!;

    public string FullFileName { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string FileExtension { get; private set; } = null!;
    public string MimeType { get; private set; } = null!;
    public string BlobContainer { get; private set; } = null!;
    public string BlobName { get; private set; } = null!;
    public string? Sha256 { get; private set; }
    public long SizeInBytes { get; private set; }

    public static TransactionImportFile Create(
        Guid importBatchId,
        string fullFileName,
        string fileName,
        string fileExtension,
        string mimeType,
        string blobContainer,
        string blobName,
        long sizeInBytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileExtension);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobContainer);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        return new TransactionImportFile
        {
            Id = Guid.CreateVersion7(),
            ImportBatchId = importBatchId,
            FullFileName = fullFileName,
            FileName = fileName,
            FileExtension = fileExtension,
            MimeType = mimeType,
            BlobContainer = blobContainer,
            BlobName = blobName,
            SizeInBytes = sizeInBytes
        };
    }

    public static TransactionImportFile Create(
        string fullFileName,
        string fileName,
        string fileExtension,
        string mimeType,
        string blobContainer,
        string blobName,
        long sizeInBytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileExtension);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobContainer);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);

        return new TransactionImportFile
        {
            Id = Guid.CreateVersion7(),
            FullFileName = fullFileName,
            FileName = fileName,
            FileExtension = fileExtension,
            MimeType = mimeType,
            BlobContainer = blobContainer,
            BlobName = blobName,
            SizeInBytes = sizeInBytes
        };
    }

    /// <summary>
    /// Updates the SHA256 hash after file upload confirmation
    /// </summary>
    public void UpdateSha256(string sha256)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sha256);
        Sha256 = sha256;
    }

    /// <summary>
    /// Updates the actual file size after upload confirmation
    /// </summary>
    public void UpdateSizeInBytes(long sizeInBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sizeInBytes);
        SizeInBytes = sizeInBytes;
    }
}
