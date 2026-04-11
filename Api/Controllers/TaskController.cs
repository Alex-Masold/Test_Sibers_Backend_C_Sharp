using Api.Models;
using Api.Requests.PaginationRequests;
using Api.Requests.TaskRequests;
using Application.Contracts.TaskContracts;
using Application.Parsers;
using Application.Services;
using Domain.Filters;
using Domain.Sort;
using Domain.Sort.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/tasks")]
public class TaskController(TaskService service) : ControllerBase
{
    [HttpGet("{taskId:int}", Name = "GetTask")]
    public async Task<ActionResult<TaskReadDto>> GetTask(
        [FromRoute] int taskId,
        CancellationToken ct = default
    )
    {
        var task = await service.GetTaskByIdAsync(taskId, ct);

        return Ok(task);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskReadDto>>> GetTasks(
        [FromQuery] PaginationRequest request,
        [FromQuery] TaskFilter? filter = null,
        [FromQuery] string? sortQuery = null,
        CancellationToken ct = default
    )
    {
        var options = new SortOptions<TaskSortField>()
        {
            Items = SortParser<TaskSortField>.Parse(sortQuery),
        };

        var result = await service.GetTasksAsync(request.ToDto(), filter, options, ct);

        return Ok(
            new PagedResponse<TaskReadDto>()
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            }
        );
    }

    [HttpPost]
    [Authorize(Roles = "Director, Manager")]
    public async Task<ActionResult<TaskReadDto>> CreateTask(
        [FromBody] TaskCreateRequest request,
        CancellationToken ct = default
    )
    {
        var createdDto = request.ToDto();
        var createdTask = await service.CreateTaskAsync(createdDto, ct);

        return CreatedAtAction(nameof(GetTask), new { taskId = createdTask.Id }, createdTask);
    }

    [HttpPatch("{taskId:int}")]
    public async Task<ActionResult<TaskReadDto>> UpdateTask(
        [FromRoute] int taskId,
        [FromBody] TaskUpdateRequest request,
        CancellationToken ct = default
    )
    {
        var updatedDto = request.ToDto();
        var updatedTask = await service.UpdateTaskAsync(taskId, updatedDto, ct);

        return Ok(updatedTask);
    }

    [HttpDelete("{taskId:int}")]
    [Authorize(Roles = "Director, Manager")]
    public async Task<ActionResult> DeleteTask([FromRoute] int taskId, CancellationToken ct = default)
    {
        await service.DeleteTaskAsync(taskId, ct);

        return NoContent();
    }

    [HttpPost("batch-delete")]
    [Authorize(Roles = "Director, Manager")]
    public async Task<ActionResult> DeleteTasks(
        [FromBody] IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        await service.DeleteTasksAsync(idList, ct);

        return NoContent();
    }
}
