using Domain.Constants;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Base;

namespace Persistence.DataContext.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    private const string Base = $"PROJECT";

    private const string IdColumn = $"{Base}_ID";
    private const string NameColumn = $"{Base}_NAME";
    private const string PriorityColumn = $"{Base}_PRIORITY";
    private const string CompanyOrderingNameColumn = $"{Base}_COMPANY_ORDERING_NAME";
    private const string CompanyExecutingNameColumn = $"{Base}_COMPANY_EXECUTING_NAME";

    private const string StartDateColumn = $"{Base}_START_DATE";
    private const string EndDateColumn = $"{Base}_END_DATE";

    private const string Employee = $"EMPLOYEE";
    private const string ManagerIdColumn = $"{Base}_MANAGER_ID";
    private const string EmployeeFk = $"FK_{Base}S_{Employee}S";

    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable($"{Base}S");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName(IdColumn)
            .HasColumnType(SqlTypes.Integer())
            .ValueGeneratedOnAdd();

        builder
            .Property(e => e.Name)
            .HasColumnName(NameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Project.NameMaxLength)
            .IsRequired();

        builder
            .Property(e => e.Priority)
            .HasColumnName(PriorityColumn)
            .HasColumnType(SqlTypes.Integer())
            .IsRequired();

        builder
            .Property(e => e.CompanyOrdering)
            .HasColumnName(CompanyOrderingNameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Project.CompanyNameMaxLength)
            .IsRequired();

        builder
            .Property(e => e.CompanyExecuting)
            .HasColumnName(CompanyExecutingNameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Project.CompanyNameMaxLength);

        builder
            .Property(e => e.StartDate)
            .HasColumnName(StartDateColumn)
            .HasColumnType(SqlTypes.Text())
            .IsRequired();

        builder
            .Property(e => e.EndDate)
            .HasColumnName(EndDateColumn)
            .HasColumnType(SqlTypes.Text());

        builder
            .Property(e => e.ManagerId)
            .HasColumnName(ManagerIdColumn)
            .HasColumnType(SqlTypes.Integer());
        builder
            .HasOne(p => p.Manager)
            .WithMany(e => e.ManagedProjects)
            .HasForeignKey(p => p.ManagerId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName(EmployeeFk);
    }
}
