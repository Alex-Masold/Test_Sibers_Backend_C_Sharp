using Domain.Stores;
using StackExchange.Redis;

namespace RedisService.Repositories;

public class RefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenStore
{
    private static readonly TimeSpan TokenTTL = TimeSpan.FromDays(7);

    private readonly IDatabase db = redis.GetDatabase();

    private static RedisKey TokenKey(string token) => $"refresh:{token}";

    private static RedisKey UserTokensKey(int userId) => $"userId_token:{userId}";

    public async Task Save(string token, int userId, CancellationToken cancellationToken = default)
    {
        var tokenKey = TokenKey(token);
        var userKey = UserTokensKey(userId);

        var transaction = db.CreateTransaction();

        _ = transaction.StringSetAsync(tokenKey, userId, TokenTTL);

        _ = transaction.SetAddAsync(userKey, token);

        _ = transaction.KeyExpireAsync(userKey, TokenTTL);

       await transaction.ExecuteAsync();
    }

    public async Task<int?> GetUserId(string token, CancellationToken cancellationToken = default)
    {
        var value = await db.StringGetAsync(TokenKey(token));
        if (value.HasValue && int.TryParse(value, out var id))
            return id;

        return null;
    }

    public async Task Delete(string token, CancellationToken cancellationToken = default)
    {
        var tokenKey = TokenKey(token);

        var userId = await GetUserId(token, cancellationToken);
        var userKey = UserTokensKey(userId.Value);
        if (!userId.HasValue)
        {
            await db.KeyDeleteAsync(tokenKey);
            return;
        }

        var transaction = db.CreateTransaction();

        transaction.AddCondition(Condition.KeyExists(tokenKey));

        _ = transaction.KeyDeleteAsync(tokenKey);
        _ = transaction.SetRemoveAsync(userKey, token);

        await transaction.ExecuteAsync();
        }

    public async Task Delete(int userId, CancellationToken cancellationToken = default)
    {
        var userKey = UserTokensKey(userId);

        var tokens = await db.SetMembersAsync(userKey);

        if (tokens.Length == 0)
        {
            return;
        }

        var transaction = db.CreateTransaction();

        var keys = tokens.Select(t => TokenKey(t.ToString())).ToArray();

        _ = transaction.KeyDeleteAsync(keys);
        _ = transaction.KeyDeleteAsync(userKey);

       await transaction.ExecuteAsync();
    }

    public async Task Delete(
        IReadOnlyCollection<int> idList,
        CancellationToken cancellationToken = default
    )
    {
        var batch = db.CreateBatch();
        var memberTask = new Dictionary<int, Task<RedisValue[]>>();

        foreach (var userId in idList)
        {
            memberTask[userId] = batch.SetMembersAsync(UserTokensKey(userId));
        }
        batch.Execute();
        await Task.WhenAll(memberTask.Values);

        var keysToDelete = new List<RedisKey>();

        foreach (var (userId, task) in memberTask)
        {
            keysToDelete.Add(UserTokensKey(userId));

            var tokens = await task;
            foreach (var token in tokens)
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
