namespace Domain.Core.Enums;
public enum TransactionImportRowStatusEnum
{
    Unprocessed = 0,
    Parsed = 1,
    Normalized = 2,
    Duplicate = 3,
    Rejected = 4,
    Skipped = 5,
    Imported = 6
}
