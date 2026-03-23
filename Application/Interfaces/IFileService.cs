using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    FileStream GetFileStream(string storedFileName);
    void DeleteFile(string storedFileName);
}