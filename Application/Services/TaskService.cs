using Application.Contracts;
using Application.Contracts.TaskContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
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
    IUnitOfWork unitOfWork
)
{
    private async Task<WorkTask> GetTask(int taskId, CancellationToken ct = default)
    {
        var task = await taskStore.GetByIdAsync(taskId, ct);
        if (task is null)
            throw new NotFoundException(nameof(WorkTask), taskId);
        return task;
    }

    private async Task<IReadOnlyCollection<WorkTask>> GetTasks(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();
        var existingTasks = await taskStore.GetRangeByIdsAsync(distinctIdList, ct);

        if (existingTasks.Count != distinctIdList.Count())
        {
            var existingIds = existingTasks.Select(t => t.Id).ToList();
            var nonExistingIds = distinctIdList.Where(id => !existingIds.Contains(id)).ToList();
            throw new NotFoundException(nameof(WorkTask), nonExistingIds);
        }
        return existingTasks;
    }

    private async Task<Project> GetProject(int projectId, CancellationToken ct = default)
    {
        var project = await projectStore.GetByIdAsync(projectId, ct);
        if (project == null)
            throw new NotFoundException(nameof(Project), projectId);
        return project;
    }

    public async Task<TaskReadDto> GetTaskByIdAsync(int taskId, CancellationToken ct = default)
    {
        var task = await GetTask(taskId, ct);
        await taskStore.LoadProjectAsync(task, ct);

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

        var project = await GetProject(dto.ProjectId, ct);
        accessValidator.EnsureCreatePermission(project);

        var task = dto.ToEntity();
        task.AuthorId = userService.UserId;

        var createdTask = taskStore.Create(task);
        await unitOfWork.SaveChangesAsync(ct);
        await taskStore.LoadProjectAsync(createdTask, ct);
        await taskStore.LoadAuthorAsync(createdTask, ct);
        await taskStore.LoadExecutorAsync(createdTask, ct);

        return TaskReadDto.From(createdTask);
    }

    public async Task<TaskReadDto> UpdateTaskAsync(
        int taskId,
        TaskUpdateDto dto,
        CancellationToken ct = default
    )
    {
        var task = await GetTask(taskId, ct);
        await taskStore.LoadProjectAsync(task, ct);
        await taskStore.LoadAuthorAsync(task, ct);
        await taskStore.LoadExecutorAsync(task, ct);

        accessValidator.EnsureUpdatePermission(task, dto);

        var validationContext = new ValidationContext<TaskUpdateDto>(dto);
        validationContext.RootContextData["ExistingTask"] = task;

        var validationResult = await updateValidator.ValidateAsync(validationContext, ct);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (dto.ApplyTo(task))
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return TaskReadDto.From(task);
    }

    public async Task<int> DeleteTaskAsync(int taskId, CancellationToken ct = default)
    {
        var task = await GetTask(taskId, ct);

        if (userService.IsDirector)
            return await taskStore.DeleteAsync(taskId, ct);

        await taskStore.LoadProjectAsync(task, ct);

        accessValidator.EnsureDeletePermission(task);

        return await taskStore.DeleteAsync(taskId, ct);
    }

    public async Task<int> DeleteTasksByIdsAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();

        var tasks = await GetTasks(distinctIdList, ct);
        if (userService.IsDirector)
            return await taskStore.DeleteAsync(distinctIdList, ct);

        foreach (var task in tasks)
        {
            accessValidator.EnsureDeletePermission(task);
        }

        return await taskStore.DeleteAsync(distinctIdList, ct);
    }
}
