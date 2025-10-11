using Blob.Integration.Contracts;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;

namespace Blob.Integration.Implementation;

public sealed class BlobService : IBlobService
{
    private readonly IMinioClient _minioClient;

    public BlobService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<string> UploadBlobAsync(
        string containerName, 
        string blobName, 
        Stream data, 
        string contentType, 
        Dictionary<string, string>? metadata = null, 
        CancellationToken cancellationToken = default)
    {
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(containerName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(containerName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithStreamData(data)
            .WithObjectSize(data.Length)
            .WithContentType(contentType);

        if (metadata != null && metadata.Count > 0)
        {
            putObjectArgs.WithHeaders(metadata);
        }

        PutObjectResponse response = await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
        
        return response.ObjectName;
    }

    public async Task<Stream> DownloadBlobAsync(
        string containerName, 
        string blobName, 
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        GetObjectArgs getObjectArgs = new GetObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteBlobAsync(
        string containerName, 
        string blobName, 
        CancellationToken cancellationToken = default)
    {
        RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }

    public async Task AddMetadataAsync(
        string containerName, 
        string blobName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> existingMetadata = await GetMetadataAsync(containerName, blobName, cancellationToken);

        foreach (KeyValuePair<string, string> kvp in metadata)
        {
            existingMetadata[kvp.Key] = kvp.Value;
        }

        await UpdateMetadataInternalAsync(containerName, blobName, existingMetadata, cancellationToken);
    }

    public async Task RemoveMetadataAsync(
        string containerName, 
        string blobName, 
        IEnumerable<string> metadataKeys, 
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, string> existingMetadata = await GetMetadataAsync(containerName, blobName, cancellationToken);

        foreach (string key in metadataKeys)
        {
            existingMetadata.Remove(key);
        }

        await UpdateMetadataInternalAsync(containerName, blobName, existingMetadata, cancellationToken);
    }

    public async Task UpdateMetadataAsync(
        string containerName, 
        string blobName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken = default)
    {
        await UpdateMetadataInternalAsync(containerName, blobName, metadata, cancellationToken);
    }

    public async Task<Dictionary<string, string>> GetMetadataAsync(
        string containerName, 
        string blobName, 
        CancellationToken cancellationToken = default)
    {
        StatObjectArgs statObjectArgs = new StatObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName);

        Minio.DataModel.ObjectStat objectStat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

        var metadata = new Dictionary<string, string>();

        if (objectStat.MetaData != null)
        {
            foreach (KeyValuePair<string, string> kvp in objectStat.MetaData)
            {
                metadata[kvp.Key] = kvp.Value;
            }
        }

        return metadata;
    }

    private async Task UpdateMetadataInternalAsync(
        string containerName, 
        string blobName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken)
    {
        CopySourceObjectArgs copySourceObjectArgs = new CopySourceObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName);

        CopyObjectArgs copyObjectArgs = new CopyObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithCopyObjectSource(copySourceObjectArgs)
            .WithReplaceMetadataDirective(true);

        if (metadata != null && metadata.Count > 0)
        {
            copyObjectArgs.WithHeaders(metadata);
        }

        await _minioClient.CopyObjectAsync(copyObjectArgs, cancellationToken);
    }

    public async Task CreateEmptyBlobAsync(
        string containerName, 
        string blobName, 
        string contentType = "application/octet-stream", 
        Dictionary<string, string>? metadata = null, 
        CancellationToken cancellationToken = default)
    {
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(containerName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(containerName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        using var emptyStream = new MemoryStream();

        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithStreamData(emptyStream)
            .WithObjectSize(0)
            .WithContentType(contentType);

        if (metadata != null && metadata.Count > 0)
        {
            putObjectArgs.WithHeaders(metadata);
        }

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);
    }

    public async Task<string> GetPresignedUploadUrlAsync(
        string containerName, 
        string blobName, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(containerName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(containerName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        int expirySeconds = (int)expiry.TotalSeconds;

        PresignedPutObjectArgs presignedPutObjectArgs = new PresignedPutObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedPutObjectAsync(presignedPutObjectArgs);
    }

    public async Task<string> GetPresignedDownloadUrlAsync(
        string containerName, 
        string blobName, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        int expirySeconds = (int)expiry.TotalSeconds;

        PresignedGetObjectArgs presignedGetObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
    }

    public async Task<bool> BlobExistsAsync(
        string containerName, 
        string blobName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            StatObjectArgs statObjectArgs = new StatObjectArgs()
                .WithBucket(containerName)
                .WithObject(blobName);

            await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<long> GetBlobSizeAsync(
        string containerName, 
        string blobName, 
        CancellationToken cancellationToken = default)
    {
        StatObjectArgs statObjectArgs = new StatObjectArgs()
            .WithBucket(containerName)
            .WithObject(blobName);

        ObjectStat objectStat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);
        
        return objectStat.Size;
    }
}
