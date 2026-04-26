using Domain.Models;

public interface IEmployeeFields
{
    string FirstName { get; }
    string? MiddleName { get; }
    string LastName { get; }
    string Email { get; }
    Role Role { get; }
}
