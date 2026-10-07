using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts;
using TaskManagement.Api.Services;
using TaskManagement.Api.Validation;

namespace TaskManagement.Api.Endpoints
{
	public static class TaskEndpoints
	{
		public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
		{
			var group = endpoints
				.MapGroup("/api/tasks")
				.WithTags("Tasks");

			group.MapPost("/", CreateTask)
				.WithName(nameof(CreateTask))
				.WithSummary("Create task")
				.ProducesValidationProblem()
				.AddEndpointFilter<ValidationFilter<CreateTaskRequest>>();

			group.MapGet("/", GetAllTasks)
				.WithName(nameof(GetAllTasks))
				.WithSummary("Get all tasks");

			group.MapGet("/{id:guid}", GetTaskById)
				.WithName(nameof(GetTaskById))
				.WithSummary("Get task by id");

			group.MapPut("/{id:guid}", UpdateTask)
				.WithName(nameof(UpdateTask))
				.WithSummary("Update task")
				.ProducesValidationProblem()
				.AddEndpointFilter<ValidationFilter<UpdateTaskRequest>>();

			group.MapDelete("/{id:guid}", DeleteTask)
				.WithName(nameof(DeleteTask))
				.WithSummary("Delete task");

			return endpoints;
		}

		private static async Task<Created<TaskResponse>> CreateTask(CreateTaskRequest request, ITaskService taskService, CancellationToken cancellationToken)
		{
			var task = await taskService.CreateAsync(request, cancellationToken);
			return TypedResults.Created($"/api/tasks/{task.Id}", task);
		}

		private static async Task<Ok<IReadOnlyCollection<TaskResponse>>> GetAllTasks(ITaskService taskService, CancellationToken cancellationToken)
		{
			var tasks = await taskService.GetAllAsync(cancellationToken);
			return TypedResults.Ok(tasks);
		}

		private static async Task<Results<Ok<TaskResponse>, NotFound>> GetTaskById(Guid id, ITaskService taskService, CancellationToken cancellationToken)
		{
			var task = await taskService.GetByIdAsync(id, cancellationToken);
			return task is null
				? TypedResults.NotFound()
				: TypedResults.Ok(task);
		}

		private static async Task<Results<Ok<TaskResponse>, NotFound>> UpdateTask(Guid id, UpdateTaskRequest request, ITaskService taskService,	CancellationToken cancellationToken)
		{
			var task = await taskService.UpdateAsync(id, request, cancellationToken);
			return task is null
				? TypedResults.NotFound()
				: TypedResults.Ok(task);
		}

		private static async Task<Results<NoContent, NotFound>> DeleteTask(Guid id, ITaskService taskService, CancellationToken cancellationToken)
		{
			var taskDeleted = await taskService.DeleteAsync(id, cancellationToken);
			return taskDeleted
				? TypedResults.NoContent()
				: TypedResults.NotFound();
		}
	}
}
