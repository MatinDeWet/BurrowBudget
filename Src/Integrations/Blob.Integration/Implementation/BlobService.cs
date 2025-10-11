using Blob.Integration.Contracts;
using Minio;
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
        string bucketName, 
        string objectName, 
        Stream data, 
        string contentType, 
        Dictionary<string, string>? metadata = null, 
        CancellationToken cancellationToken = default)
    {
        // Ensure bucket exists
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        // Upload the object
        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
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
        string bucketName, 
        string objectName, 
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        GetObjectArgs getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCallbackStream(stream => stream.CopyTo(memoryStream));

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
        
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteBlobAsync(
        string bucketName, 
        string objectName, 
        CancellationToken cancellationToken = default)
    {
        RemoveObjectArgs removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }

    public async Task AddMetadataAsync(
        string bucketName, 
        string objectName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken = default)
    {
        // Get existing metadata
        Dictionary<string, string> existingMetadata = await GetMetadataAsync(bucketName, objectName, cancellationToken);

        // Merge with new metadata
        foreach (KeyValuePair<string, string> kvp in metadata)
        {
            existingMetadata[kvp.Key] = kvp.Value;
        }

        // Update metadata using copy operation
        await UpdateMetadataInternalAsync(bucketName, objectName, existingMetadata, cancellationToken);
    }

    public async Task RemoveMetadataAsync(
        string bucketName, 
        string objectName, 
        IEnumerable<string> metadataKeys, 
        CancellationToken cancellationToken = default)
    {
        // Get existing metadata
        Dictionary<string, string> existingMetadata = await GetMetadataAsync(bucketName, objectName, cancellationToken);

        // Remove specified keys
        foreach (string key in metadataKeys)
        {
            existingMetadata.Remove(key);
        }

        // Update metadata using copy operation
        await UpdateMetadataInternalAsync(bucketName, objectName, existingMetadata, cancellationToken);
    }

    public async Task UpdateMetadataAsync(
        string bucketName, 
        string objectName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken = default)
    {
        await UpdateMetadataInternalAsync(bucketName, objectName, metadata, cancellationToken);
    }

    public async Task<Dictionary<string, string>> GetMetadataAsync(
        string bucketName, 
        string objectName, 
        CancellationToken cancellationToken = default)
    {
        StatObjectArgs statObjectArgs = new StatObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        Minio.DataModel.ObjectStat objectStat = await _minioClient.StatObjectAsync(statObjectArgs, cancellationToken);

        var metadata = new Dictionary<string, string>();

        if (objectStat.MetaData != null)
        {
            foreach (KeyValuePair<string, string> kvp in objectStat.MetaData)
            {
                // MinIO returns metadata keys with "x-amz-meta-" prefix, we'll store them as-is
                // but you can strip the prefix if needed
                metadata[kvp.Key] = kvp.Value;
            }
        }

        return metadata;
    }

    private async Task UpdateMetadataInternalAsync(
        string bucketName, 
        string objectName, 
        Dictionary<string, string> metadata, 
        CancellationToken cancellationToken)
    {
        // MinIO doesn't support direct metadata update, so we use copy operation
        // with REPLACE directive to update metadata
        CopySourceObjectArgs copySourceObjectArgs = new CopySourceObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName);

        CopyObjectArgs copyObjectArgs = new CopyObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithCopyObjectSource(copySourceObjectArgs)
            .WithReplaceMetadataDirective(true);

        if (metadata != null && metadata.Count > 0)
        {
            copyObjectArgs.WithHeaders(metadata);
        }

        await _minioClient.CopyObjectAsync(copyObjectArgs, cancellationToken);
    }

    public async Task CreateEmptyBlobAsync(
        string bucketName, 
        string objectName, 
        string contentType = "application/octet-stream", 
        Dictionary<string, string>? metadata = null, 
        CancellationToken cancellationToken = default)
    {
        // Ensure bucket exists
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        // Create an empty stream (0 bytes)
        using var emptyStream = new MemoryStream();

        PutObjectArgs putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
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
        string bucketName, 
        string objectName, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        // Ensure bucket exists
        BucketExistsArgs bucketExistsArgs = new BucketExistsArgs()
            .WithBucket(bucketName);
        
        bool bucketExists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        
        if (!bucketExists)
        {
            MakeBucketArgs makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }

        int expirySeconds = (int)expiry.TotalSeconds;

        PresignedPutObjectArgs presignedPutObjectArgs = new PresignedPutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedPutObjectAsync(presignedPutObjectArgs);
    }

    public async Task<string> GetPresignedDownloadUrlAsync(
        string bucketName, 
        string objectName, 
        TimeSpan expiry, 
        CancellationToken cancellationToken = default)
    {
        int expirySeconds = (int)expiry.TotalSeconds;

        PresignedGetObjectArgs presignedGetObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
    }
}
