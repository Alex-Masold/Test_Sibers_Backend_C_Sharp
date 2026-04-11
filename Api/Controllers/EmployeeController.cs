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
    [HttpGet("{employeeId:int}", Name = "GetEmployee")]
    public async Task<ActionResult<EmployeeReadDto>> GetEmployee(
        [FromRoute] int employeeId,
        CancellationToken ct = default
    )
    {
        var employee = await service.GetEmployeeByIdAsync(employeeId, ct);
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
            new { employeeId = createdEmployee.Id },
            createdEmployee
        );
    }

    [HttpPatch("{employeeId:int}")]
    public async Task<ActionResult<EmployeeReadDto>> UpdateEmployee(
        [FromRoute] int employeeId,
        [FromBody] EmployeeUpdateRequest request,
        CancellationToken ct = default
    )
    {
        var updateDto = request.ToDto();
        var updateEmployee = await service.UpdateEmployeeAsync(employeeId, updateDto, ct);

        return Ok(updateEmployee);
    }

    [HttpDelete("{employeeId:int}")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteEmployee(
        [FromRoute] int employeeId,
        CancellationToken ct = default
    )
    {
        await service.DeleteEmployeeAsync(employeeId, ct);

        return NoContent();
    }

    [HttpPost("batch-delete")]
    [Authorize(Roles = "Director")]
    public async Task<ActionResult> DeleteEmployees(
        [FromBody] IReadOnlyCollection<int> employeeIdList,
        CancellationToken ct = default
    )
    {
        await service.DeleteEmployeesAsync(employeeIdList, ct);

        return NoContent();
    }
}
