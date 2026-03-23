using Api.Models;
using Api.Requests.PaginationRequests;
using Api.Requests.ProjectMemberRequests;
using Application.Contracts.ProjectMemberContracts;
using Application.Services;
using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/project-members")]
public class ProjectMemberController(ProjectMemberService service) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Director, ProjectManager")]
    public async Task<ActionResult<IReadOnlyCollection<ProjectMemberReadDto>>> GetMembers(
        [FromQuery] PaginationRequest request,
        [FromQuery] ProjectMemberFilter? filter = null,
        CancellationToken ct = default
    )
    {
        var result = await service.GetMembersAsync(request.ToDto(), filter, ct);

        return Ok(
            new PagedResponse<ProjectMemberReadDto>()
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            }
        );
    }

    [HttpPost]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ProjectMemberReadDto>> CreateMember(
        [FromBody] ProjectMemberCreateRequest request,
        CancellationToken ct = default
    )
    {
        var dto = request.ToDto();
        var memberDto = await service.CreateMemberAsync(dto, ct);
        return Ok(memberDto);
    }

    [HttpPost("batch")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<IReadOnlyCollection<ProjectMemberReadDto>>> CreateMembers(
        [FromBody] IReadOnlyCollection<ProjectMemberCreateRequest> requests,
        CancellationToken ct = default
    )
    {
        var dtos = requests.Select(r => r.ToDto()).ToList();
        var memberDtos = await service.CreateMembersAsync(dtos, ct);
        return Ok(memberDtos);
    }

    [HttpDelete("{projectId:int}/{employeeId:int}")]
    [Authorize(Roles = "Director, ProjectManager")]
    public async Task<ActionResult> DeleteMember(
        [FromRoute] int projectId,
        [FromRoute] int employeeId,
        CancellationToken ct = default
    )
    {
        var deletedMemberId = await service.DeleteMemberAsync(projectId, employeeId, ct);

        return NoContent();
    }

    [HttpDelete]
    [Authorize(Roles = "Director, ProjectManager")]
    public async Task<ActionResult> DeleteMembers(
        [FromBody] IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var deletedMemberIds = await service.DeleteMembersByIdsAsync(idList, ct);

        return NoContent();
    }
}
