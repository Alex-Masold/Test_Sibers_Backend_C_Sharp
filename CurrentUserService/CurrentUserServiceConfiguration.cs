using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CurrentUserService;

public static class CurrentUserServiceConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, UserService>();
    }
}
