namespace WebApi.Application.Features.TransactionImportFeatures.PrepareTransactionImportBatchUpload;
public sealed record PrepareTransactionImportBatchUploadRequest : ICommand<PrepareTransactionImportBatchUploadResponse>
{
    public string FileName { get; init; }

    public string ContentType { get; init; }

    public long FileSize { get; init; }

    public Guid AccountId { get; init; }
}
