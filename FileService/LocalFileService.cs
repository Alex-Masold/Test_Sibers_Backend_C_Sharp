using Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace FileService;

public class LocalFileService(IWebHostEnvironment environment) : IFileService
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
        IFormFile file,
        CancellationToken cancellationToken = default
    )
    {
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);

        var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = GetSafePath(storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream, cancellationToken);

        return storedFileName;
    }

    public FileStream GetFileStream(string storedFileName)
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
            File.Delete(fullPath);
    }
}
