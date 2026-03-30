namespace Application.Interfaces;

public interface IFileService
{
    Task<string> SaveFileAsync(
        Stream content,
        string fileName,
        CancellationToken cancellationToken = default
    );
    Stream GetFileStream(string storedFileName);
    void DeleteFile(string storedFileName);
}

