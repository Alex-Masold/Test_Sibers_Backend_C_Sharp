// using Application.Contracts.ProjectContracts;
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
// public class ProjectServiceTests
// {
//     private readonly Mock<IProjectStore> _projectStoreMock;
//     private readonly Mock<IProjectDocumentStore> _documentStoreMock;
//
//     private readonly Mock<ICurrentUserService> _userServiceMock;
//     private readonly Mock<IFileService> _fileServiceMock;
//
//     private readonly ProjectService _service;
//
//     public ProjectServiceTests()
//     {
//         _projectStoreMock = new Mock<IProjectStore>();
//         _documentStoreMock = new Mock<IProjectDocumentStore>();
//         _userServiceMock = new Mock<ICurrentUserService>();
//         _fileServiceMock = new Mock<IFileService>();
//
//         _service = new ProjectService(
//             _projectStoreMock.Object,
//             _documentStoreMock.Object,
//             _userServiceMock.Object,
//             _fileServiceMock.Object
//         );
//     }
//
//     private void SetupCurrentUser(int userId, bool isDirector = false, Role role = Role.Worker)
//     {
//         _userServiceMock.Setup(x => x.UserId).Returns(userId);
//         _userServiceMock.Setup(x => x.IsDirector).Returns(isDirector);
//         _userServiceMock.Setup(x => x.Role).Returns(role);
//     }
//
//     [Fact]
//     public async Task GetProjectByIdAsync_ShouldReturnProject_WhenUserIsMember()
//     {
//         const int projectId = 200;
//         const int employeeId = 200;
//
//         SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 300,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//             Members = new List<ProjectMember>
//             {
//                 new() { ProjectId = projectId, EmployeeId = employeeId },
//             },
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         var result = await _service.GetProjectAsync(projectId);
//
//         Assert.NotNull(result);
//         Assert.Equal(projectId, result.Id);
//     }
//
//     [Fact]
//     public async Task GetProjectByIdAsync_ShouldThrowAccessDenied_WhenUserIsForeigner()
//     {
//         const int projectId = 200;
//         const int employeeId = 403;
//
//         SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 300,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//             Members = new List<ProjectMember>(),
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetProjectAsync(projectId));
//     }
//
//     [Fact]
//     public async Task GetProjectByIdAsync_ShouldThrowNotFound_WhenProjectDoesNotExist()
//     {
//         const int projectId = 404;
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((Project?)null);
//
//         await Assert.ThrowsAsync<NotFoundException>(() => _service.GetProjectAsync(projectId));
//     }
//
//     [Fact]
//     public async Task GetProjectsAsync_ShouldFilterByEmployee_WhenUserIsNotDirector()
//     {
//         const int employeeId = 200;
//
//         SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);
//
//         var projects = new List<Project>().AsQueryable();
//
//         _projectStoreMock.Setup(x => x.Query()).Returns(projects);
//
//         await _service.GetProjectsAsync(1, 10);
//
//         _projectStoreMock.Verify(x => x.Query(), Times.Once);
//     }
//
//     [Fact]
//     public async Task CreateProjectAsync_ShouldCreate_WhenUserIsDirector()
//     {
//         const int employeeId = 201;
//
//         SetupCurrentUser(200, isDirector: true, role: Role.Director);
//
//         var dto = new ProjectCreateDto()
//         {
//             Name = "New Project",
//             CompanyOrdering = "Co",
//             Priority = 1,
//         };
//
//         var createdEntity = dto.ToEntity();
//         createdEntity.Id = employeeId;
//
//         _projectStoreMock
//             .Setup(x => x.CreateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(createdEntity);
//
//         var result = await _service.CreateProjectAsync(dto);
//
//         Assert.NotNull(result);
//         Assert.Equal(employeeId, result.Id);
//     }
//
//     [Fact]
//     public async Task CreateProjectAsync_ShouldThrowAccessDenied_WhenUserIsManager()
//     {
//         const int managerId = 403;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         var dto = new ProjectCreateDto
//         {
//             Priority = 1,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateProjectAsync(dto));
//     }
//
//     [Fact]
//     public async Task CreateProjectAsync_ShouldThrowValidation_WhenEndDateBeforeStartDate()
//     {
//         SetupCurrentUser(200, isDirector: true, role: Role.Director);
//
//         ProjectCreateDto dto = new()
//         {
//             Priority = 1,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//             // Start = DateOnly.FromDateTime(DateTime.UtcNow)
//             EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddMonths(-1),
//         };
//
//         await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProjectAsync(dto));
//     }
//
//     [Fact]
//     public async Task CreateProjectAsync_ShouldThrowValidation_WhenPriorityIsZero()
//     {
//         SetupCurrentUser(1, isDirector: true, role: Role.Director);
//
//         ProjectCreateDto dto = new()
//         {
//             Priority = 0,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         await Assert.ThrowsAsync<ValidationException>(() => _service.CreateProjectAsync(dto));
//     }
//
//     [Fact]
//     public async Task UpdateProjectAsync_ShouldUpdate_WhenManagerUpdatesOwnProject()
//     {
//         const int managerId = 200;
//         const int projectId = 200;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = managerId,
//             Name = "Old",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         ProjectUpdateDto dto = new() { Name = "New Name" };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _projectStoreMock
//             .Setup(x => x.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         var result = await _service.UpdateProjectAsync(projectId, dto);
//
//         Assert.NotNull(result);
//
//         _projectStoreMock.Verify(
//             x => x.UpdateAsync(project, It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task UpdateProjectAsync_ShouldThrowAccessDenied_WhenManagerUpdatesForeignProject()
//     {
//         const int projectId = 200;
//         const int managerId = 403;
//         const int otherManagerId = 300;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = otherManagerId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         ProjectUpdateDto dto = new() { Name = "Hacked" };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.UpdateProjectAsync(projectId, dto)
//         );
//     }
//
//     [Fact]
//     public async Task UpdateProjectAsync_ShouldThrowValidation_WhenNewEndDateBeforeStartDate()
//     {
//         const int directorId = 200;
//         const int projectId = 200;
//
//         SetupCurrentUser(directorId, isDirector: true, role: Role.Director);
//
//         Project project = new()
//         {
//             Id = projectId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//             StartDate = new DateOnly(2025, 10, 1),
//         };
//
//         ProjectUpdateDto dto = new() { EndDate = project.StartDate.AddMonths(-1) };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<ValidationException>(() =>
//             _service.UpdateProjectAsync(projectId, dto)
//         );
//     }
//
//     [Fact]
//     public async Task DeleteProjectAsync_ShouldDelete_WhenUserIsDirector()
//     {
//         const int projectId = 200;
//
//         SetupCurrentUser(1, isDirector: true, role: Role.Director);
//
//         Project project = new()
//         {
//             Id = projectId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await _service.DeleteProjectAsync(projectId);
//
//         _projectStoreMock.Verify(
//             x => x.DeleteAsync(project, It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task DeleteProjectAsync_ShouldThrowAccessDenied_WhenUserIsManager()
//     {
//         const int projectId = 200;
//         const int managerId = 403;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = managerId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.DeleteProjectAsync(projectId)
//         );
//     }
//
//     [Fact]
//     public async Task UploadProjectDocumentAsync_ShouldSuccess_WhenUserIsManagerOfProject()
//     {
//         const int projectId = 200;
//         const int managerId = 200;
//
//         SetupCurrentUser(managerId, isDirector: true, role: Role.Director);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = managerId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         Mock<IFormFile> fileMock = new();
//         fileMock.Setup(f => f.FileName).Returns("test.pdf");
//         fileMock.Setup(f => f.ContentType).Returns("application/pdf");
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _fileServiceMock
//             .Setup(x => x.SaveFileAsync(fileMock.Object, It.IsAny<CancellationToken>()))
//             .ReturnsAsync("unique_guid.pdf");
//
//         await _service.UploadProjectDocumentAsync(projectId, fileMock.Object);
//
//         _documentStoreMock.Verify(
//             x => x.CreateDocumentAsync(It.IsAny<ProjectDocument>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
//
//     [Fact]
//     public async Task UploadProjectDocumentAsync_ShouldThrowAccessDenied_WhenUserIsWorker()
//     {
//         const int projectId = 200;
//         const int workerId = 403;
//
//         SetupCurrentUser(workerId, role: Role.Worker);
//
//         Project project = new()
//         {
//             Id = projectId,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         var fileMock = new Mock<IFormFile>();
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.UploadProjectDocumentAsync(projectId, fileMock.Object)
//         );
//
//         _fileServiceMock.Verify(
//             x => x.SaveFileAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()),
//             Times.Never
//         );
//     }
//
//     [Fact]
//     public async Task GetDocumentAsync_ShouldReturnStream_WhenUserIsMember()
//     {
//         const int projectId = 200;
//         const int employeeId = 200;
//         const int documentId = 300;
//
//         SetupCurrentUser(employeeId, isDirector: false, role: Role.Worker);
//
//         ProjectDocument document = new()
//         {
//             Id = documentId,
//             ProjectId = projectId,
//             StoredFileName = "file.pdf",
//             ContentType = "app/pdf",
//             OriginalFileName = "original.pdf",
//         };
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 300,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//             Members = new()
//             {
//                 new() { ProjectId = projectId, EmployeeId = employeeId },
//             },
//         };
//
//         _documentStoreMock
//             .Setup(x => x.GetByIdAsync(documentId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(document);
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _fileServiceMock
//             .Setup(x => x.GetFileStream(document.StoredFileName))
//             .Returns(new FileStream(Path.GetTempFileName(), FileMode.Open));
//
//         var result = await _service.GetDocumentAsync(documentId, CancellationToken.None);
//
//         Assert.Equal(document.OriginalFileName, result.FileName);
//         result.stream.Close();
//     }
// }
