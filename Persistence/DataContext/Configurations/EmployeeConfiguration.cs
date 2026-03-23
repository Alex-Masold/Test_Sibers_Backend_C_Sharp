using Domain.Constants;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Base;

namespace Persistence.DataContext.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    private const string Base = $"EMPLOYEE";

    private const string IdColumn = $"{Base}_ID";
    private const string FirstNameColumn = $"{Base}_FIRST_NAME";
    private const string MiddleNameColumn = $"{Base}_MIDDLE_NAME";
    private const string LastNameColumn = $"{Base}_LAST_NAME";
    private const string EmailColumn = $"{Base}_EMAIL";

    private const string RoleColumn = $"{Base}_ROLE";

    private const string EmailUniqueIndex = $"UX_{Base}S_{EmailColumn}";

    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable($"{Base}S");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName(IdColumn)
            .HasColumnType(SqlTypes.Integer())
            .ValueGeneratedOnAdd();

        builder
            .Property(e => e.FirstName)
            .HasColumnName(FirstNameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Employee.FirstNameMaxLength)
            .IsRequired();

        builder
            .Property(e => e.MiddleName)
            .HasColumnName(MiddleNameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Employee.MiddleNameMaxLength);

        builder
            .Property(e => e.LastName)
            .HasColumnName(LastNameColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Employee.LastNameMaxLength)
            .IsRequired();

        builder
            .Property(e => e.Email)
            .HasColumnName(EmailColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.Employee.EmailMaxLength)
            .IsRequired();

        builder
            .Property(e => e.Role)
            .HasColumnName(RoleColumn)
            .HasColumnType(SqlTypes.Integer())
            .IsRequired();

        builder.HasIndex(e => e.Email).IsUnique().HasDatabaseName(EmailUniqueIndex);
    }
}
