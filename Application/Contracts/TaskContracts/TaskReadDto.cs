using System.Linq.Expressions;
using Application.Contracts.Base;
using Application.Contracts.EmployeeContracts;
using Application.Contracts.ProjectContracts;
using Domain.Models;

namespace Application.Contracts.TaskContracts;

public record TaskReadDto : IReadDto<WorkTask, TaskReadDto>
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public WorkTaskStatus Status { get; init; }
    public string? Comment { get; init; }
    public int Priority { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public EmployeeShortReadDto? Author { get; init; }
    public EmployeeShortReadDto? Executor { get; init; }
    public required ProjectShortReadDto Project { get; init; }

    public static Expression<Func<WorkTask, TaskReadDto>> Projection =>
        task =>
            new()
            {
                Id = task.Id,
                Title = task.Title,
                Status = task.Status,
                Comment = task.Comment,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,

                Author =
                    task.AuthorId != null
                        ? new EmployeeShortReadDto
                        {
                            Id = task.AuthorId.Value,
                            FirstName = task.Author != null ? task.Author.FirstName : "",
                            MiddleName = task.Author != null ? task.Author.MiddleName : "",
                            LastName = task.Author != null ? task.Author.LastName : "",
                        }
                        : null,
                Executor =
                    task.ExecutorId != null
                        ? new EmployeeShortReadDto
                        {
                            Id = task.ExecutorId.Value,
                            FirstName = task.Executor != null ? task.Executor.FirstName : "",
                            MiddleName = task.Executor != null ? task.Executor.MiddleName : "",
                            LastName = task.Executor != null ? task.Executor.LastName : "",
                        }
                        : null,
                Project = new ProjectShortReadDto { Id = task.ProjectId, Name = task.Project.Name },
            };

    public static TaskReadDto From(WorkTask task) =>
        new()
        {
            Id = task.Id,

            Title = task.Title,
            Status = task.Status,
            Comment = task.Comment,
            Priority = task.Priority,

            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,

            Author =
                task.AuthorId != null
                    ? new EmployeeShortReadDto
                    {
                        Id = task.AuthorId.Value,
                        FirstName = task.Author != null ? task.Author.FirstName : "",
                        MiddleName = task.Author != null ? task.Author.MiddleName : "",
                        LastName = task.Author != null ? task.Author.LastName : "",
                    }
                    : null,
            Executor =
                task.ExecutorId != null
                    ? new EmployeeShortReadDto
                    {
                        Id = task.ExecutorId.Value,
                        FirstName = task.Executor != null ? task.Executor.FirstName : "",
                        MiddleName = task.Executor != null ? task.Executor.MiddleName : "",
                        LastName = task.Executor != null ? task.Executor.LastName : "",
                    }
                    : null,
            Project = new ProjectShortReadDto { Id = task.ProjectId, Name = task.Project.Name },
        };
}
