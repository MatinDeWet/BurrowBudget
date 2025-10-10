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
    public string Sha256 { get; private set; } = null!;
    public long SizeInBytes { get; private set; }

    public static TransactionImportFile Create(
        Guid importBatchId,
        string fullFileName,
        string fileName,
        string fileExtension,
        string mimeType,
        string blobContainer,
        string blobName,
        string sha256,
        long sizeInBytes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullFileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileExtension);
        ArgumentException.ThrowIfNullOrWhiteSpace(mimeType);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobContainer);
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);
        ArgumentException.ThrowIfNullOrWhiteSpace(sha256);
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
            Sha256 = sha256,
            SizeInBytes = sizeInBytes
        };
    }
}
