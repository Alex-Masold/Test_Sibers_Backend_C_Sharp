using Application.Contracts.TaskContracts;
using Domain.Common;
using Domain.Models;

namespace Api.Requests.TaskRequests;

public class TaskUpdateRequest
{
    public string? Title { get; init; }

    public int? Priority { get; init; }

    public WorkTaskStatus? Status { get; init; }
    public Optional<string?> Comment { get; init; }
    public Optional<int?> ExecutorId { get; init; }
    public int? ProjectId { get; init; }

    public TaskUpdateDto ToDto() =>
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
