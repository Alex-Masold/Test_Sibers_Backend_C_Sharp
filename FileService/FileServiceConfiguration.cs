using Application.Interfaces;
using FileService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FileService;

public static class FileServiceConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services.AddSingleton<IFileService, LocalFileService>();
    }
}
