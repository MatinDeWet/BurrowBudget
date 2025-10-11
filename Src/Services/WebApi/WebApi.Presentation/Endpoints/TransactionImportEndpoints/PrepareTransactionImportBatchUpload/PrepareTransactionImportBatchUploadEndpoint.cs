using CQRS.Contracts;
using WebApi.Application.Features.TransactionImportFeatures.PrepareTransactionImportBatchUpload;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.TransactionImportEndpoints.PrepareTransactionImportBatchUpload;

public class PrepareTransactionImportBatchUploadEndpoint(ICommandManager<PrepareTransactionImportBatchUploadRequest, PrepareTransactionImportBatchUploadResponse> manager)
    : CommandEndpoint<PrepareTransactionImportBatchUploadRequest, PrepareTransactionImportBatchUploadResponse>(manager)
{
    public override void Configure()
    {
        Post("transaction-import/prepare");
        Summary(x =>
        {
            x.Summary = "Prepare transaction import batch upload";
            x.Description = "Prepares a new transaction import batch and generates a SAS token for file upload";
        });
    }
}
