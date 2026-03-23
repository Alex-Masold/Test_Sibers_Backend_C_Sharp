using System.ComponentModel.DataAnnotations;
using Api.ValidateAttributes;
using Application.Contracts.TaskContracts;
using Domain.Models;

namespace Api.Requests.TaskRequests;

public class TaskCreateRequest
{
    public required string Title { get; init; }

    [Range(1, 5, ErrorMessage = "The priority must be between 1 and 5")]
    public int Priority { get; init; }

    [InEnum(typeof(WorkTaskStatus), ErrorMessage = "The value is not included in the enumeration")]
    public WorkTaskStatus Status { get; init; }

    public string? Comment { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "The ExecutorId must not is less than 0")]
    public int? ExecutorId { get; init; }

    [Range(0, int.MaxValue, ErrorMessage = "The ProjectId must not is less than 0")]
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
