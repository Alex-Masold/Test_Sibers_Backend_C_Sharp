using Application.Contracts.TaskContracts;
using Domain.Models;

namespace Application.Interfaces.Access;

public interface ITaskAccessValidator
{
    void EnsureCreatePermission(Project project);
    void EnsureReadPermission(WorkTask task);
    void EnsureUpdatePermission(WorkTask task, TaskUpdateDto dto);
    void EnsureDeletePermission(WorkTask task);
}
