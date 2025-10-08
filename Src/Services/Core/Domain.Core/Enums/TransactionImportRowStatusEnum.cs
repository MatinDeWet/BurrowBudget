namespace Domain.Core.Enums;
public enum TransactionImportRowStatusEnum
{
    Unprocessed = 0,
    Parsed = 1,
    Mapped = 2,
    Promoted = 3,
    Skipped = 4,
    Duplicate = 5,
}
