using System.Globalization;
using Ardalis.Result;
using Blob.Integration.Contracts;
using Domain.Core.Entities;
using Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.TransactionImportFeatures.ConfirmTransactionImportBatchUpload;

internal sealed class ConfirmTransactionImportBatchUploadHandler(
    ITransactionImportBatchQueryRepo transactionImportBatchQueryRepo,
    ITransactionImportBatchCommandRepo transactionImportBatchCommandRepo,
    IBlobService blobService)
    : ICommandManager<ConfirmTransactionImportBatchUploadRequest>
{
    public async Task<Result> Handle(
        ConfirmTransactionImportBatchUploadRequest request,
        CancellationToken cancellationToken)
    {
        TransactionImportBatch? batch = await transactionImportBatchQueryRepo.TransactionImportBatches
            .Include(b => b.File)
            .FirstOrDefaultAsync(b => b.Id == request.ImportBatchId, cancellationToken);

        if (batch is null)
        {
            return Result.NotFound($"Import batch with ID {request.ImportBatchId} was not found.");
        }

        if (batch.File is null)
        {
            return Result.Invalid(new ValidationError("Import batch does not have an associated file."));
        }

        if (batch.Status != TransactionImportBatchStatusEnum.PendingFileUpload)
        {
            return Result.Invalid(new ValidationError($"Import batch is in '{batch.Status}' status. Expected 'PendingFileUpload' status."));
        }

        bool blobExists = await blobService.BlobExistsAsync(
            blobName: batch.File.BlobName,
            containerName: batch.File.BlobContainer,
            cancellationToken: cancellationToken);

        if (!blobExists)
        {
            batch.SetError("File was not found in blob storage.");
            batch.Transition(TransactionImportBatchStatusEnum.Failed);

            await transactionImportBatchCommandRepo.UpdateAsync(batch, true, cancellationToken);

            return Result.Invalid(new ValidationError("The file was not successfully uploaded to blob storage."));
        }

        Dictionary<string, string> metadata = await blobService.GetBlobMetadataAsync(
            blobName: batch.File.BlobName,
            containerName: batch.File.BlobContainer,
            cancellationToken: cancellationToken);

        long actualFileSize = await blobService.GetBlobSizeAsync(
                blobName: batch.File.BlobName,
                containerName: batch.File.BlobContainer,
                cancellationToken: cancellationToken);

        if (actualFileSize == 0)
        {
            batch.SetError($"Uploaded file is empty (0 bytes). Expected {batch.File.SizeInBytes} bytes.");
            batch.Transition(TransactionImportBatchStatusEnum.Failed);

            await transactionImportBatchCommandRepo.UpdateAsync(batch, true, cancellationToken);

            return Result.Invalid(new ValidationError($"The uploaded file is empty. Expected size: {batch.File.SizeInBytes} bytes."));
        }

        if (Math.Abs(actualFileSize - batch.File.SizeInBytes) > 1024)
        {
            batch.SetError($"File size mismatch. Expected: {batch.File.SizeInBytes} bytes, Actual: {actualFileSize} bytes.");
            batch.Transition(TransactionImportBatchStatusEnum.Failed);

            await transactionImportBatchCommandRepo.UpdateAsync(batch, true, cancellationToken);

            return Result.Invalid(new ValidationError($"File size mismatch. Expected: {batch.File.SizeInBytes} bytes, Actual: {actualFileSize} bytes."));
        }

        batch.File.UpdateSha256(request.Sha256Hash);
        batch.File.UpdateSizeInBytes(actualFileSize);

        await blobService.AddBlobMetadataAsync(
            blobName: batch.File.BlobName,
            containerName: batch.File.BlobContainer,
            metadata: new Dictionary<string, string>
            {
                { "ConfirmedAt", DateTimeOffset.UtcNow.ToString("O") },
                { "Sha256", request.Sha256Hash },
                { "ActualFileSize", actualFileSize.ToString(CultureInfo.InvariantCulture) }
            },
            cancellationToken: cancellationToken);

        batch.ClearError();
        batch.Transition(TransactionImportBatchStatusEnum.FileUploaded);

        await transactionImportBatchCommandRepo.UpdateAsync(batch, true, cancellationToken);

        return Result.Success();
    }
}
