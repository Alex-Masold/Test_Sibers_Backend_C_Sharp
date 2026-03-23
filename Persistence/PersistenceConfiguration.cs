using Domain.Interfaces;
using Domain.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DataContext;
using Persistence.Repositories;

namespace Persistence;

public static class PersistenceConfiguration
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Sqlite");

        services.AddScoped<IEmployeeStore, EmployeeRepository>();
        services.AddScoped<IProjectStore, ProjectRepository>();
        services.AddScoped<IProjectDocumentStore, ProjectDocumentRepository>();
        services.AddScoped<IProjectMemberStore, ProjectMemberRepository>();
        services.AddScoped<ITaskStore, TaskRepository>();

        services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationContext>());
    }
}
