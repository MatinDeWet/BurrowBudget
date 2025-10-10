using Microsoft.AspNetCore.StaticFiles;

namespace Blob.Integration.Extensions;
public static class FileExtensions
{
    /// <summary>
    /// Extension method to validate if a file name has a valid file extension.
    /// </summary>
    /// <param name="fileName">The file name to validate</param>
    /// <param name="allowedExtensions">Array of allowed extensions</param>
    /// <returns>True if the file extension is valid, false otherwise</returns>
    public static bool IsValidFileExtension(this string? fileName, params string[] allowedExtensions)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return false;
        }

        string extension = Path.GetExtension(fileName).ToUpperInvariant();
        return allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extension method to validate if a content type is valid.
    /// </summary>
    /// <param name="contentType">The content type to validate</param>
    /// <param name="allowedContentTypes">Array of allowed content types</param>
    /// <returns>True if the content type is valid, false otherwise</returns>
    public static bool IsValidContentType(this string? contentType, params string[] allowedContentTypes)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return false;
        }

        return allowedContentTypes.Contains(contentType, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Extension method to convert file size from megabytes to bytes.
    /// </summary>
    /// <param name="sizeInMB">Size in megabytes</param>
    /// <returns>Size in bytes</returns>
    public static long ToBytes(this int sizeInMB)
    {
        return sizeInMB * 1024L * 1024L;
    }

    /// <summary>
    /// Extension method to convert file size from megabytes to bytes.
    /// </summary>
    /// <param name="sizeInMB">Size in megabytes</param>
    /// <returns>Size in bytes</returns>
    public static long ToBytes(this double sizeInMB)
    {
        return (long)(sizeInMB * 1024 * 1024);
    }

    /// <summary>
    /// Extension method to convert bytes to megabytes.
    /// </summary>
    /// <param name="sizeInBytes">Size in bytes</param>
    /// <returns>Size in megabytes</returns>
    public static double ToMegabytes(this long sizeInBytes)
    {
        return sizeInBytes / (1024.0 * 1024.0);
    }

    /// <summary>
    /// Extension method to format file size in a human-readable format.
    /// </summary>
    /// <param name="sizeInBytes">Size in bytes</param>
    /// <returns>Formatted file size string (e.g., "1.5 MB", "2.3 GB")</returns>
    public static string ToHumanReadableSize(this long sizeInBytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double size = sizeInBytes;
        int order = 0;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Extension method to check if a file size exceeds the maximum allowed size.
    /// </summary>
    /// <param name="fileSizeInBytes">File size in bytes</param>
    /// <param name="maxSizeInMB">Maximum allowed size in megabytes</param>
    /// <returns>True if file size exceeds the limit, false otherwise</returns>
    public static bool ExceedsMaxSize(this long fileSizeInBytes, int maxSizeInMB)
    {
        long maxSizeInBytes = maxSizeInMB.ToBytes();
        return fileSizeInBytes > maxSizeInBytes;
    }

    /// <summary>
    /// Extension method to get the filename without extension.
    /// </summary>
    /// <param name="fileName">The full filename</param>
    /// <returns>The filename without extension, or empty string if input is null/empty</returns>
    public static string GetFileNameWithoutExtension(this string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        return Path.GetFileNameWithoutExtension(fileName);
    }

    /// <summary>
    /// Extension method to get the file extension (including the dot).
    /// </summary>
    /// <param name="fileName">The filename to get extension from</param>
    /// <returns>The file extension including the dot (e.g., ".pdf"), or empty string if no extension or input is null/empty</returns>
    public static string GetFileExtension(this string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return string.Empty;
        }

        return Path.GetExtension(fileName);
    }

    /// <summary>
    /// Extension method to get the MIME type (Content-Type) for a file.
    /// </summary>
    /// <param name="fileName">The filename to get MIME type for</param>
    /// <returns>The MIME type, or "application/octet-stream" if not found</returns>
    public static string GetMimeType(this string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return "application/octet-stream";
        }

        var provider = new FileExtensionContentTypeProvider();

        if (provider.TryGetContentType(fileName, out string? contentType))
        {
            return contentType;
        }

        return "application/octet-stream";
    }

    /// <summary>
    /// Extension method to get the file size from a stream.
    /// </summary>
    /// <param name="stream">The stream to get size from</param>
    /// <returns>The size in bytes, or 0 if stream cannot seek</returns>
    public static long GetFileSize(this Stream? stream)
    {
        if (stream == null || !stream.CanSeek)
        {
            return 0;
        }

        return stream.Length;
    }

    /// <summary>
    /// Extension method to get the file size from a FileInfo.
    /// </summary>
    /// <param name="fileInfo">The FileInfo to get size from</param>
    /// <returns>The size in bytes, or 0 if fileInfo is null or file doesn't exist</returns>
    public static long GetFileSize(this FileInfo? fileInfo)
    {
        if (fileInfo == null || !fileInfo.Exists)
        {
            return 0;
        }

        return fileInfo.Length;
    }

    /// <summary>
    /// Extension method to get file size from a file path.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>The size in bytes, or 0 if file doesn't exist or path is invalid</returns>
    public static long GetFileSize(this string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return 0;
        }

        var fileInfo = new FileInfo(filePath);
        return fileInfo.GetFileSize();
    }
}
