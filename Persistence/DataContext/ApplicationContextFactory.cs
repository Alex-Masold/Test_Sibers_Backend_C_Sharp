using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistence.DataContext;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var config = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        var sqliteConnStr =
            config.GetConnectionString("Sqlite")
            ?? throw new InvalidOperationException("Connection string not found");

        optionsBuilder.UseSqlite(
            sqliteConnStr,
            b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)
        );

        return new ApplicationContext(optionsBuilder.Options);
    }
}