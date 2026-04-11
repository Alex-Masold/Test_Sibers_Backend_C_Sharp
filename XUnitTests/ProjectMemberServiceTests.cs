// using Application.Contracts.ProjectMemberContracts;
// using Application.Interfaces;
// using Application.Services;
// using Domain.Exceptions;
// using Domain.Models;
// using Domain.Stores;
// using FluentValidation;
// using Moq;
//
// namespace XUnitTests;
//
// public class ProjectMemberServiceTests
// {
//     private readonly Mock<IProjectStore> _projectStoreMock;
//     private readonly Mock<IProjectMemberStore> _memberStoreMock;
//     private readonly Mock<ICurrentUserService> _currentUserServiceMock;
//
//     private readonly ProjectMemberService _service;
//
//     public ProjectMemberServiceTests()
//     {
//         _projectStoreMock = new Mock<IProjectStore>();
//         _memberStoreMock = new Mock<IProjectMemberStore>();
//         _currentUserServiceMock = new Mock<ICurrentUserService>();
//
//         _service = new ProjectMemberService(
//             _projectStoreMock.Object,
//             _memberStoreMock.Object,
//             _currentUserServiceMock.Object
//         );
//     }
//
//     private void SetupCurrentUser(int userId, bool isDirector = false, Role role = Role.Worker)
//     {
//         _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
//         _currentUserServiceMock.Setup(x => x.IsDirector).Returns(isDirector);
//         _currentUserServiceMock.Setup(x => x.Role).Returns(role);
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldSuccess_WhenDirectorAddsMember()
//     {
//         const int projectId = 200;
//         const int employeeId = 200;
//
//         SetupCurrentUser(1, isDirector: true, role: Role.Director);
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         Project project = new()
//         {
//             Id = dto.ProjectId,
//             ManagerId = 100,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         ProjectMember createdMember = new()
//         {
//             ProjectId = dto.ProjectId,
//             EmployeeId = dto.EmployeeId,
//             Employee = new Employee()
//             {
//                 Id = dto.EmployeeId,
//                 FirstName = "New",
//                 LastName = "Mem",
//                 Email = "mem@siberc.com",
//             },
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _memberStoreMock
//             .Setup(x =>
//                 x.MemberExistsAsync(dto.ProjectId, dto.EmployeeId, It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(false);
//         _memberStoreMock
//             .Setup(x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(createdMember);
//
//         var result = await _service.CreateProjectMemberAsync(dto);
//
//         Assert.NotNull(result);
//         Assert.Equal(employeeId, result.Id);
//
//         _memberStoreMock.Verify(
//             x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldSuccess_WhenManagerAddsToOwnProject()
//     {
//         const int managerId = 200;
//         const int projectId = 200;
//         const int employeeId = 302;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = managerId,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         ProjectMember createdMember = new()
//         {
//             ProjectId = dto.ProjectId,
//             EmployeeId = dto.EmployeeId,
//             Employee = new Employee()
//             {
//                 Id = dto.EmployeeId,
//                 FirstName = "N",
//                 LastName = "M",
//                 Email = "mem@gmail.com",
//             },
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _memberStoreMock
//             .Setup(x =>
//                 x.MemberExistsAsync(dto.ProjectId, dto.EmployeeId, It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(false);
//         _memberStoreMock
//             .Setup(x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(createdMember);
//
//         var result = await _service.CreateProjectMemberAsync(dto);
//
//         Assert.NotNull(result);
//         Assert.Equal(employeeId, result.Id);
//
//         _memberStoreMock.Verify(
//             x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldThrowAccessDenied_WhenManagerAddsToOtherProject()
//     {
//         const int managerId = 401;
//         const int otherManagerId = 302;
//         const int projectId = 302;
//         const int employeeId = 201;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = otherManagerId,
//             Name = "P",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.CreateProjectMemberAsync(dto, It.IsAny<CancellationToken>())
//         );
//
//         _memberStoreMock.Verify(
//             x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldThrowAccessDenied_WhenWorkerTriesToAdd()
//     {
//         const int projectId = 200;
//         const int employeeId = 201;
//         const int workerId = 403;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 500,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.CreateProjectMemberAsync(dto)
//         );
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldThrowValidation_WhenMemberAlreadyExists()
//     {
//         const int projectId = 200;
//         const int employeeId = 409;
//
//         SetupCurrentUser(1, isDirector: true, Role.Director);
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         Project project = new()
//         {
//             Id = dto.ProjectId,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _memberStoreMock
//             .Setup(x =>
//                 x.MemberExistsAsync(dto.ProjectId, dto.EmployeeId, It.IsAny<CancellationToken>())
//             )
//             .ReturnsAsync(true);
//
//         await Assert.ThrowsAsync<ValidationException>(() =>
//             _service.CreateProjectMemberAsync(dto, It.IsAny<CancellationToken>())
//         );
//
//         _memberStoreMock.Verify(
//             x => x.CreateAsync(It.IsAny<ProjectMember>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//     }
//
//     [Fact]
//     public async Task CreateProjectMemberAsync_ShouldThrowNotFound_WhenProjectDoesNotExist()
//     {
//         const int projectId = 404;
//         const int employeeId = 200;
//
//         ProjectMemberCreateDto dto = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(dto.ProjectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((Project?)null);
//
//         await Assert.ThrowsAsync<NotFoundException>(() =>
//             _service.CreateProjectMemberAsync(dto, It.IsAny<CancellationToken>())
//         );
//     }
//
//     [Fact]
//     public async Task DeleteMemberAsync_ShouldSuccess_WhenManagerDeletesFromOwnProject()
//     {
//         const int projectId = 200;
//         const int employeeId = 200;
//         const int managerId = 200;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = managerId,
//             Name = "Test project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         ProjectMember member = new() { ProjectId = projectId, EmployeeId = employeeId };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _memberStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, employeeId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(member);
//
//         var result = await _service.DeleteMemberAsync(projectId, employeeId);
//
//         Assert.NotNull(result);
//         Assert.Equal(projectId, result.Value.Item1);
//
//         _memberStoreMock.Verify(
//             x => x.DeleteAsync(member, It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task DeleteMemberAsync_ShouldThrowNotFound_WhenMemberNotFound()
//     {
//         const int projectId = 200;
//         const int employeeId = 404;
//
//         SetupCurrentUser(1, isDirector: true, role: Role.Director);
//
//         Project project = new()
//         {
//             Id = projectId,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _memberStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, employeeId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((ProjectMember?)null);
//
//         await Assert.ThrowsAsync<NotFoundException>(() =>
//             _service.DeleteMemberAsync(projectId, employeeId)
//         );
//     }
//
//     [Fact]
//     public async Task DeleteMemberAsync_ShouldThrowAccessDenied_WhenWorkerTriesToDelete()
//     {
//         const int workerId = 403;
//         const int projectId = 200;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 200,
//             Name = "Test Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.DeleteMemberAsync(projectId, 101)
//         );
//     }
// }
