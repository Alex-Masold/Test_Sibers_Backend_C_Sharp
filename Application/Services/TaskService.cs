using Application.Contracts;
using Application.Contracts.TaskContracts;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Filters;
using Domain.Interfaces;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class TaskService(
    ITaskStore taskStore,
    IProjectStore projectStore,
    ICurrentUserService userService,
    ITaskAccessValidator accessValidator,
    IValidator<TaskCreateDto> createValidator,
    IValidator<TaskUpdateDto> updateValidator,
    IValidator<PagedDto> pagedValidator,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider
)
{
    public async Task<TaskReadDto> GetTaskByIdAsync(int taskId, CancellationToken ct = default)
    {
        var task = await taskStore.GetOrThrowAsync(taskId, ct);

        accessValidator.EnsureReadPermission(task);

        return TaskReadDto.From(task);
    }

    public async Task<(IReadOnlyCollection<TaskReadDto> Items, int TotalCount)> GetTasksAsync(
        PagedDto pagedDto,
        TaskFilter? filter = null,
        SortOptions<TaskSortField>? options = null,
        CancellationToken ct = default
    )
    {
        var validationResult = await pagedValidator.ValidateAsync(pagedDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        filter ??= new TaskFilter();

        if (!userService.IsDirector)
        {
            if (userService.Role == Role.Manager)
                filter = filter with { ProjectManagerId = userService.UserId };
            else if (userService.Role == Role.Worker)
                filter = filter with { ExecutorId = userService.UserId };
        }

        var result = await taskStore.GetPagedAsync<TaskReadDto>(
            pagedDto.PageNumber,
            pagedDto.PageSize,
            TaskReadDto.Projection,
            filter,
            options,
            ct
        );

        return result;
    }

    public async Task<TaskReadDto> CreateTaskAsync(
        TaskCreateDto dto,
        CancellationToken ct = default
    )
    {
        var validationResult = await createValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var project = await projectStore.GetOrThrowAsync(dto.ProjectId, ct);
        accessValidator.EnsureCreatePermission(project);

        var task = dto.ToEntity();
        task.CreatedAt = timeProvider.GetUtcNow();
        task.AuthorId = userService.UserId;

        var createdTasksId = taskStore.Create(task).Id;
        await unitOfWork.SaveChangesAsync(ct);

        var createdTask = await taskStore.GetOrThrowAsync(createdTasksId, ct);

        return TaskReadDto.From(createdTask);
    }

    public async Task<TaskReadDto> UpdateTaskAsync(
        int taskId,
        TaskUpdateDto dto,
        CancellationToken ct = default
    )
    {
        var task = await taskStore.GetOrThrowAsync(taskId, ct);

        accessValidator.EnsureUpdatePermission(task, dto);

        var validationContext = new ValidationContext<TaskUpdateDto>(dto);
        validationContext.RootContextData["ExistingTask"] = task;

        var validationResult = await updateValidator.ValidateAsync(validationContext, ct);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (dto.ApplyTo(task))
        {
            task.UpdatedAt = timeProvider.GetUtcNow();
            await unitOfWork.SaveChangesAsync(ct);
        }

        return TaskReadDto.From(task);
    }

    public async Task<int> DeleteTaskAsync(int taskId, CancellationToken ct = default)
    {
        var task = await taskStore.GetOrThrowAsync(taskId, ct);

        accessValidator.EnsureDeletePermission(task);

        return await taskStore.DeleteAsync(taskId, ct);
    }

    public async Task<int> DeleteTasksAsync(
        IReadOnlyCollection<int> taskIdList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = taskIdList.Distinct().ToList();

        var tasks = await taskStore.GetOrThrowAsync(distinctIdList, ct);
        if (userService.IsDirector)
            return await taskStore.DeleteAsync(distinctIdList, ct);

        foreach (var task in tasks)
        {
            accessValidator.EnsureDeletePermission(task);
        }

        return await taskStore.DeleteAsync(distinctIdList, ct);
    }
}
