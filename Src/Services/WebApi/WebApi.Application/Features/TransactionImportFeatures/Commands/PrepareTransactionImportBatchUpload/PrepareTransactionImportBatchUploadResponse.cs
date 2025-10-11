namespace WebApi.Application.Features.TransactionImportFeatures.Commands.PrepareTransactionImportBatchUpload;
public sealed record PrepareTransactionImportBatchUploadResponse
{
    public Guid ImportBatchId { get; init; }
    
    public string UploadUrl { get; init; } = null!;
    
    public DateTimeOffset SasExpiresAt { get; init; }
}
