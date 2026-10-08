using Application.Contracts.TaskContracts;
using Domain.Models;

namespace Application.Interfaces.Access;

public interface ITaskAccessValidator
{
    void EnsureCreatePermission(Project project);
    Task EnsureReadPermission(WorkTask task, CancellationToken ct = default);
    void EnsureUpdatePermission(WorkTask task, TaskUpdateDto dto);
    void EnsureDeletePermission(WorkTask task);
}
