using Microsoft.EntityFrameworkCore;
using Moq;
using TaskManagement.Api.Clients.TaskAudit;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Data;
using TaskManagement.Api.Messaging;
using TaskManagement.Api.Models;
using TaskManagement.Api.Services;
using TaskManagement.Contracts.Events;

namespace TaskManagement.UnitTests
{
	public class TaskServiceTests
	{
		private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;
		private readonly Mock<ITaskEventProducer> _eventProducerMock;
		private readonly Mock<ITaskAuditClient> _auditClientMock;


		public TaskServiceTests()
		{
			_eventProducerMock = new Mock<ITaskEventProducer>();
			_auditClientMock = new Mock<ITaskAuditClient>();

			_eventProducerMock
				.Setup(x => x.PublishAsync(
					It.IsAny<TaskChangedEvent>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);

			_auditClientMock
				.Setup(x => x.LogAsync(
					It.IsAny<TaskChangedEvent>(),
					It.IsAny<CancellationToken>()))
				.Returns(Task.CompletedTask);
		}

		private static TaskDbContext CreateDbContext()
		{
			var options = new DbContextOptionsBuilder<TaskDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			return new TaskDbContext(options);
		}

		private TaskService CreateService(TaskDbContext dbContext)
		{
			return new TaskService(
				dbContext,
				_eventProducerMock.Object,
				_auditClientMock.Object
			);
		}

		[Fact]
		public async Task CreateAsync_ShouldCreateTask_WithNewStatus()
		{
			await using var dbContext = CreateDbContext();
			var service = CreateService(dbContext);
			var request = new CreateTaskRequest(
				"Test task",
				"Test task description"
			);

			var result = await service.CreateAsync(request, _cancellationToken);

			Assert.NotEqual(Guid.Empty, result.Id);
			Assert.Equal("Test task", result.Title);
			Assert.Equal("Test task description", result.Description);
			Assert.Equal(TaskItemStatus.New, result.Status);
			Assert.NotEqual(default, result.CreatedAt);
			Assert.NotEqual(default, result.UpdatedAt);

			var entity = await dbContext.Tasks.SingleAsync(cancellationToken: _cancellationToken);

			Assert.Equal(result.Id, entity.Id);
		}

		[Fact]
		public async Task CreateAsync_ShouldPublishCreatedEvent()
		{
			await using var dbContext = CreateDbContext();
			var service = CreateService(dbContext);
			var request = new CreateTaskRequest(
				"Test task",
				"Test task description"
			);

			var result = await service.CreateAsync(request, _cancellationToken);

			_eventProducerMock.Verify(
				x => x.PublishAsync(
					It.Is<TaskChangedEvent>(e =>
						e.TaskId == result.Id &&
						e.Title == result.Title &&
						e.EventType == TaskEventType.Created),
					It.IsAny<CancellationToken>()),
				Times.Once
			);

			_auditClientMock.Verify(
				x => x.LogAsync(
					It.Is<TaskChangedEvent>(e =>
						e.TaskId == result.Id &&
						e.Title == result.Title &&
						e.EventType == TaskEventType.Created),
					It.IsAny<CancellationToken>()),
				Times.Once
			);
		}

		[Fact]
		public async Task GetByIdAsync_WhenTaskExists_ShouldReturnTask()
		{
			await using var dbContext = CreateDbContext();
			var task = new TaskItem
			{
				Title = "Existing task",
				Description = "description",
				Status = TaskItemStatus.InProgress
			};

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(_cancellationToken);

			var service = CreateService(dbContext);

			var result = await service.GetByIdAsync(task.Id, _cancellationToken);

			Assert.NotNull(result);
			Assert.Equal(task.Id, result.Id);
			Assert.Equal(task.Title, result.Title);
			Assert.Equal(task.Description, result.Description);
			Assert.Equal(task.Status, result.Status);
		}

		[Fact]
		public async Task GetByIdAsync_WhenTaskDoesNotExist_ShouldReturnNull()
		{
			await using var dbContext = CreateDbContext();
			var service = CreateService(dbContext);

			var result = await service.GetByIdAsync(Guid.NewGuid(), _cancellationToken);

			Assert.Null(result);
		}

		[Fact]
		public async Task GetAllAsync_ShouldReturnAllTasks()
		{
			await using var dbContext = CreateDbContext();

			dbContext.Tasks.AddRange(
				new TaskItem { Title = "t1", Status = TaskItemStatus.New },
				new TaskItem { Title = "t2", Status = TaskItemStatus.Completed }
			);

			await dbContext.SaveChangesAsync(_cancellationToken);
			var service = CreateService(dbContext);

			var result = await service.GetAllAsync(_cancellationToken);

			Assert.Equal(2, result.Count);
			Assert.Contains(result, x => x.Title == "t1");
			Assert.Contains(result, x => x.Title == "t2");
		}

		[Fact]
		public async Task GetAllAsync_WhenNoTasksExist_ShouldReturnEmptyCollection()
		{
			await using var dbContext = CreateDbContext();
			var service = CreateService(dbContext);

			var result = await service.GetAllAsync(_cancellationToken);

			Assert.Empty(result);
		}

		[Fact]
		public async Task UpdateAsync_WhenTaskExists_ShouldUpdateTask()
		{
			await using var dbContext = CreateDbContext();

			var task = new TaskItem { Title = "old title", Description = "old desc", Status = TaskItemStatus.InProgress };

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(_cancellationToken);

			var createdAt = task.CreatedAt;
			var oldUpdatedAt = task.UpdatedAt;

			var service = CreateService(dbContext);
			var request = new UpdateTaskRequest("new title", "new desc", TaskItemStatus.Completed);

			var result = await service.UpdateAsync(task.Id, request, _cancellationToken);

			Assert.NotNull(result);
			Assert.Equal("new title", result.Title);
			Assert.Equal("new desc", result.Description);
			Assert.Equal(TaskItemStatus.Completed, result.Status);
			Assert.Equal(createdAt, result.CreatedAt);
			Assert.True(result.UpdatedAt >= oldUpdatedAt);

			var entity = await dbContext.Tasks.SingleAsync(x => x.Id == task.Id, _cancellationToken);

			Assert.Equal("new title", entity.Title);
			Assert.Equal(TaskItemStatus.Completed, entity.Status);
		}

		[Fact]
		public async Task UpdateAsync_WhenTaskDoesNotExist_ShouldReturnNull()
		{
			await using var dbContext = CreateDbContext();
			var service = CreateService(dbContext);

			var request = new UpdateTaskRequest("new title", null, TaskItemStatus.Completed);

			var result = await service.UpdateAsync(Guid.NewGuid(), request, _cancellationToken);

			Assert.Null(result);
		}

		[Fact]
		public async Task UpdateAsync_ShouldPublishUpdatedEvent()
		{
			await using var dbContext = CreateDbContext();

			var task = new TaskItem { Title = "old title", Description = "old desc", Status = TaskItemStatus.InProgress };

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(_cancellationToken);

			var service = CreateService(dbContext);
			var request = new UpdateTaskRequest("new title", "new desc", TaskItemStatus.Completed);

			await service.UpdateAsync(task.Id, request, _cancellationToken);

			_eventProducerMock.Verify(
				x => x.PublishAsync(
					It.Is<TaskChangedEvent>(e =>
						e.TaskId == task.Id &&
						e.EventType == TaskEventType.Updated &&
						e.Title == "new title" &&
						e.Description == "new desc" &&
						e.Status == TaskItemStatus.Completed.ToString()),
					It.IsAny<CancellationToken>()),
				Times.Once
			);
		}

		[Fact]
		public async Task DeleteAsync_WhenTaskExists_ShouldDeleteTask()
		{
			await using var dbContext = CreateDbContext();

			var task = new TaskItem
			{
				Title = "task to delete",
				Status = TaskItemStatus.New
			};

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(_cancellationToken);

			var service = CreateService(dbContext);

			var result = await service.DeleteAsync(task.Id, _cancellationToken);
			Assert.True(result);

			var exists = await dbContext.Tasks.AnyAsync(x => x.Id == task.Id, _cancellationToken);
			Assert.False(exists);
		}

		[Fact]
		public async Task DeleteAsync_WhenTaskDoesNotExist_ShouldReturnFalse()
		{
			await using var dbContext = CreateDbContext();

			var service = CreateService(dbContext);

			var result = await service.DeleteAsync(Guid.NewGuid(), _cancellationToken);
			Assert.False(result);
		}

		[Fact]
		public async Task DeleteAsync_ShouldPublishDeletedEvent()
		{
			await using var dbContext = CreateDbContext();

			var task = new TaskItem
			{
				Title = "task to delete",
				Description = "desc",
				Status = TaskItemStatus.Completed
			};

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(_cancellationToken);

			var service = CreateService(dbContext);

			await service.DeleteAsync(task.Id, _cancellationToken);

			_eventProducerMock.Verify(
				x => x.PublishAsync(
					It.Is<TaskChangedEvent>(e =>
						e.TaskId == task.Id &&
						e.EventType == TaskEventType.Deleted &&
						e.Title == "task to delete" &&
						e.Description == "desc" &&
						e.Status == TaskItemStatus.Completed.ToString()),
					It.IsAny<CancellationToken>()),
				Times.Once
			);
		}

		[Fact]
		public async Task DeleteAsync_WhenTaskDoesNotExist_ShouldNotPublishEvents()
		{
			await using var dbContext = CreateDbContext();

			var service = CreateService(dbContext);

			await service.DeleteAsync(Guid.NewGuid(), _cancellationToken);

			_eventProducerMock.Verify(
				x => x.PublishAsync(
					It.IsAny<TaskChangedEvent>(),
					It.IsAny<CancellationToken>()),
				Times.Never
			);
		}
	}
}