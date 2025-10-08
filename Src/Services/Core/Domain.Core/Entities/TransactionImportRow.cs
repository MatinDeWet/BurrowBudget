using Domain.Base.Implementation;
using Domain.Core.Enums;

namespace Domain.Core.Entities;
public class TransactionImportRow : Entity<Guid>
{
    public Guid ImportBatchId { get; set; }
    public virtual TransactionImportBatch ImportBatch { get; set; }

    // Raw fields (keep them verbatim for audit/replay)
    public DateOnly RawDate { get; set; }

    public string? RawAmount { get; set; }

    public string? RawDescription { get; set; }

    public string? RawFitId { get; set; }

    public string? RawType { get; set; }

    // Normalized preview fields (after parsing/mapping steps)
    public DateOnly? NormalizedDate { get; set; }

    public long? AmountMinor { get; set; }

    public string? Currency { get; set; }

    public string? Memo { get; set; }

    public TransactionImportRowStatusEnum Status { get; set; } = TransactionImportRowStatusEnum.Unprocessed;

    public string? StatusReason { get; set; }
}
