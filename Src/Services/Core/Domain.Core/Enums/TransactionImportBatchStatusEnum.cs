namespace Domain.Core.Enums;
public enum TransactionImportBatchStatusEnum
{
    PendingFileUpload = 0,
    FileUploaded = 1,
    Queued = 2,
    Processing = 3,
    Completed = 4,
    Failed = 5,
    Canceled = 6,
    Duplicate = 7,
    Superseded = 8
}
