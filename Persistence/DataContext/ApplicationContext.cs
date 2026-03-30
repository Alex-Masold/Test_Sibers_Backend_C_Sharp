using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.DataContext.Configurations;

namespace Persistence.DataContext;

public class ApplicationContext(DbContextOptions<ApplicationContext> options)
    : DbContext(options),
        IUnitOfWork
{
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<WorkTask> Tasks { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<ProjectDocument> ProjectDocuments { get; set; } = null!;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectMemberConfiguration());
        modelBuilder.ApplyConfiguration(new ProjectDocumentConfiguration());
    }
}
