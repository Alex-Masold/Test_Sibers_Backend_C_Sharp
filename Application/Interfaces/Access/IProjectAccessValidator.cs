using Application.Contracts.ProjectContracts;
using Domain.Models;

namespace Application.Interfaces.Access;

public interface IProjectAccessValidator
{
    Task EnsureReadPermission(Project project, CancellationToken cancellationToken = default);
    void EnsureCreatePermission();
    void EnsureUpdatePermission(Project project, ProjectUpdateDto? dto = null);
    void EnsureDeletePermission();
}
