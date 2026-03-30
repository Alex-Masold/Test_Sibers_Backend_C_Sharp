using Domain.Stores;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace RedisService.Repositories;

public class RefreshTokenRepository(
    IConnectionMultiplexer redis,
    ILogger<RefreshTokenRepository> logger
) : IRefreshTokenStore
{
    private static readonly TimeSpan TokenTTL = TimeSpan.FromDays(7);

    private readonly IDatabase db = redis.GetDatabase();

    private static RedisKey TokenKey(string token) => $"refresh:{token}";

    private static RedisKey UserTokensKey(int userId) => $"userId_token:{userId}";

    private static double ToUnixSeconds(DateTimeOffset dt) => dt.ToUnixTimeSeconds();

    private static readonly LuaScript DeleteByUserIdScript = LuaScript.Prepare(
        LoadScript("DeleteByUserId.lua")
    );

    private static string LoadScript(string fileName)
    {
        var assembly = typeof(RefreshTokenRepository).Assembly;
        var resourceName = $"RedisService.Scripts.{fileName}";

        using var stream =
            assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Lua script '{resourceName}' not found");
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }

    public async Task SaveAsync(
        string token,
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        var tokenKey = TokenKey(token);
        var userKey = UserTokensKey(userId);
        var expiresAt = DateTimeOffset.UtcNow.Add(TokenTTL);

        var transaction = db.CreateTransaction();

        _ = transaction.StringSetAsync(tokenKey, userId, TokenTTL);

        _ = transaction.SortedSetAddAsync(userKey, token, ToUnixSeconds(expiresAt));

        _ = transaction.SortedSetRemoveRangeByScoreAsync(
            userKey,
            0,
            ToUnixSeconds(DateTimeOffset.UtcNow)
        ); // score < now = expired

        _ = transaction.KeyExpireAsync(userKey, TokenTTL);

        bool committed = await transaction.ExecuteAsync();
        if (!committed)
        {
            logger.LogWarning("Redis transaction failed in SaveAsync for userId {UserId}", userId);
        }
    }

    public async Task<int?> GetUserIdAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var value = await db.StringGetAsync(TokenKey(token));
        if (value.HasValue && int.TryParse(value, out var id))
            return id;

        return null;
    }

    public async Task DeleteByTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    )
    {
        var tokenKey = TokenKey(token);
        var userId = await GetUserIdAsync(token, cancellationToken);
        if (!userId.HasValue)
        {
            await db.KeyDeleteAsync(tokenKey);
            return;
        }

        var userKey = UserTokensKey(userId.Value);

        var transaction = db.CreateTransaction();

        transaction.AddCondition(Condition.KeyExists(tokenKey));

        _ = transaction.KeyDeleteAsync(tokenKey);
        _ = transaction.SortedSetRemoveAsync(userKey, token);

        bool committed = await transaction.ExecuteAsync();
        if (!committed)
        {
            logger.LogWarning(
                "Token {TokenPrefix}... already deleted (race condition)",
                token[..Math.Min(8, token.Length)]
            );
        }
    }

    public async Task DeleteByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userKey = UserTokensKey(userId);

        var result = await db.ScriptEvaluateAsync(DeleteByUserIdScript, new { key = userKey }); // RedisKey → KEYS[1]

        logger.LogDebug("Deleted {Count} keys for userId {UserId}", (int)result, userId);
    }

    public async Task DeleteByUserIdAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        var now = ToUnixSeconds(DateTimeOffset.UtcNow);

        var batch = db.CreateBatch();
        var memberTasks = new Dictionary<int, Task<RedisValue[]>>();

        foreach (var userId in idList.Distinct())
        {
            memberTasks[userId] = batch.SortedSetRangeByScoreAsync(
                UserTokensKey(userId),
                start: now,
                stop: double.PositiveInfinity
            );
        }

        batch.Execute();
        await Task.WhenAll(memberTasks.Values);

        var keysToDelete = new List<RedisKey>();

        foreach (var (userId, task) in memberTasks)
        {
            keysToDelete.Add(UserTokensKey(userId));

            foreach (var token in task.Result)
            {
                keysToDelete.Add(TokenKey(token.ToString()));
            }
        }

        if (keysToDelete.Count > 0)
        {
            await db.KeyDeleteAsync(keysToDelete.ToArray());
        }
    }
}
