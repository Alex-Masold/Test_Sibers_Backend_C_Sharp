using Domain.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedisService.Repositories;
using StackExchange.Redis;

namespace RedisService;

public static class RedisConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var connection =
            configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is not configured");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            try
            {
                return ConnectionMultiplexer.Connect(connection);
            }
            catch (RedisConnectionException ex)
            {
                throw new InvalidOperationException($"Cannot connect to Redis: {connection}", ex);
            }
        });

        services.AddScoped<IRefreshTokenStore, RefreshTokenRepository>();
    }
}
