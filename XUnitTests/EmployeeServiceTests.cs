using Application.Contracts.EmployeeContracts;
using Application.Interfaces;
using Application.Services;
using Domain.Exceptions;
using Domain.Models;
using Domain.Stores;
using Moq;
using ValidationException = FluentValidation.ValidationException;

namespace XUnitTests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeStore> _employeeStoreMock;
    private readonly Mock<IRefreshTokenStore> _refreshTokenStoreMock;
    private readonly Mock<ICurrentUserService> _userServiceMock;

    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _employeeStoreMock = new Mock<IEmployeeStore>();
        _userServiceMock = new Mock<ICurrentUserService>();
        _refreshTokenStoreMock = new Mock<IRefreshTokenStore>();

        _service = new EmployeeService(
            _employeeStoreMock.Object,
            _userServiceMock.Object,
            _refreshTokenStoreMock.Object
        );
    }

    private void SetupCurrentUser(int userId, bool isDirector = false, Role role = Role.Worker)
    {
        _userServiceMock.Setup(x => x.UserId).Returns(userId);
        _userServiceMock.Setup(x => x.IsDirector).Returns(isDirector);
        _userServiceMock.Setup(x => x.Role).Returns(role);
    }

    [Fact]
    public async Task GetMeAsync_ShouldReturnEmployee_WhenTokenIsValid()
    {
        const int employeeId = 200;
        const string refreshToken = "valid_refresh_token";

        Employee employee = new()
        {
            Id = employeeId,
            Email = "me@sibers.com",
            FirstName = "My",
            LastName = "Self",
        };

        _refreshTokenStoreMock
            .Setup(x => x.GetUserIdAsync(refreshToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeId);

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _service.GetMeAsync(refreshToken);

        Assert.NotNull(result);
        Assert.Equal(employee.Id, result.Id);
    }

    [Fact]
    public async Task GetMeAsync_ShouldThrowAuthentication_WhenTokenIsInvalid()
    {
        const string invalidToken = "invalid_or_expired_token";

        _refreshTokenStoreMock
            .Setup(x => x.GetUserIdAsync(invalidToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        await Assert.ThrowsAsync<AuthenticationException>(() => _service.GetMeAsync(invalidToken));
    }

    [Fact]
    public async Task GetEmployeeByIdAsync_ShouldThrowNotFound_WhenEmployeeDoesNotExist()
    {
        const int employeeId = 404;

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetEmployeeByIdAsync(employeeId)
        );
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldCreateEmployee_WhenUserIsDirector()
    {
        const int directorId = 200;

        SetupCurrentUser(directorId, isDirector: true, role: Role.Director);

        var createDto = new EmployeeCreateDto()
        {
            Email = "new@sibers.com",
            FirstName = "New",
            LastName = "User",
        };

        var employee = createDto.ToEntity();

        _employeeStoreMock
            .Setup(x => x.EmailExistAsync(createDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _employeeStoreMock
            .Setup(x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        var result = await _service.CreateEmployeeAsync(createDto);

        Assert.NotNull(result);

        _employeeStoreMock.Verify(
            x => x.CreateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldAccessDenied_WhenUserIsNotDirector()
    {
        const int notDirectorId = 403;

        SetupCurrentUser(notDirectorId, isDirector: false, role: Role.Worker);

        var createDto = new EmployeeCreateDto
        {
            Email = "new@sibers.com",
            FirstName = "New",
            LastName = "User",
        };

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            _service.CreateEmployeeAsync(createDto)
        );
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldThrowValidation_WhenEmailExists()
    {
        const int directorId = 200;

        SetupCurrentUser(directorId, isDirector: true, role: Role.Director);

        var createDto = new EmployeeCreateDto
        {
            Email = "existsing@sibers.com",
            FirstName = "New",
            LastName = "User",
        };

        _employeeStoreMock
            .Setup(x => x.EmailExistAsync(createDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.CreateEmployeeAsync(createDto)
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdate_WhenDirectorUpdatesAnotherUser()
    {
        const int directorId = 200;
        const int otherEmployeeId = 300;

        SetupCurrentUser(directorId, isDirector: true, role: Role.Director);

        var employeeToUpdate = new Employee()
        {
            Id = otherEmployeeId,
            Email = "target_user@sibers.com",
            FirstName = "Old",
            LastName = "Name",
        };

        var updateDto = new EmployeeUpdateDto() { FirstName = "UpdatedNameByDirector" };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeToUpdate.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);
        _employeeStoreMock
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);

        var result = await _service.UpdateEmployeeAsync(employeeToUpdate.Id, updateDto);

        Assert.NotNull(result);

        _employeeStoreMock.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdate_WhenUserUpdateOwnProfile()
    {
        const int employeeId = 200;

        SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);

        Employee employeeToUpdate = new()
        {
            Id = employeeId,
            Email = "me@sibers.com",
            FirstName = "OldName",
            LastName = "Self",
        };
        var updateDto = new EmployeeUpdateDto { FirstName = "NewSelfName" };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);

        _employeeStoreMock
            .Setup(x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);

        var result = await _service.UpdateEmployeeAsync(employeeId, updateDto);

        Assert.NotNull(result);
        _employeeStoreMock.Verify(
            x => x.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldThrowAccessDenied_WhenUserUpdatesAnotherUserProfile()
    {
        const int currentEmployeeId = 403;
        const int otherEmployeeId = 300;

        SetupCurrentUser(currentEmployeeId, isDirector: false, role: Role.Worker);

        Employee employeeToUpdate = new()
        {
            Id = otherEmployeeId,
            Email = "other@sibers.com",
            FirstName = "Other",
            LastName = "Employee",
        };

        var updateDto = new EmployeeUpdateDto { FirstName = "HackedName" };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(otherEmployeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            _service.UpdateEmployeeAsync(otherEmployeeId, updateDto)
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldThrowValidation_WhenEmailAlreadyExists()
    {
        const int employeeId = 200;

        SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);

        Employee employee = new()
        {
            Id = employeeId,
            Email = "current@sibers.com",
            FirstName = "Current",
            LastName = "User",
        };
        var updateDto = new EmployeeUpdateDto { Email = "busy@sibers.com" };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);
        _employeeStoreMock
            .Setup((x => x.EmailExistAsync(updateDto.Email, It.IsAny<CancellationToken>())))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.UpdateEmployeeAsync(employeeId, updateDto)
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldThrowAccessDenied_WhenUserTriesToChangeRole()
    {
        const int notDirectorId = 403;

        SetupCurrentUser(notDirectorId, isDirector: false, role: Role.Worker);

        Employee employeeToUpdate = new()
        {
            Id = notDirectorId,
            Email = "notDirector@sibers.com",
            FirstName = "Current",
            LastName = "user",
            Role = Role.Worker,
        };

        var dto = new EmployeeUpdateDto() { Role = Role.Director };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(notDirectorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToUpdate);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            _service.UpdateEmployeeAsync(notDirectorId, dto)
        );
    }

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldThrowNotFound_WhenEmployeeNotFound()
    {
        const int employeeId = 404;

        SetupCurrentUser(200, isDirector: true, Role.Director);

        EmployeeUpdateDto dto = new();

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateEmployeeAsync(employeeId, dto)
        );
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldDelete_WhenUserIsDirector()
    {
        const int directorId = 200;
        const int otherEmployeeId = 300;

        SetupCurrentUser(directorId, isDirector: true, role: Role.Director);

        Employee employeeToDelete = new()
        {
            Id = otherEmployeeId,
            Email = "director@sibers.com",
            FirstName = "Me",
            LastName = "Director",
        };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeToDelete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToDelete);

        await _service.DeleteEmployeeAsync(employeeToDelete.Id);

        _employeeStoreMock.Verify(
            x => x.DeleteAsync(employeeToDelete, It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldThrowAccessDenied_WhenUserIsNotDirector()
    {
        const int notDirectorId = 403;
        const int otherEmployeeId = 300;

        SetupCurrentUser(notDirectorId, isDirector: false, role: Role.Worker);

        Employee employeeToDelete = new()
        {
            Id = otherEmployeeId,
            Email = "notDirector@sibers.com",
            FirstName = "Me",
            LastName = "Worker",
        };

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeToDelete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(employeeToDelete);

        await Assert.ThrowsAsync<AccessDeniedException>(() =>
            _service.DeleteEmployeeAsync(employeeToDelete.Id)
        );
    }

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldReturnNull_WhenEmployeeNotFound()
    {
        const int employeeId = 404;
        SetupCurrentUser(1, isDirector: true);

        _employeeStoreMock
            .Setup(x => x.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteEmployeeAsync(employeeId));
    }
}
