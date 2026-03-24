using Domain.Models;

namespace Application.Interfaces.Access;

public interface IProjectMemberAccessValidator
{
    void EnsureCreatePermission(Project project);
    void EnsureReadPermission(Project project);
    void EnsureUpdatePermission(Project project);
    void EnsureDeletePermission(Project project);
}
