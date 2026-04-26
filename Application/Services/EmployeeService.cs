using Application.Contracts;
using Application.Contracts.EmployeeContracts;
using Application.Extensions;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Filters;
using Domain.Interfaces;
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
    public async Task<EmployeeReadDto> GetMeAsync(CancellationToken ct = default)
    {
        var employee = await employeeStore.GetOrThrowAsync(userService.UserId, ct);

        return EmployeeReadDto.From(employee);
    }

    public async Task<EmployeeReadDto> GetEmployeeByIdAsync(
        int employeeId,
        CancellationToken ct = default
    )
    {
        var employee = await employeeStore.GetOrThrowAsync(employeeId, ct);

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
        var employee = await employeeStore.GetOrThrowAsync(employeeId, ct);

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

        await employeeStore.EnsureExists(employeeId, ct);

        var deleted = await employeeStore.DeleteAsync(employeeId, ct);
        await refreshTokenStore.DeleteByUserIdAsync(employeeId, ct);

        return deleted;
    }

    public async Task<int> DeleteEmployeesAsync(
        IReadOnlyCollection<int> employeeIdList,
        CancellationToken ct = default
    )
    {
        foreach (var id in employeeIdList)
        {
            accessValidator.EnsureDeletePermission(id);
        }
        var existingEmployeeIds = await employeeStore.EnsureAllExist(employeeIdList, ct);

        var deleted = await employeeStore.DeleteAsync(existingEmployeeIds, ct);

        await refreshTokenStore.DeleteByUserIdAsync(employeeIdList, ct);

        return deleted;
    }
}
