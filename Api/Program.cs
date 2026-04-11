using Api.Configurations;
using Api.Middlewares;
using Application;
using CurrentUserService;
using DotNetEnv;
using FileService;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Data;
using RedisService;
using Scalar.AspNetCore;
using TokenService;

if (File.Exists(".env"))
{
    Env.Load();
}

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

ApplicationConfiguration.Configure(services);
CurrentUserServiceConfiguration.Configure(services);
FileServiceConfiguration.Configure(services);
JwtConfiguration.Configure(services, configuration, builder.Environment.IsDevelopment());
RedisConfiguration.Configure(services, configuration);
PersistenceConfiguration.Configure(services, configuration);

ControllersConfiguration.Configure(services);
OpenApiConfiguration.Configure(services);
CorsConfiguration.Configure(services, configuration);

services.AddSingleton(TimeProvider.System);

builder
    .Configuration.AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    try
    {
        var context =
            serviceProvider.GetRequiredService<Persistence.DataContext.ApplicationContext>();

        context.Database.Migrate();
        if (app.Environment.IsDevelopment())
        {
            DbSeeder.Seed(context, app.Services.GetRequiredService<TimeProvider>());
        }
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

app.Run();
