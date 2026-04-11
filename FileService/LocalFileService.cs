using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace FileService.Services;

public class LocalFileService(IWebHostEnvironment environment, ILogger<IFileService> logger)
    : IFileService
{
    private const string UploadDirectory = "Uploads";
    private readonly string _uploadPath = Path.Combine(
        environment.WebRootPath ?? environment.ContentRootPath,
        UploadDirectory
    );

    private string GetSafePath(string storedFileName)
    {
        var fileName = Path.GetFileName(storedFileName);
        if (string.IsNullOrEmpty(fileName) || fileName != storedFileName)
            throw new ArgumentException("Invalid file name", nameof(storedFileName));

        var fullPath = Path.GetFullPath(Path.Combine(_uploadPath, fileName));

        if (!fullPath.StartsWith(_uploadPath + Path.DirectorySeparatorChar))
            throw new ArgumentException("Path traversal detected");

        return fullPath;
    }

    public async Task<string> SaveFileAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default
    )
    {
        Directory.CreateDirectory(_uploadPath);

        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var fullPath = GetSafePath(storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await content.CopyToAsync(stream, cancellationToken);

        return storedFileName;
    }

    public Stream GetFileStream(string storedFileName)
    {
        var fullPath = GetSafePath(storedFileName);

        if (!File.Exists(fullPath))
            throw new FileNotFoundException("File not found on server", storedFileName);

        return new FileStream(fullPath, FileMode.Open, FileAccess.Read);
    }

    public void DeleteFile(string storedFileName)
    {
        var fullPath = GetSafePath(storedFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return;
        }

        logger.LogWarning($"{fullPath} not exist");
    }
}
