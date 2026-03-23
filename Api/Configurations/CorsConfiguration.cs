namespace Api.Configurations;

public abstract class CorsConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var clientSection = configuration.GetSection("Client");
        var clientUrl = clientSection.Get<ClientUrl>();

        if (clientUrl == null || string.IsNullOrEmpty(clientUrl.WebVue))
        {
            throw new InvalidOperationException(
                "Секция 'Client:WebVue' не найдена в appsettings.json!"
            );
        }
        services.Configure<ClientUrl>(clientSection);
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
