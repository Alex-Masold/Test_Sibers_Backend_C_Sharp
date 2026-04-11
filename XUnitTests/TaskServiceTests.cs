// using Application.Contracts.TaskContracts;
// using Application.Interfaces;
// using Application.Services;
// using Domain.Exceptions;
// using Domain.Models;
// using Domain.Stores;
// using FluentAssertions;
// using Moq;
//
// namespace XUnitTests;
//
// public class TaskServiceTests
// {
//     private readonly Mock<ITaskStore> _taskStoreMock;
//     private readonly Mock<IProjectStore> _projectStoreMock;
//
//     private readonly Mock<ICurrentUserService> _userService;
//
//     private readonly TaskService _service;
//
//     public TaskServiceTests()
//     {
//         _taskStoreMock = new();
//         _projectStoreMock = new();
//
//         _userService = new();
//
//         _service = new(_taskStoreMock.Object, _projectStoreMock.Object, _userService.Object);
//     }
//
//     private void SetupCurrentUser(int userId, bool isDirector = false, Role role = Role.Worker)
//     {
//         _userService.Setup(x => x.UserId).Returns(userId);
//         _userService.Setup(x => x.IsDirector).Returns(isDirector);
//         _userService.Setup(x => x.Role).Returns(role);
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldReturnTask_WhenUserIsDirector()
//     {
//         const int taskId = 200;
//         const int directorId = 200;
//
//         SetupCurrentUser(directorId, isDirector: true, role: Role.Director);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             Title = "Title",
//             ProjectId = 300,
//             AuthorId = 300,
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         var result = await _service.GetTaskByIdAsync(taskId);
//
//         Assert.NotNull(result);
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldReturnTask_WhenManagerIsOwnerOfProject()
//     {
//         const int taskId = 200;
//         const int managerId = 200;
//         const int projectId = 300;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             Title = "Title",
//             AuthorId = 300,
//             ProjectId = projectId,
//             Project = new()
//             {
//                 Id = projectId,
//                 ManagerId = managerId,
//                 Name = "Project",
//                 CompanyOrdering = "CO",
//                 CompanyExecuting = "CE",
//             },
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         var result = await _service.GetTaskByIdAsync(taskId);
//
//         Assert.NotNull(result);
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldReturnTask_WhenWorkerIsExecutor()
//     {
//         const int taskId = 200;
//         const int workerId = 200;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             AuthorId = 300,
//             ProjectId = 300,
//             ExecutorId = workerId,
//             Title = "Title",
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         var result = await _service.GetTaskByIdAsync(taskId);
//
//         Assert.NotNull(result);
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldThrowAccessDenied_WhenManagerViewsForeignProjectTask()
//     {
//         const int taskId = 200;
//         const int projectId = 300;
//         const int managerId = 403;
//         const int otherManagerId = 300;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             ProjectId = projectId,
//             AuthorId = 301,
//             Title = "Title",
//             Project = new()
//             {
//                 Id = projectId,
//                 ManagerId = otherManagerId,
//                 Name = "Project",
//                 CompanyOrdering = "CO",
//                 CompanyExecuting = "CE",
//             },
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetTaskByIdAsync(taskId));
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldThrowAccessDenied_WhenWorkerViewsOtherTask()
//     {
//         const int taskId = 200;
//         const int workerId = 403;
//         const int otherWorkerId = 300;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             ProjectId = 300,
//             AuthorId = 301,
//             ExecutorId = otherWorkerId,
//             Title = "title",
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetTaskByIdAsync(taskId));
//     }
//
//     [Fact]
//     public async Task GetTaskByIdAsync_ShouldThrowNotFound_WhenTaskDoesNotExist()
//     {
//         const int notExistTaskId = 404;
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(notExistTaskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((WorkTask?)null);
//
//         await Assert.ThrowsAsync<NotFoundException>(() =>
//             _service.GetTaskByIdAsync(notExistTaskId)
//         );
//     }
//
//     [Fact]
//     public async Task GetTasksAsync_ShouldApplyManagerFilter_WhenUserIsManager()
//     {
//         const int managerId = 200;
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         var tasks = new List<WorkTask>
//         {
//             new WorkTask
//             {
//                 Id = 1,
//                 Title = "Task 1",
//                 AuthorId = 1,
//                 ProjectId = 1,
//                 Project = new Project
//                 {
//                     Id = 1,
//                     Name = "Project 1",
//                     CompanyOrdering = "Company",
//                     StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
//                     ManagerId = managerId,
//                 },
//             },
//             new WorkTask
//             {
//                 Id = 2,
//                 Title = "Task 2",
//                 AuthorId = 1,
//                 ProjectId = 2,
//                 Project = new Project
//                 {
//                     Id = 2,
//                     Name = "Project 2",
//                     CompanyOrdering = "Company",
//                     StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
//                     ManagerId = 999,
//                 },
//             },
//         }.AsQueryable();
//
//         _taskStoreMock.Setup(x => x.Query()).Returns(tasks);
//
//         var result = await _service.GetTasksAsync(1, 10);
//
//         result.Items.Should().HaveCount(1);
//         result.Items.First().Id.Should().Be(1);
//     }
//
//     [Fact]
//     public async Task GetTasksAsync_ShouldApplyExecutorFilter_WhenUserIsWorker()
//     {
//         const int workerId = 200;
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         var tasks = new List<WorkTask>
//         {
//             new WorkTask
//             {
//                 Id = 1,
//                 Title = "Task 1",
//                 AuthorId = 1,
//                 ProjectId = 1,
//                 ExecutorId = workerId,
//             },
//         }.AsQueryable();
//
//         _taskStoreMock.Setup(x => x.Query()).Returns(tasks);
//
//         var result = await _service.GetTasksAsync(1, 10);
//
//         result.Items.Should().HaveCount(1);
//         result.Items.First().Id.Should().Be(1);
//     }
//
//     [Fact]
//     public async Task CreateTaskAsync_ShouldSuccess_WhenManagerCreatesInOwnProject()
//     {
//         const int managerId = 200;
//         const int projectId = 200;
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
//         TaskCreateDto dto = new()
//         {
//             Title = "Task",
//             ProjectId = projectId,
//             AuthorId = managerId,
//         };
//         var createdTask = dto.ToEntity();
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//         _taskStoreMock
//             .Setup(x => x.CreateAsync(It.IsAny<WorkTask>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(createdTask);
//
//         var result = await _service.CreateTaskAsync(dto);
//
//         Assert.NotNull(result);
//     }
//
//     [Fact]
//     public async Task CreateTaskAsync_ShouldThrowAccessDenied_WhenManagerCreatesInForeignProject()
//     {
//         const int managerId = 403;
//         const int projectId = 200;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         Project project = new()
//         {
//             Id = projectId,
//             ManagerId = 300,
//             Name = "Project",
//             CompanyOrdering = "CO",
//             CompanyExecuting = "CE",
//         };
//
//         TaskCreateDto dto = new()
//         {
//             Title = "Title",
//             ProjectId = projectId,
//             AuthorId = managerId,
//         };
//
//         _projectStoreMock
//             .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateTaskAsync(dto));
//     }
//
//     [Fact]
//     public async Task CreateTaskAsync_ShouldThrowAccessDenied_WhenWorkerCreates()
//     {
//         const int workerId = 403;
//         const int projectId = 200;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         TaskCreateDto dto = new()
//         {
//             Title = "Task",
//             ProjectId = 10,
//             AuthorId = workerId,
//         };
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
//             .Setup(x => x.GetByIdAsync(10, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(project);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateTaskAsync(dto));
//     }
//
//     [Fact]
//     public async Task UpdateTaskAsync_ShouldSuccess_WhenWorkerUpdatesStatusOnly()
//     {
//         const int workerId = 200;
//         const int taskId = 200;
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         var task = new WorkTask
//         {
//             Id = taskId,
//             ExecutorId = workerId,
//             Title = "Original",
//             ProjectId = 10,
//             AuthorId = 1,
//         };
//
//         var dto = new TaskUpdateDto { Status = WorkTaskStatus.Done };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//         _taskStoreMock
//             .Setup(x => x.UpdateAsync(It.IsAny<WorkTask>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         var result = await _service.UpdateTaskAsync(taskId, dto);
//
//         Assert.NotNull(result);
//         _taskStoreMock.Verify(x => x.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task UpdateTaskAsync_ShouldSuccess_WhenManagerUpdatesOwnProjectTask()
//     {
//         const int taskId = 200;
//         const int projectId = 200;
//         const int managerId = 200;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             ProjectId = projectId,
//             AuthorId = 300,
//             Title = "Title",
//             Project = new()
//             {
//                 Id = projectId,
//                 ManagerId = managerId,
//                 Name = "Test Project",
//                 CompanyOrdering = "CO",
//                 CompanyExecuting = "CE",
//             },
//         };
//
//         TaskUpdateDto dto = new() { Title = "New Title" };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//         _taskStoreMock
//             .Setup(x => x.UpdateAsync(task, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await _service.UpdateTaskAsync(taskId, dto);
//
//         _taskStoreMock.Verify(x => x.UpdateAsync(task, It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task UpdateTaskAsync_ShouldThrowAccessDenied_WhenWorkerUpdatesForbiddenFields()
//     {
//         const int taskId = 200;
//         const int workerId = 403;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             ExecutorId = workerId,
//             ProjectId = 300,
//             AuthorId = 300,
//             Title = "Original",
//         };
//
//         TaskUpdateDto dto = new() { Title = "Hacked Title" };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.UpdateTaskAsync(taskId, dto)
//         );
//     }
//
//     [Fact]
//     public async Task UpdateTaskAsync_ShouldThrowAccessDenied_WhenWorkerUpdatesForeignTask()
//     {
//         const int taskId = 200;
//         const int projectId = 300;
//         const int workerId = 403;
//         const int otherWorkerId = 300;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             ProjectId = projectId,
//             AuthorId = 301,
//             ExecutorId = otherWorkerId,
//             Title = "Original",
//         };
//
//         TaskUpdateDto dto = new() { Status = WorkTaskStatus.Done };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() =>
//             _service.UpdateTaskAsync(taskId, dto)
//         );
//     }
//
//     [Fact]
//     public async Task DeleteTaskAsync_ShouldSuccess_WhenManagerDeletesOwnProjectTask()
//     {
//         const int taskId = 200;
//         const int managerId = 200;
//         const int projectId = 200;
//
//         SetupCurrentUser(managerId, isDirector: false, role: Role.Manager);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             Title = "Title",
//             ProjectId = projectId,
//             AuthorId = 300,
//             Project = new()
//             {
//                 Id = projectId,
//                 ManagerId = managerId,
//                 Name = "Test Progect",
//                 CompanyOrdering = "CO",
//                 CompanyExecuting = "CE",
//             },
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await _service.DeleteTaskAsync(taskId);
//
//         _taskStoreMock.Verify(x => x.DeleteAsync(task, It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task DeleteTaskAsync_ShouldThrowAccessDenied_WhenWorkerTriesToDelete()
//     {
//         const int taskId = 200;
//         const int workerId = 403;
//         const int projectId = 200;
//
//         SetupCurrentUser(workerId, isDirector: false, role: Role.Worker);
//
//         WorkTask task = new()
//         {
//             Id = taskId,
//             Title = "Title",
//             ExecutorId = workerId,
//             ProjectId = projectId,
//             AuthorId = 300,
//         };
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(task);
//
//         await Assert.ThrowsAsync<AccessDeniedException>(() => _service.DeleteTaskAsync(taskId));
//     }
//
//     [Fact]
//     public async Task DeleteTaskAsync_ShouldThrowNotFound_WhenTaskDoesNotExist()
//     {
//         const int taskId = 404;
//
//         _taskStoreMock
//             .Setup(x => x.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync((WorkTask?)null);
//
//         await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTaskAsync(taskId));
//     }
// }
