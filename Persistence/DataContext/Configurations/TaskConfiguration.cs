using Domain.Constants;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Base;

namespace Persistence.DataContext.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<WorkTask>
{
    private const string Base = $"TASK";

    private const string IdColumn = $"{Base}_ID";
    private const string TitleColumn = $"{Base}_TITLE";
    private const string PriorityColumn = $"{Base}_PRIORITY";
    private const string StatusColumn = $"{Base}_STATUS";
    private const string CommentColumn = $"{Base}_COMMENT";
    private const string CreatedAtColumn = $"{Base}_CREATED_AT";
    private const string UpdatedAtColumn = $"{Base}_UPDATED_AT";

    private const string Author = $"AUTHOR";
    private const string AuthorIdColumn = $"{Base}_{Author}_ID";
    private const string AuthorFk = $"FK_{Base}S_{Author}";

    private const string Executor = $"EXECUTOR";
    private const string ExecutorIdColumn = $"{Base}_{Executor}_ID";
    private const string ExecutorFk = $"FK_{Base}S_{Executor}";

    private const string Project = $"PROJECT";
    private const string ProjectIdColumn = $"{Base}_{Project}_ID";
    private const string ProjectFk = $"FK_{Base}S_{Project}S";

    public void Configure(EntityTypeBuilder<WorkTask> builder)
    {
        builder.ToTable($"{Base}S");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName(IdColumn)
            .HasColumnType(SqlTypes.Integer())
            .ValueGeneratedOnAdd();

        builder
            .Property(e => e.Title)
            .HasColumnName(TitleColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.WorkTask.TitleMaxLength)
            .IsRequired();

        builder
            .Property(e => e.Priority)
            .HasColumnName(PriorityColumn)
            .HasColumnType(SqlTypes.Integer())
            .IsRequired();

        builder
            .Property(e => e.Status)
            .HasColumnName(StatusColumn)
            .HasColumnType(SqlTypes.Integer())
            .IsRequired();

        builder
            .Property(e => e.Comment)
            .HasColumnName(CommentColumn)
            .HasColumnType(SqlTypes.Text())
            .HasMaxLength(FieldLimits.WorkTask.CommentMaxLength);

        builder
            .Property(e => e.CreatedAt)
            .HasColumnName(CreatedAtColumn)
            .HasColumnType(SqlTypes.Text())
            .IsRequired();

        builder
            .Property(e => e.UpdatedAt)
            .HasColumnName(UpdatedAtColumn)
            .HasColumnType(SqlTypes.Text());

        builder
            .Property(e => e.AuthorId)
            .HasColumnName(AuthorIdColumn)
            .HasColumnType(SqlTypes.Integer());
        builder
            .HasOne(t => t.Author)
            .WithMany(e => e.AuthoredTasks)
            .HasForeignKey(t => t.AuthorId)
            .HasConstraintName(AuthorFk)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property(e => e.ExecutorId)
            .HasColumnName(ExecutorIdColumn)
            .HasColumnType(SqlTypes.Integer());
        builder
            .HasOne(t => t.Executor)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(t => t.ExecutorId)
            .HasConstraintName(ExecutorFk)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .Property(t => t.ProjectId)
            .HasColumnName(ProjectIdColumn)
            .HasColumnType(SqlTypes.Integer());

        builder
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(ProjectFk);

        builder.HasIndex(t => t.AuthorId);
        builder.HasIndex(t => t.ExecutorId);
        builder.HasIndex(t => t.Status);
        builder.HasIndex(t => t.Priority);
    }
}
