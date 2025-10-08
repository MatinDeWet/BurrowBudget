using Domain.Base.Implementation;
using Domain.Core.Enums;

namespace Domain.Core.Entities;
public class TransactionImportRow : Entity<Guid>
{
    public Guid ImportBatchId { get; private set; }
    public virtual TransactionImportBatch ImportBatch { get; private set; }

    // Raw fields (keep them verbatim for audit/replay)
    public DateOnly RawDate { get; private set; }

    public string? RawAmount { get; private set; }

    public string? RawDescription { get; private set; }

    public string? RawFitId { get; private set; }

    public string? RawType { get; private set; }

    // Normalized preview fields (after parsing/mapping steps)
    public DateOnly? NormalizedDate { get; private set; }

    public long? AmountMinor { get; private set; }

    public string? Currency { get; private set; }

    public string? Memo { get; private set; }

    public TransactionImportRowStatusEnum Status { get; private set; }

    public string? StatusReason { get; private set; }

    public static TransactionImportRow Create(
        Guid importBatchId,
        DateOnly rawDate,
        string? rawAmount = null,
        string? rawDescription = null,
        string? rawFitId = null,
        string? rawType = null)
    {
        return new TransactionImportRow
        {
            Id = Guid.CreateVersion7(),
            ImportBatchId = importBatchId,
            RawDate = rawDate,
            RawAmount = rawAmount,
            RawDescription = rawDescription,
            RawFitId = rawFitId,
            RawType = rawType,
            Status = TransactionImportRowStatusEnum.Unprocessed
        };
    }

    public void Update(
        DateOnly? normalizedDate = null,
        long? amountMinor = null,
        string? currency = null,
        string? memo = null,
        TransactionImportRowStatusEnum? status = null,
        string? statusReason = null)
    {
        if (normalizedDate.HasValue)
        {
            NormalizedDate = normalizedDate.Value;
        }

        if (amountMinor.HasValue)
        {
            AmountMinor = amountMinor.Value;
        }

        if (!string.IsNullOrWhiteSpace(currency))
        {
            Currency = currency;
        }

        if (!string.IsNullOrWhiteSpace(memo))
        {
            Memo = memo;
        }

        if (status.HasValue)
        {
            Status = status.Value;
        }

        if (!string.IsNullOrWhiteSpace(statusReason))
        {
            StatusReason = statusReason;
        }
    }

    public void UpdateStatus(
        TransactionImportRowStatusEnum status,
        string? statusReason = null)
    {
        Status = status;
        if (!string.IsNullOrWhiteSpace(statusReason))
        {
            StatusReason = statusReason;
        }
    }
}
