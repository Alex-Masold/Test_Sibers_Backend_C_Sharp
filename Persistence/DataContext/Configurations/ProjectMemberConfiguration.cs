using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Base;

namespace Persistence.DataContext.Configurations;

public class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    private const string Base = $"PROJECT_MEMBER";

    private const string Project = $"PROJECT";
    private const string ProjectFk = $"FK_{Project}S_MEMBERS";

    private const string Employee = $"EMPLOYEE";
    private const string EmployeeFk = $"FK_{Employee}S_MEMBERS";

    private const string ProjectIdColumn = $"{Project}_ID";
    private const string EmployeeIdColumn = $"{Employee}_ID";

    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable($"{Base}S");

        builder.HasKey(e => new { e.ProjectId, e.EmployeeId });

        builder
            .Property(e => e.ProjectId)
            .HasColumnName(ProjectIdColumn)
            .HasColumnType(SqlTypes.Integer)
            .IsRequired();
        builder
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(ProjectFk);

        builder
            .Property(e => e.EmployeeId)
            .HasColumnName(EmployeeIdColumn)
            .HasColumnType(SqlTypes.Integer)
            .IsRequired();
        builder
            .HasOne(pm => pm.Employee)
            .WithMany(e => e.Memberships)
            .HasForeignKey(pm => pm.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(EmployeeFk);
    }
}
