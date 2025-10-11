namespace WebApi.Application.Features.TransactionImportFeatures.ConfirmTransactionImportBatchUpload;

public sealed record ConfirmTransactionImportBatchUploadRequest : ICommand
{
    public Guid ImportBatchId { get; init; }

    public string Sha256Hash { get; init; }
}
