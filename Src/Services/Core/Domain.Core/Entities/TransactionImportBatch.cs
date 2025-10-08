using Domain.Base.Implementation;
using Domain.Core.Enums;
using Domain.Core.Extensions;

namespace Domain.Core.Entities;
public class TransactionImportBatch : Entity<Guid>
{
    public Guid AccountId { get; private set; }
    public virtual Account Account { get; private set; }

    public DateTimeOffset ImportedAt { get; private set; }

    // Status & telemetry
    public TransactionImportBatchStatusEnum Status { get; private set; }

    public int RetryCount { get; private set; }

    public string? Error { get; private set; }

    // Timestamps (audit)
    public DateTimeOffset? UploadedAt { get; private set; }

    public DateTimeOffset? QueuedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? FailedAt { get; private set; }

    public DateTimeOffset? CanceledAt { get; private set; }

    public DateTimeOffset? SupersededAt { get; private set; }

    // Concurrency / background processing
    public uint Version { get; private set; }

    public virtual ICollection<TransactionImportRow> Rows { get; private set; } = [];

    public virtual TransactionImportFile File { get; private set; }

    public static TransactionImportBatch Create(Guid accountId)
    {
        var batch = new TransactionImportBatch
        {
            Id = Guid.CreateVersion7(),
            AccountId = accountId,
            Status = TransactionImportBatchStatusEnum.PendingFileUpload
        };

        batch.Stamp(batch.Status, DateTimeOffset.Now);
        return batch;
    }

    public void Transition(TransactionImportBatchStatusEnum next, DateTimeOffset? now = null)
    {
        if (!TransactionJobStatusPolicyExtension.CanTransition(Status, next))
        {
            throw new InvalidOperationException($"Illegal transition {Status} -> {next}");
        }

        Status = next;
        Stamp(next, now ?? DateTimeOffset.UtcNow);
    }

    public void ClearError() => Error = null;

    public void SetError(string? error) => Error = Truncate(error, 2000);

    public void IncrementRetry() => RetryCount++;

    public bool IsTerminal => TransactionJobStatusPolicyExtension.IsTerminal(Status);

    public IReadOnlyList<TransactionImportBatchStatusEnum> AllowedNext() => TransactionJobStatusPolicyExtension.AllowedNext(Status);

    private void Stamp(TransactionImportBatchStatusEnum next, DateTimeOffset ts)
    {
        switch (next)
        {
            case TransactionImportBatchStatusEnum.PendingFileUpload:
                ImportedAt = ts;
                break;
            case TransactionImportBatchStatusEnum.FileUploaded:
                UploadedAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Queued:
                QueuedAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Processing:
                StartedAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Completed:
                CompletedAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Failed:
                FailedAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Canceled:
                CanceledAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Superseded:
                SupersededAt ??= ts;
                break;
            case TransactionImportBatchStatusEnum.Duplicate:
                break;
        }
    }

    private static string? Truncate(string? value, int max) =>
        string.IsNullOrEmpty(value) ? value : (value!.Length <= max ? value : value[..max]);
}
