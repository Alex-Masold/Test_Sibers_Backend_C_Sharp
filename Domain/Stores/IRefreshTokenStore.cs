namespace Domain.Stores;

public interface IRefreshTokenStore
{
    Task Save(string token, int userId, CancellationToken cancellationToken = default);
    Task<int?> GetUserId(string token, CancellationToken cancellationToken = default);
    Task Delete(string token, CancellationToken cancellationToken = default);
    Task Delete(int userId, CancellationToken cancellationToken = default);
    Task Delete(IReadOnlyCollection<int> idList, CancellationToken cancellationToken = default);
}
