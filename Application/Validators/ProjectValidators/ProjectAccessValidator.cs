using Application.Contracts.ProjectContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;

namespace Application.Validators.ProjectValidators;

public class ProjectAccessValidator(
    ICurrentUserService userService,
    IProjectMemberStore memberStore
) : IProjectAccessValidator
{
    public void EnsureCreatePermission()
    {
        if (!userService.IsDirector)
            throw new AccessDeniedException("Only director can create project");
    }

    public void EnsureDeletePermission()
    {
        if (userService.IsDirector)
            return;

        throw new AccessDeniedException("Only a director can delete project");
    }

    public async Task EnsureReadPermission(Project project, CancellationToken ct = default)
    {
        if (userService.IsDirector)
            return;
        if (project.ManagerId == userService.UserId)
            return;

        bool isMember = await memberStore.MemberExistsAsync(project.Id, userService.UserId, ct);

        if (!isMember)
        {
            throw new AccessDeniedException("You do not have permission to view this project");
        }
    }

    public void EnsureUpdatePermission(Project project, ProjectUpdateDto? dto = null)
    {
        if (userService.IsDirector)
            return;

        if (userService.Role == Role.Worker)
            throw new AccessDeniedException("Workers cannot edit projects");

        if (userService.Role == Role.Manager)
        {
            if (project.ManagerId != userService.UserId)
                throw new AccessDeniedException("Managers can only edit their own projects");
            if (dto != null && dto.ManagerId.HasValue && dto.ManagerId.Value != project.ManagerId)
                throw new AccessDeniedException("Only a director can reassign a project manager");
        }
    }
}
