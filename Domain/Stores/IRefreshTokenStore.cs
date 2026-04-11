namespace Domain.Stores;

public interface IRefreshTokenStore
{
    Task SaveAsync(string token, int userId, CancellationToken cancellationToken = default);
    Task<int?> GetUserIdAsync(string token, CancellationToken cancellationToken = default);
    Task DeleteByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteByUserIdAsync(
        IReadOnlyCollection<int> userIdList,
        CancellationToken cancellationToken = default
    );
}
