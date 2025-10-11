using CQRS.Contracts;
using WebApi.Application.Features.TransactionImportFeatures.Commands.ConfirmTransactionImportBatchUpload;
using WebApi.Presentation.Common.Helpers;

namespace WebApi.Presentation.Endpoints.TransactionImportEndpoints.ConfirmTransactionImportBatchUpload;

public class ConfirmTransactionImportBatchUploadEndpoint(ICommandManager<ConfirmTransactionImportBatchUploadRequest> manager)
    : CommandEndpoint<ConfirmTransactionImportBatchUploadRequest>(manager)
{
    public override void Configure()
    {
        Put("transaction-import/confirm");
        Summary(x =>
        {
            x.Summary = "Confirm transaction import batch upload";
            x.Description = "Confirms that a file has been successfully uploaded and validates its integrity";
        });
    }
}
