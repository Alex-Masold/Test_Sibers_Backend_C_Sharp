using Application.Contracts.EmployeeContracts;
using Domain.Models;

namespace Application.Interfaces.Access;

public interface IEmployeeAccessValidator
{
    void EnsureCreatePermission();
    void EnsureUpdatePermission(Employee employee, EmployeeUpdateDto dto);
    void EnsureDeletePermission(int id);
}
