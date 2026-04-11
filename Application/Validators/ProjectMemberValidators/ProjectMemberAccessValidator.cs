using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Models;

namespace Application.Validators.ProjectMemberValidators;

public class ProjectMemberAccessValidator(ICurrentUserService userService)
    : IProjectMemberAccessValidator
{
    private void ManagePermission(Project project)
    {
        if (userService.IsDirector)
        {
            return;
        }
        if (userService.Role == Role.Worker)
        {
            throw new AccessDeniedException(
                "Workers do not have permission to manage project members"
            );
        }

        if (userService.Role == Role.Manager)
        {
            if (project.ManagerId != userService.UserId)
            {
                throw new AccessDeniedException(
                    "Only the project manager can manage members of this project"
                );
            }
        }
    }

    public void EnsureCreatePermission(Project project)
    {
        ManagePermission(project);
    }

    public void EnsureReadPermission(Project project)
    {
        ManagePermission(project);
    }

    public void EnsureUpdatePermission(Project project)
    {
        ManagePermission(project);
    }

    public void EnsureDeletePermission(Project project)
    {
        ManagePermission(project);
    }
}
