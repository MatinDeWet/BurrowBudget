using Domain.Base.Implementation;

namespace Domain.Core.Entities;
public class TransactionImportFile : Entity<Guid>
{
    public Guid ImportBatchId { get; set; }
    public virtual TransactionImportBatch ImportBatch { get; set; }

    public string FullFileName { get; private set; }

    public string FileName { get; private set; }

    public string FileExtension { get; private set; }

    public string MimeType { get; private set; }

    public string BlobContainer { get; private set; }

    public string BlobName { get; private set; }

    public string Sha256 { get; private set; }

    public long SizeInBytes { get; private set; }
}
