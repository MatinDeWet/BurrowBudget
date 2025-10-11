using Blob.Integration.Contracts;
using Blob.Integration.Implementation;
using Blob.Integration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace Blob.Integration;

public static class BlobStorageDI
{
    public static IServiceCollection AddBlobSupport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageOptions>(configuration.GetSection(BlobStorageOptions.SectionName));

        BlobStorageOptions blobOptions = configuration.GetSection(BlobStorageOptions.SectionName).Get<BlobStorageOptions>()
            ?? throw new InvalidOperationException("BlobStorage configuration section is missing or invalid.");

        services.AddMinio(client =>
        {
            client.WithEndpoint(blobOptions.Endpoint);
            client.WithCredentials(blobOptions.AccessKey, blobOptions.SecretKey);
            client.WithSSL(blobOptions.UseSSL);
            if (!string.IsNullOrWhiteSpace(blobOptions.Region))
            {
                client.WithRegion(blobOptions.Region);
            }
        });

        services.AddScoped<IBlobService, BlobService>();

        return services;
    }

    public static IServiceCollection AddBlobSupport(this IServiceCollection services, Action<BlobStorageOptions> configureOptions)
    {
        var blobOptions = new BlobStorageOptions();
        configureOptions(blobOptions);

        services.Configure(configureOptions);

        services.AddMinio(client =>
        {
            client.WithEndpoint(blobOptions.Endpoint);
            client.WithCredentials(blobOptions.AccessKey, blobOptions.SecretKey);
            client.WithSSL(blobOptions.UseSSL);
            if (!string.IsNullOrWhiteSpace(blobOptions.Region))
            {
                client.WithRegion(blobOptions.Region);
            }
        });

        services.AddScoped<IBlobService, BlobService>();

        return services;
    }
}
