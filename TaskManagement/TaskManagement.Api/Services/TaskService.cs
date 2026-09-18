using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Clients.TaskAudit;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Data;
using TaskManagement.Api.Messaging;
using TaskManagement.Api.Models;
using TaskManagement.Contracts.Events;

namespace TaskManagement.Api.Services
{
	public sealed class TaskService(
		TaskDbContext dbContext,
		ITaskEventProducer eventProducer,
		ITaskAuditClient auditClient,
		ILogger<TaskService> logger) : ITaskService
	{
		public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
		{
			var task = new TaskItem
			{
				Title = request.Title,
				Description = request.Description,
				Status = request.Status
			};

			dbContext.Tasks.Add(task);
			await dbContext.SaveChangesAsync(cancellationToken);

			var taskEvent = CreateEvent(task, TaskEventType.Created);
			await auditClient.LogAsync(taskEvent, cancellationToken);
			await eventProducer.PublishAsync(taskEvent, cancellationToken);

			return MapToResponse(task);
		}

		public async Task<TaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
		{
			var task = await dbContext.Tasks
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return task is null ? null : MapToResponse(task);
		}

		public async Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(CancellationToken cancellationToken)
		{
			var tasks = await dbContext.Tasks.AsNoTracking().ToListAsync(cancellationToken);

			return [.. tasks.Select(MapToResponse)];
		}

		public async Task<TaskResponse?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
		{
			var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (task is null) return null;
		
			task.Title = request.Title;
			task.Description = request.Description;
			task.Status = request.Status;

			await dbContext.SaveChangesAsync(cancellationToken);

			var taskEvent = CreateEvent(task, TaskEventType.Updated);
			await auditClient.LogAsync(taskEvent, cancellationToken);
			await eventProducer.PublishAsync(taskEvent, cancellationToken);

			return MapToResponse(task);
		}

		public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
		{
			var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			if (task is null) return false;

			var taskEvent = CreateEvent(task, TaskEventType.Deleted);

			dbContext.Tasks.Remove(task);
			await dbContext.SaveChangesAsync(cancellationToken);

			await auditClient.LogAsync(taskEvent, cancellationToken);
			await eventProducer.PublishAsync(taskEvent, cancellationToken);

			return true;
		}

		private static TaskResponse MapToResponse(TaskItem task) =>
			new(
				task.Id,
				task.Title,
				task.Description,
				task.Status,
				task.CreatedAt,
				task.UpdatedAt
			);

		private static TaskChangedEvent CreateEvent(TaskItem taskItem, TaskEventType eventType) =>
			new(
				EventId: Guid.NewGuid(),
				TaskId: taskItem.Id,
				EventType: eventType,
				Title: taskItem.Title,
				Description: taskItem.Description,
				Status: (int)taskItem.Status,
				HappenedAt: DateTimeOffset.UtcNow
			);
	}
}
