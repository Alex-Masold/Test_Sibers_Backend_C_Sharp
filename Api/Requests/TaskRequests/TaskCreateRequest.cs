using Application.Contracts.TaskContracts;
using Domain.Models;

namespace Api.Requests.TaskRequests;

public class TaskCreateRequest
{
    public required string Title { get; init; }

    public int Priority { get; init; }

    public WorkTaskStatus Status { get; init; }

    public string? Comment { get; init; }

    public int? ExecutorId { get; init; }

    public required int ProjectId { get; init; }

    public TaskCreateDto ToDto() =>
        new()
        {
            Title = Title,
            Status = Status,
            Priority = Priority,
            Comment = Comment,

            ExecutorId = ExecutorId,
            ProjectId = ProjectId,
        };
}
