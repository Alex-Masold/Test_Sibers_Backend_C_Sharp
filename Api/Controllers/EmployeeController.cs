using Api.Models;
using Api.Requests.EmployeeRequests;
using Api.Requests.PaginationRequests;
using Application.Contracts.EmployeeContracts;
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
[Route("api/employees")]
public class EmployeeController(EmployeeService service) : ControllerBase
{
    [HttpGet("{id:int}", Name = "GetEmployee")]
    public async Task<ActionResult<EmployeeReadDto>> GetEmployee(
        [FromRoute] int id,
        CancellationToken ct = default
    )
    {
        var employee = await service.GetEmployeeByIdAsync(id, ct);
        return Ok(employee);
    }

    [HttpGet("me")]
    public async Task<ActionResult<EmployeeReadDto>> GetMe(CancellationToken ct = default)
    {
        var employee = await service.GetMeAsync(ct);
        return Ok(employee);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<EmployeeListDto>>> GetEmployees(
        [FromQuery] PaginationRequest request,
        [FromQuery] EmployeeFilter? filter = null,
        [FromQuery] string? sortQuery = null,
        CancellationToken ct = default
    )
    {
        var options = new SortOptions<EmployeeSortField>()
        {
            Items = SortParser<EmployeeSortField>.Parse(sortQuery),
        };

        var result = await service.GetEmployeesAsync(request.ToDto(), filter, options, ct);

        return Ok(
            new PagedResponse<EmployeeListDto>()
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
    public async Task<ActionResult<EmployeeReadDto>> CreateEmployee(
        [FromBody] EmployeeCreateRequest request,
        CancellationToken ct = default
    )
    {
        var createDto = request.ToDto();
        var createdEmployee = await service.CreateEmployeeAsync(createDto, ct);
        return CreatedAtAction(
            nameof(GetEmployee),
            new { id = createdEmployee.Id },
            createdEmployee
        );
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<EmployeeReadDto>> UpdateEmployee(
        [FromRoute] int id,
        [FromBody] EmployeeUpdateRequest request,
        CancellationToken ct = default
    )
    {
        var updateDto = request.ToDto();
        var updateEmployee = await service.UpdateEmployeeAsync(id, updateDto, ct);

        return Ok(updateEmployee);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteEmployee(
        [FromRoute] int id,
        CancellationToken ct = default
    )
    {
        await service.DeleteEmployeeAsync(id, ct);

        return NoContent();
    }

    [HttpPost("batch-delete")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteEmployees(
        [FromBody] IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        await service.DeleteEmployeesAsync(idList, ct);

        return NoContent();
    }
}
