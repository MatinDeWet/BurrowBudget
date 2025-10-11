using System.Globalization;
using Ardalis.Result;
using Azure.Storage.Sas;
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
    private static readonly TimeSpan SasTokenExpiration = TimeSpan.FromHours(2);

    public async Task<Result<PrepareTransactionImportBatchUploadResponse>> Handle(PrepareTransactionImportBatchUploadRequest request, CancellationToken cancellationToken)
    {
        bool hasAccount = await accountQueryRepo.Accounts
            .AnyAsync(a => a.Id == request.AccountId, cancellationToken);

        if (!hasAccount)
        {
            return Result.NotFound($"Account with ID {request.AccountId} was not found.");
        }

        string blobName = await blobService.CreateEmptyBlobAsync(
            containerName: ContainerName,
            contentType: request.ContentType,
            metadata: new Dictionary<string, string>
            {
                { "AccountId", request.AccountId.ToString() },
                { "OriginalFileName", request.FileName },
                { "ExpectedFileSize", request.FileSize.ToString(CultureInfo.InvariantCulture) },
                { "UploadInitiatedAt", DateTimeOffset.UtcNow.ToString("O") }
            },
            cancellationToken: cancellationToken);

        Uri sasUri = await blobService.CreateBlobSasTokenAsync(
            blobName: blobName,
            containerName: ContainerName,
            expiration: SasTokenExpiration,
            permissions: BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Create,
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
            UploadUrl = sasUri.ToString(),
            SasExpiresAt = DateTimeOffset.UtcNow.Add(SasTokenExpiration)
        };
    }
}
