using WebApi.Application.Features.TransactionImportFeatures.PrepareTransactionImportBatchUpload;
using WebApi.Presentation.Common.Validation;

namespace WebApi.Presentation.Endpoints.TransactionImportEndpoints.PrepareTransactionImportBatchUpload;

public class PrepareTransactionImportBatchUploadRequestValidator : Validator<PrepareTransactionImportBatchUploadRequest>
{
    public PrepareTransactionImportBatchUploadRequestValidator()
    {
        RuleFor(x => x.FileName)
            .StringInput(255);

        RuleFor(x => x.ContentType)
            .StringInput(100);

        RuleFor(x => x.FileSize)
            .GreaterThan(0)
            .WithMessage("File size must be greater than 0 bytes.")
            .LessThanOrEqualTo(5 * 1024 * 1024) // 5 MB limit
            .WithMessage("File size must not exceed 100 MB.");

        RuleFor(x => x.AccountId)
            .NotEmpty();
    }
}
