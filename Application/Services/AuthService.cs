using Application.Contracts.LoginContracts;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class AuthService(
    IEmployeeStore employeeStore,
    IRefreshTokenStore refreshTokenStore,
    ITokenService tokenService,
    IValidator<LoginDto> loginValidator
)
{
    private async Task<Employee> GetEmployee(int employeeId, CancellationToken ct = default)
    {
        var employee = await employeeStore.GetByIdAsync(employeeId, ct);
        if (employee is null)
            throw new NotFoundException(nameof(Employee), employeeId);
        return employee;
    }

    private async Task<Employee> GetEmployee(string email, CancellationToken ct = default)
    {
        var employee = await employeeStore.GetByEmailAsync(email, ct);
        if (employee is null)
            throw new AuthenticationException("Invalid email");
        return employee;
    }

    private async Task<int> GetEmployeeId(string refreshToken, CancellationToken ct = default)
    {
        var employeeId = await refreshTokenStore.GetUserIdAsync(refreshToken, ct);
        if (!employeeId.HasValue)
            throw new AuthenticationException("Refresh token not found or expired");
        return employeeId.Value;
    }

    public async Task<(string accessToken, string refreshToken)> LoginAsync(
        LoginDto dto,
        CancellationToken ct = default
    )
    {
        var validationResult = await loginValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var employee = await GetEmployee(dto.Email, ct);

        var accessToken = tokenService.GenerateAccessToken(employee);
        var refreshToken = tokenService.GenerateRefreshToken();

        await refreshTokenStore.SaveAsync(refreshToken, employee.Id, ct);

        return (accessToken, refreshToken);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default
    )
    {
        var employeeId = await GetEmployeeId(refreshToken, ct);
        var employee = await GetEmployee(employeeId, ct);

        await refreshTokenStore.DeleteByTokenAsync(refreshToken, ct);

        var newAccessToken = tokenService.GenerateAccessToken(employee);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await refreshTokenStore.SaveAsync(newRefreshToken, employeeId, ct);
        return (newAccessToken, newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await refreshTokenStore.DeleteByTokenAsync(refreshToken, ct);
    }

    public async Task LogoutAllAsync(string refreshToken, CancellationToken ct = default)
    {
        var userId = await GetEmployeeId(refreshToken, ct);

        await refreshTokenStore.DeleteByUserIdAsync(userId, ct);
    }
}
