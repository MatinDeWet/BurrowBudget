using Blob.Integration.Contracts;
using Blob.Integration.Implementation;
using Blob.Integration.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blob.Integration;

public static class BlobStorageDI
{
    public static IServiceCollection AddBlobSupport(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlobStorageOptions>(options =>
        {
            configuration.GetSection(BlobStorageOptions.SectionName).Bind(options);

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException($"BlobStorage:ConnectionString is required in configuration.");
            }
        });

        services.AddScoped<IBlobService, BlobService>();

        return services;
    }

    public static IServiceCollection AddBlobSupport(this IServiceCollection services, Action<BlobStorageOptions> configureOptions)
    {
        services.Configure<BlobStorageOptions>(options =>
        {
            configureOptions(options);

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString is required in BlobStorageOptions.");
            }
        });

        services.AddScoped<IBlobService, BlobService>();

        return services;
    }

    /// <summary>
    /// Adds blob storage support with connection string
    /// </summary>
    public static IServiceCollection AddBlobSupport(this IServiceCollection services, string connectionString, Action<BlobStorageOptions>? configureOptions = null)
    {
        services.Configure<BlobStorageOptions>(options =>
        {
            options.ConnectionString = connectionString;
            configureOptions?.Invoke(options);
        });

        services.AddScoped<IBlobService, BlobService>();

        return services;
    }
}
