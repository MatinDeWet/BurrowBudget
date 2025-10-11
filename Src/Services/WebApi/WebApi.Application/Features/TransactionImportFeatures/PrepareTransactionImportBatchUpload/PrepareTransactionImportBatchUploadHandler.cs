using System.Globalization;
using Ardalis.Result;
using Blob.Integration.Contracts;
using Blob.Integration.Extensions;
using Domain.Core.Constants;
using Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Repositories.Command;
using WebApi.Application.Repositories.Query;

namespace WebApi.Application.Features.TransactionImportFeatures.PrepareTransactionImportBatchUpload;
internal sealed class PrepareTransactionImportBatchUploadHandler(
    IAccountQueryRepo accountQueryRepo,
    ITransactionImportBatchCommandRepo transactionImportBatchCommandRepo,
    IBlobService blobService)
    : ICommandManager<PrepareTransactionImportBatchUploadRequest, PrepareTransactionImportBatchUploadResponse>
{
    private const string ContainerName = BlobNameConstants.TransactionImport;
    private static readonly TimeSpan PresignedUrlExpiration = TimeSpan.FromHours(2);

    public async Task<Result<PrepareTransactionImportBatchUploadResponse>> Handle(PrepareTransactionImportBatchUploadRequest request, CancellationToken cancellationToken)
    {
        bool hasAccount = await accountQueryRepo.Accounts
            .AnyAsync(a => a.Id == request.AccountId, cancellationToken);

        if (!hasAccount)
        {
            return Result.NotFound($"Account with ID {request.AccountId} was not found.");
        }

        // Generate a unique blob name for the upload
        string blobName = $"{request.AccountId}/{Guid.NewGuid()}/{request.FileName}";

        await blobService.CreateEmptyBlobAsync(
            containerName: ContainerName,
            blobName: blobName,
            contentType: request.ContentType,
            metadata: new Dictionary<string, string>
            {
                { "AccountId", request.AccountId.ToString() },
                { "OriginalFileName", request.FileName },
                { "ExpectedFileSize", request.FileSize.ToString(CultureInfo.InvariantCulture) },
                { "UploadInitiatedAt", DateTimeOffset.UtcNow.ToString("O") }
            },
            cancellationToken: cancellationToken);

        string uploadUrl = await blobService.GetPresignedUploadUrlAsync(
            containerName: ContainerName,
            blobName: blobName,
            expiry: PresignedUrlExpiration,
            cancellationToken: cancellationToken);

        var importFile = TransactionImportFile.Create(
            fullFileName: request.FileName,
            fileName: request.FileName.GetFileNameWithoutExtension(),
            fileExtension: request.FileName.GetFileExtension(),
            mimeType: request.ContentType,
            blobContainer: ContainerName,
            blobName: blobName,
            sizeInBytes: request.FileSize);

        var batch = TransactionImportBatch.Create(request.AccountId, importFile);

        await transactionImportBatchCommandRepo.InsertAsync(batch, true, cancellationToken);

        return new PrepareTransactionImportBatchUploadResponse
        {
            ImportBatchId = batch.Id,
            UploadUrl = uploadUrl,
            SasExpiresAt = DateTimeOffset.UtcNow.Add(PresignedUrlExpiration)
        };
    }
}
