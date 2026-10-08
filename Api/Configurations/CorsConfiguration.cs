using Api.Models;
using Application.Settings;

namespace Api.Configurations;

public static class CorsConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var clientSection = configuration.GetSection("Client");
        var clientUrl = clientSection.Get<ClientSettings>();

        if (clientUrl == null || string.IsNullOrEmpty(clientUrl.WebVue))
        {
            throw new InvalidOperationException(
                "The Section 'Client:WebVue' not found in appSettings.json!"
            );
        }
        services.Configure<ClientSettings>(clientSection);
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .WithOrigins(clientUrl.WebVue)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
