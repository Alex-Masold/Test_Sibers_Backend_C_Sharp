using Microsoft.OpenApi.Models;

namespace Api.Configurations;

public static class OpenApiConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer(
                (document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo()
                    {
                        Title = "Sibers Test API",
                        Version = "v1",
                        Description = "API для управления проектами",
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes.Add(
                        "Bearer",
                        new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description = "Введите ваш Access Token",
                        }
                    );

                    document.SecurityRequirements.Add(
                        new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer",
                                    },
                                },
                                Array.Empty<string>()
                            },
                        }
                    );

                    return Task.CompletedTask;
                }
            );
        });
        services.AddEndpointsApiExplorer();
    }
}
