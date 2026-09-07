using Application.Contracts.EmployeeContracts;
using Application.Interfaces;
using Application.Interfaces.Access;
using Domain.Exceptions;
using Domain.Models;

namespace Application.Validators.EmployeeValidators;

public class EmployeeAccessValidator(ICurrentUserService userService) : IEmployeeAccessValidator
{
    private void IsCurrentUser(int employeeId)
    {
        if (userService.UserId != employeeId)
            throw new AccessDeniedException("You only have permission to update your own profile");
    }

    public void EnsureCreatePermission()
    {
        if (!userService.IsDirector)
            throw new AccessDeniedException("Only directors can create new employees");
    }

    public void EnsureUpdatePermission(int employeeId)
    {
        if (!userService.IsDirector)
            IsCurrentUser(employeeId);
    }

    public void EnsureUpdatePermission(Employee employee, EmployeeUpdateDto dto)
    {
        if (!userService.IsDirector)
        {
            IsCurrentUser(employee.Id);
            if (dto.Role.HasValue && dto.Role.Value != employee.Role)
                throw new AccessDeniedException("Only directors can change employees roles");
        }
    }

    public void EnsureDeletePermission(int id)
    {
        if (!userService.IsDirector)
            throw new AccessDeniedException("Only directors can delete employees");

        if (userService.UserId == id)
            throw new AccessDeniedException("Cannot delete yourself");
    }
}
