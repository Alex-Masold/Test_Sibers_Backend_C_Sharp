using Application.Contracts;
using Application.Contracts.EmployeeContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Filters;
using Domain.Interfaces;
using Domain.Models;
using Domain.Sort;
using Domain.Sort.Base;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class EmployeeService(
    IEmployeeStore employeeStore,
    IRefreshTokenStore refreshTokenStore,
    IEmployeeAccessValidator accessValidator,
    IValidator<EmployeeCreateDto> createValidator,
    IValidator<EmployeeUpdateDto> updateValidator,
    IValidator<PagedDto> pagedValidator,
    ICurrentUserService userService,
    IUnitOfWork unitOfWork
)
{
    private async Task<Employee> GetEmployee(int employeeId, CancellationToken ct = default)
    {
        var employee = await employeeStore.GetByIdAsync(employeeId, ct);
        if (employee is null)
            throw new NotFoundException(nameof(Employee), employeeId);
        return employee;
    }

    private async Task EmployeeExist(int employeeId, CancellationToken ct = default)
    {
        var exist = await employeeStore.EmployeeExistAsync(employeeId, ct);

        if (!exist)
        {
            throw new NotFoundException(nameof(Employee), employeeId);
        }
    }

    private async Task<IReadOnlyCollection<int>> EmployeesExist(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        var distinctIdList = idList.Distinct().ToList();
        var existingEmployeeId = await employeeStore.GetExistingIdsAsync(distinctIdList, ct);

        if (existingEmployeeId.Count != distinctIdList.Count)
        {
            var nonExistingIds = distinctIdList
                .Where(id => !existingEmployeeId.Contains(id))
                .ToList();
            throw new NotFoundException(nameof(Employee), nonExistingIds);
        }

        return existingEmployeeId;
    }

    public async Task<EmployeeReadDto> GetMeAsync(CancellationToken ct = default)
    {
        var employee = await GetEmployee(userService.UserId, ct);

        return EmployeeReadDto.From(employee);
    }

    public async Task<EmployeeReadDto> GetEmployeeByIdAsync(int id, CancellationToken ct = default)
    {
        var employee = await GetEmployee(id, ct);

        return EmployeeReadDto.From(employee);
    }

    public async Task<(
        IReadOnlyCollection<EmployeeListDto> Items,
        int TotalCount
    )> GetEmployeesAsync(
        PagedDto pagedDto,
        EmployeeFilter? filter = null,
        SortOptions<EmployeeSortField>? options = null,
        CancellationToken ct = default
    )
    {
        var validationResult = await pagedValidator.ValidateAsync(pagedDto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var result = await employeeStore.GetPagedAsync<EmployeeListDto>(
            pagedDto.PageNumber,
            pagedDto.PageSize,
            EmployeeListDto.Projection,
            filter,
            options,
            ct
        );

        return result;
    }

    public async Task<EmployeeReadDto> CreateEmployeeAsync(
        EmployeeCreateDto dto,
        CancellationToken ct = default
    )
    {
        accessValidator.EnsureCreatePermission();

        var validationResult = await createValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var employee = dto.ToEntity();

        var createdEmployee = employeeStore.Create(employee);
        await unitOfWork.SaveChangesAsync(ct);
        return EmployeeReadDto.From(createdEmployee);
    }

    public async Task<EmployeeReadDto> UpdateEmployeeAsync(
        int employeeId,
        EmployeeUpdateDto dto,
        CancellationToken ct = default
    )
    {
        var employee = await GetEmployee(employeeId, ct);

        accessValidator.EnsureUpdatePermission(employee, dto);

        var validationContext = new ValidationContext<EmployeeUpdateDto>(dto);
        validationContext.RootContextData["ExistingEmployee"] = employee;

        var validationResult = await updateValidator.ValidateAsync(validationContext, ct);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        if (dto.ApplyTo(employee))
            await unitOfWork.SaveChangesAsync(ct);

        return EmployeeReadDto.From(employee);
    }

    public async Task<int> DeleteEmployeeAsync(int employeeId, CancellationToken ct = default)
    {
        accessValidator.EnsureDeletePermission(employeeId);

        await EmployeeExist(employeeId, ct);

        await refreshTokenStore.DeleteByUserIdAsync(employeeId, ct);
        var deleted = await employeeStore.DeleteAsync(employeeId, ct);

        return deleted;
    }

    public async Task<int> DeleteEmployeesAsync(
        IReadOnlyCollection<int> idList,
        CancellationToken ct = default
    )
    {
        foreach (var id in idList)
        {
            accessValidator.EnsureDeletePermission(id);
        }
        var existingEmployeeIds = await EmployeesExist(idList, ct);

        var cleanupTask = existingEmployeeIds.Select(id =>
            refreshTokenStore.DeleteByUserIdAsync(id, ct)
        );
        await Task.WhenAll(cleanupTask);

        var deleted = await employeeStore.DeleteAsync(existingEmployeeIds, ct);

        return deleted;
    }
}
