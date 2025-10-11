namespace Blob.Integration.Options;

public class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";

    public string Endpoint { get; set; } = string.Empty;

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public bool UseSSL { get; set; } = true;

    public string Region { get; set; } = "us-east-1";
}
