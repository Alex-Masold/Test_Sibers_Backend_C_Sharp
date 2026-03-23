using Api.Configurations;
using Api.Middlewares;
using Application;
using CurrentUserService;
using FileService;
using Microsoft.EntityFrameworkCore;
using Persistence;
using RedisService;
using Scalar.AspNetCore;
using TokenService;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

ApplicationConfiguration.Configure(services);
CurrentUserServiceConfiguration.Configure(services);
FileServiceConfiguration.Configure(services);
JwtConfiguration.Configure(services, configuration);
RedisConfiguration.Configure(services, configuration);
PersistenceConfiguration.Configure(services, configuration);

ControllersConfiguration.Configure(services);
OpenApiConfiguration.Configure(services);
CorsConfiguration.Configure(services, configuration);

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

        Persistence.Data.DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();
