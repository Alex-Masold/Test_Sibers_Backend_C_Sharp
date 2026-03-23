using Api.Models;
using Api.Requests.PaginationRequests;
using Api.Requests.ProjectRequests;
using Application.Contracts.ProjectContracts;
using Application.Contracts.ProjectDocumentContracts;
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
[Route("api/projects")]
public class ProjectController(ProjectService service, ProjectDocumentService documentService)
    : ControllerBase
{
    [HttpGet("{projectId:int}", Name = "GetProject")]
    public async Task<ActionResult<ProjectReadDto>> GetProject(
        [FromRoute] int projectId,
        CancellationToken ct = default
    )
    {
        var project = await service.GetProjectAsync(projectId, ct);

        return Ok(project);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProjectListDto>>> GetProjects(
        [FromQuery] PaginationRequest request,
        [FromQuery] ProjectFilter? filter = null,
        [FromQuery] string? sortQuery = null,
        CancellationToken ct = default
    )
    {
        var options = new SortOptions<ProjectSortField>()
        {
            Items = SortParser<ProjectSortField>.Parse(sortQuery),
        };

        var result = await service.GetProjectsAsync(request.ToDto(), filter, options, ct);

        return Ok(
            new PagedResponse<ProjectListDto>()
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            }
        );
    }

    [HttpPost("{projectId}/documents")]
    [Authorize(Roles = "Director, Manager")]
    public async Task<IActionResult> UploadDocument(
        int projectId,
        IFormFile file,
        CancellationToken ct = default
    )
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is empty");

        await documentService.UploadProjectDocumentAsync(projectId, file, ct);

        return Ok();
    }

    [HttpGet("{projectId:int}/documents")]
    public async Task<ActionResult<PagedResponse<ProjectDocumentReadDto>>> GetDocuments(
        [FromQuery] PaginationRequest request,
        [FromRoute] int projectId,
        CancellationToken ct = default
    )
    {
        var result = await documentService.GetDocumentsAsync(projectId, request.ToDto(), ct);

        return Ok(
            new PagedResponse<ProjectDocumentReadDto>()
            {
                Items = result.Items,
                TotalCount = result.TotalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            }
        );
    }

    [HttpGet("documents/{documentId:int}")]
    public async Task<IActionResult> DownloadDocument(
        int documentId,
        CancellationToken ct = default
    )
    {
        var (stream, contentType, fileName) = await documentService.GetDocumentAsync(
            documentId,
            ct
        );

        return File(stream, contentType, fileName);
    }

    [HttpPost]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult<ProjectReadDto>> CreateProject(
        [FromBody] ProjectCreateRequest request,
        CancellationToken ct = default
    )
    {
        var createDto = request.ToDto();
        var created = await service.CreateProjectAsync(createDto, ct);

        return CreatedAtRoute("GetProject", new { projectId = created.Id }, created);
    }

    [HttpPatch("{projectId:int}")]
    [Authorize(Roles = "Director, Manager")]
    public async Task<ActionResult<ProjectReadDto>> UpdateProject(
        [FromRoute] int projectId,
        [FromBody] ProjectUpdateRequest request,
        CancellationToken ct = default
    )
    {
        var updateDto = request.ToDto();
        var updatedProject = await service.UpdateProjectAsync(projectId, updateDto, ct);

        return Ok(updatedProject);
    }

    [HttpDelete("{projectId:int}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteProject(
        [FromRoute] int projectId,
        CancellationToken ct = default
    )
    {
        await service.DeleteProjectAsync(projectId, ct);

        return NoContent();
    }

    [HttpPost("delete-batch")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteProjects(
        [FromBody] IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        await service.DeleteProjectAsync(idList, ct);

        return NoContent();
    }
}
