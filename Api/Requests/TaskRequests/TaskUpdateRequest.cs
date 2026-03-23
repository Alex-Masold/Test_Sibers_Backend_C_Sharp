using System.ComponentModel.DataAnnotations;
using Api.ValidateAttributes;
using Application.Contracts.TaskContracts;
using Domain.Models;

namespace Api.Requests.TaskRequests;

public class TaskUpdateRequest
{
    public string? Title { get; init; }
    
    [Range(1, 5, ErrorMessage = "The priority must be between 1 and 5")]
    public int? Priority { get; init; }

    [InEnum(typeof(WorkTaskStatus), ErrorMessage = "The value is not included in the enumeration")]
    public WorkTaskStatus? Status { get; init; }
    public string? Comment { get; init; }
    public int? ExecutorId { get; init; }
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
