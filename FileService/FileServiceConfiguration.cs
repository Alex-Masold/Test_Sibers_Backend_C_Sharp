using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileService;

public static class FileServiceConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddScoped<IFileService, LocalFileService>();
    }
}

