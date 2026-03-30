using Application.Contracts.LoginContracts;
using Application.Interfaces;
using Application.Services;
using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;
using Moq;

namespace XUnitTests;

public class AuthServiceTests
{
    private readonly Mock<IEmployeeStore> _employeeStoreMock;
    private readonly Mock<IRefreshTokenStore> _refreshTokenStoreMock;
    private readonly Mock<ITokenService> _tokenServiceMock;

    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _employeeStoreMock = new Mock<IEmployeeStore>();
        _refreshTokenStoreMock = new Mock<IRefreshTokenStore>();
        _tokenServiceMock = new Mock<ITokenService>();

        _authService = new AuthService(
            _employeeStoreMock.Object,
            _refreshTokenStoreMock.Object,
            _tokenServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTokens_WhenEmailIsCorrect()
    {
        const string expectedAccess = "access_token";
        const string expectedRefresh = "refresh_token";
        const string correctEmail = "correct@sibers.com";

        var loginDto = new LoginDto() { Email = correctEmail };

        Employee employee = new()
        {
            Id = 300,
            Email = loginDto.Email,
            FirstName = "Should_Return_Tokens",
            LastName = "When_Email_Is_Correct",
        };

        _employeeStoreMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(employee)).Returns(expectedAccess);

        _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns(expectedRefresh);

        var (accessToken, refreshToken) = await _authService.LoginAsync(loginDto);

        Assert.Equal(expectedAccess, accessToken);
        Assert.Equal(expectedRefresh, refreshToken);

        _refreshTokenStoreMock.Verify(
            x => x.SaveAsync(expectedRefresh, employee.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowAuthentication_WhenEmailIsInvalid()
    {
        const string wrongEmail = "wrong@email.com";

        var loginDto = new LoginDto() { Email = wrongEmail };

        _employeeStoreMock
            .Setup(x => x.GetByEmailAsync(loginDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<AuthenticationException>(() => _authService.LoginAsync(loginDto));

        _refreshTokenStoreMock.Verify(
            x => x.SaveAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public async Task RefreshAsync_ShouldReturnNewAccessToken_WhenTokenIsValid()
    {
        const int employeeId = 200;
        const string refreshToken = "valid_refresh_token";
        const string newAccessToken = "new_access_token";

        Employee employee = new()
        {
            Id = employeeId,
            FirstName = "Test",
            LastName = "User",
            Email = "test@mail.com",
        };

        _refreshTokenStoreMock
            .Setup(x => x.GetUserIdAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeId);

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        _tokenServiceMock.Setup(x => x.GenerateAccessToken(employee)).Returns(newAccessToken);

        var result = await _authService.RefreshAsync(refreshToken);

        Assert.Equal(newAccessToken, result);
    }

    [Fact]
    public async Task RefreshAsync_ShouldThrowAccessDenied_WhenTokenNotFoundOrExpired()
    {
        const string refreshToken = "expired_token";

        _refreshTokenStoreMock
            .Setup((x => x.GetUserIdAsync(refreshToken, It.IsAny<CancellationToken>())))
            .ReturnsAsync((int?)null);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            _authService.RefreshAsync(refreshToken)
        );
    }

    [Fact]
    public async Task RefreshAsync_ShouldThrowNotFound_WhenUserDeletedButTokenExists()
    {
        const int employeeId = 200;
        const string refreshToken = "valid_token";

        _refreshTokenStoreMock
            .Setup(x => x.GetUserIdAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeId);

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _authService.RefreshAsync(refreshToken));
    }

    [Fact]
    public async Task LogoutAsync_ShouldCallDelete()
    {
        const string refreshToken = "token_do_delete";

        await _authService.LogoutAsync(refreshToken);

        _refreshTokenStoreMock.Verify(
            x => x.DeleteByTokenAsync(refreshToken, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
