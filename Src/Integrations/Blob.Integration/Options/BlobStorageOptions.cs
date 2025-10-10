namespace Blob.Integration.Options;

public class BlobStorageOptions
{
    public const string SectionName = "BlobStorage";

    /// <summary>
    /// Azure Storage connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Default container name for blob operations
    /// </summary>
    public string DefaultContainerName { get; set; } = "default";

    /// <summary>
    /// Maximum file size allowed in MB
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 100;

    /// <summary>
    /// Allowed file extensions
    /// </summary>
    public string[] AllowedFileExtensions { get; set; } = [];

    /// <summary>
    /// Whether to create containers automatically if they don't exist
    /// </summary>
    public bool AutoCreateContainers { get; set; } = true;

    /// <summary>
    /// Default public access level for containers
    /// </summary>
    public string DefaultPublicAccessLevel { get; set; } = "None";
}
