using Domain.Exceptions;
using Domain.Stores;

namespace Application.Extensions;

public static class RefreshTokenStoreExtensions
{
    public static async Task<int> GetIdOrThrowAsync(
        this IRefreshTokenStore refreshTokenStore,
        string refreshToken,
        CancellationToken ct = default
    )
    {
        var employeeId = await refreshTokenStore.GetUserIdAsync(refreshToken, ct);
        if (!employeeId.HasValue)
            throw new AuthenticationException("Refresh token not found or expired");
        return employeeId.Value;
    }
}
