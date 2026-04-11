using Application.Contracts.TaskContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Models;

namespace Application.Validators.TaskValidators;

public class TaskAccessValidator(ICurrentUserService userService) : ITaskAccessValidator
{
    public void EnsureCreatePermission(Project project)
    {
        if (userService.IsDirector)
            return;

        if (userService.Role == Role.Worker)
            throw new AccessDeniedException("Worker cannot create tasks");

        if (project.ManagerId != userService.UserId)
            throw new AccessDeniedException("Managers can only create tasks in their own projects");
    }

    public void EnsureReadPermission(WorkTask task)
    {
        if (!userService.IsDirector)
        {
            if (userService.Role == Role.Manager)
            {
                if (task.Project is null)
                    throw new InvalidOperationException("Task Project must be loaded");
                if (task.Project.ManagerId != userService.UserId)
                    throw new AccessDeniedException(
                        "you do not have permission to view task in this project"
                    );
            }

            if (userService.Role == Role.Worker)
            {
                if (task.ExecutorId != userService.UserId)
                    throw new AccessDeniedException("You can only view your own tasks");
            }
        }
    }

    public void EnsureUpdatePermission(WorkTask task, TaskUpdateDto dto)
    {
        if (!userService.IsDirector)
        {
            if (userService.Role == Role.Manager)
            {
                if (task.Project is null)
                    throw new InvalidOperationException("Task Project must be loaded");
                if (task.Project.ManagerId != userService.UserId)
                    throw new AccessDeniedException(
                        "Managers can only edit tasks in their own projects"
                    );
            }
            if (userService.Role == Role.Worker)
            {
                if (task.ExecutorId != userService.UserId)
                    throw new AccessDeniedException("Workers can only edit their own tasks");

                bool isChangingOnlyStatus =
                    dto.Title == null
                    && dto.Priority == null
                    && !dto.Comment.HasValue
                    && !dto.ExecutorId.HasValue
                    && dto.ProjectId == null;

                if (!isChangingOnlyStatus)
                {
                    throw new AccessDeniedException("Workers can only change the task status");
                }
            }
        }
    }

    public void EnsureDeletePermission(WorkTask task)
    {
        if (userService.IsDirector)
            return;

        if (userService.Role == Role.Manager)
        {
            if (task.Project is null)
                throw new InvalidOperationException("Task Project must be loaded");
            if (task.Project?.ManagerId != userService.UserId)
                throw new AccessDeniedException(
                    "Managers cannot delete tasks from other managers' projects"
                );
            return;
        }
        if (userService.Role == Role.Worker)
            throw new AccessDeniedException("Worker cannot delete tasks");
    }
}
