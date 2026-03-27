namespace Domain.Stores;

public interface IRefreshTokenStore
{
    Task SaveAsync(string token, int userId, CancellationToken cancellationToken = default);
    Task<int?> GetUserIdAsync(string token, CancellationToken cancellationToken = default);
    Task DeleteAsync(string token, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    );
}
