using Domain.Base;

namespace Domain.Models;

public class ProjectDocument : Entity
{
    public required string OriginalFileName { get; set; }
    public required string StoredFileName { get; set; }
    public required string ContentType { get; set; }

    public required int ProjectId { get; set; }
    public Project Project { get; init; } = null!;
}
