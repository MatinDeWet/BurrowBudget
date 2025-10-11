using WebApi.Application.Features.TransactionImportFeatures.ConfirmTransactionImportBatchUpload;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.TransactionImportEndpoints.ConfirmTransactionImportBatchUpload;

public class ConfirmTransactionImportBatchUploadRequestValidator : Validator<ConfirmTransactionImportBatchUploadRequest>
{
    public ConfirmTransactionImportBatchUploadRequestValidator()
    {
        RuleFor(x => x.ImportBatchId)
            .NotEmpty();

        RuleFor(x => x.Sha256Hash)
            .NotEmpty()
            .WithMessage("SHA256 hash is required.")
            .Length(64)
            .WithMessage("SHA256 hash must be exactly 64 characters.")
            .Matches("^[a-fA-F0-9]{64}$")
            .WithMessage("SHA256 hash must be a valid hexadecimal string.");
    }
}
