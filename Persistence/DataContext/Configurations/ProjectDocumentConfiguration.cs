using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Base;

namespace Persistence.DataContext.Configurations;

public class ProjectDocumentConfiguration: IEntityTypeConfiguration<ProjectDocument>
{
    private const string Base = $"PROJECT_DOCUMENT";
    
    private const string Project = $"PROJECT";
    private const string ProjectFk = $"FK_{Project}S_DOCUMENTS";

    private const string IdColumn = $"{Base}_ID";
    
    private const string OriginalFileNameColumn = $"{Base}_ORIGINAL_FILE_NAME";
    private const string StoredFileNameColumn = $"{Base}_STORED_FILE_NAME";
    private const string ContentTypeColumn = $"{Base}_CONTENT_TYPE";

    private const string ProjectIdColumn = $"{Project}_ID";
    
    public void Configure(EntityTypeBuilder<ProjectDocument> builder)
    {
        builder.ToTable($"{Base}S");

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasColumnName(IdColumn)
            .HasColumnType(SqlTypes.Integer())
            .ValueGeneratedOnAdd();

        builder
            .Property(e => e.OriginalFileName)
            .HasColumnName(OriginalFileNameColumn)
            .HasColumnType(SqlTypes.Text())
            .IsRequired();

        builder
            .Property(e => e.StoredFileName)
            .HasColumnName(StoredFileNameColumn)
            .HasColumnType(SqlTypes.Text())
            .IsRequired();

        builder
            .Property(e => e.ContentType)
            .HasColumnName(ContentTypeColumn)
            .HasColumnType(SqlTypes.Text())
            .IsRequired();
        
        builder
            .Property(e => e.ProjectId)
            .HasColumnName(ProjectIdColumn)
            .HasColumnType(SqlTypes.Integer())
            .IsRequired();
        builder
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Documents)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName(ProjectFk);
    }
}