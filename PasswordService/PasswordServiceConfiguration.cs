using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PasswordService;

public static class PasswordServiceConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddScoped<IPasswordService, IdentityPasswordService>();
    }
}
