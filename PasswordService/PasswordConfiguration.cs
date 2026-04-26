using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PasswordService;

public static class PasswordConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton<IPasswordService, IdentityPasswordService>();
    }
}
