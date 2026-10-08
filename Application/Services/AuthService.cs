using Application.Contracts.AuthContracts;
using Application.Extensions;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Stores;
using FluentValidation;

namespace Application.Services;

public class AuthService(
    IEmployeeStore employeeStore,
    IPasswordService passwordService,
    IRefreshTokenStore refreshTokenStore,
    ITokenService tokenService,
    IInvitationStore invitationStore,
    IValidator<RegisterDto> registoreValidator,
    IValidator<LoginDto> loginValidator,
    IUnitOfWork unitOfWork
)
{
    public async Task<(string accessToken, string refreshToken)> LoginAsync(
        LoginDto dto,
        CancellationToken ct = default
    )
    {
        var validationResult = await loginValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var employee = await employeeStore.GetByEmailAsync(dto.Email, ct);

        if (
            employee is null
            || employee.PasswordHash is null
            || !passwordService.VerifyPassword(employee.PasswordHash, dto.Password)
        )
        {
            if (employee is null)
                passwordService.VerifyPassword(
                    "$2a$11$dummy.hash.to.prevent.timing.attacks",
                    dto.Password
                );

            throw new AuthenticationException("Invalid email or password");
        }

        var accessToken = tokenService.GenerateAccessToken(employee);
        var refreshToken = tokenService.GenerateRefreshToken();

        await refreshTokenStore.SaveAsync(refreshToken, employee.Id, ct);

        return (accessToken, refreshToken);
    }

    public async Task RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        var validationResult = await registoreValidator.ValidateAsync(dto, ct);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var inviteData = await invitationStore.GetAsync(dto.Token, ct);
        if (inviteData is null)
            throw new AuthenticationException("Invitation token is invalid or expired");

        if (await employeeStore.EmailExistsAsync(inviteData.Email, ct))
        {
            await invitationStore.DeleteAsync(dto.Token, ct);
            throw new ValidationException([
                new FluentValidation.Results.ValidationFailure(
                    "Email",
                    "Email already registered."
                ),
            ]);
        }

        var passwordHash = passwordService.HashPassword((dto.Password));
        var employee = dto.ToEntity((inviteData.Email, inviteData.Role, passwordHash));

        employeeStore.Create(employee);
        await unitOfWork.SaveChangesAsync(ct);

        await invitationStore.DeleteAsync(dto.Token, ct);
    }

    public async Task<(string accessToken, string refreshToken)> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default
    )
    {
        var employeeId = await refreshTokenStore.GetIdOrThrowAsync(refreshToken, ct);
        var employee = await employeeStore.GetOrThrowAsync(employeeId, ct);

        var newAccessToken = tokenService.GenerateAccessToken(employee);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        await refreshTokenStore.SaveAsync(newRefreshToken, employeeId, ct);

        await refreshTokenStore.DeleteByTokenAsync(refreshToken, ct);
        return (newAccessToken, newRefreshToken);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        await refreshTokenStore.DeleteByTokenAsync(refreshToken, ct);
    }

    public async Task LogoutAllAsync(string refreshToken, CancellationToken ct = default)
    {
        var userId = await refreshTokenStore.GetIdOrThrowAsync(refreshToken, ct);

        await refreshTokenStore.DeleteByUserIdAsync(userId, ct);
    }
}
