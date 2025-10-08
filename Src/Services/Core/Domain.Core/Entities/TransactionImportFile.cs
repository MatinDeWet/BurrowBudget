using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class TransactionImportFile : Entity<Guid>
{
    public Guid ImportBatchId { get; private set; }
    public virtual TransactionImportBatch ImportBatch { get; private set; }

    public string FullFileName { get; private set; }

    public string FileName { get; private set; }

    public string FileExtension { get; private set; }

    public string MimeType { get; private set; }

    public string BlobContainer { get; private set; }

    public string BlobName { get; private set; }

    public string Sha256 { get; private set; }

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
