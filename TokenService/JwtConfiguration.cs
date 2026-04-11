using System.Text;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TokenService.Settings;

namespace TokenService;

public static class JwtConfiguration
{
    public static void Configure(
        IServiceCollection services,
        IConfiguration configuration,
        bool isDev
    )
    {
        var jwtSettingsSection = configuration.GetSection("JwtSettings");
        var jwtSettings =
            jwtSettingsSection.Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is not configured");

        if (Encoding.UTF8.GetByteCount(jwtSettings.SecretKey) < 32)
            throw new InvalidOperationException("JWT secret key must be at least 32 bytes");
        if (jwtSettings.Expires <= TimeSpan.Zero)
            throw new InvalidOperationException(
                "JwtSettings:Expires must be a positive duration"
            );

        services.Configure<JwtSettings>(jwtSettingsSection);
        services.AddSingleton<ITokenService, Services.JwtTokenService>();

        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                opt.RequireHttpsMetadata = !isDev;
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = !isDev,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = !isDev,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
                    ),
                };
            });
    }
}
