using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Validation;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class TaskEndpoints
	{
		public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
		{
			var group = endpoints
				.MapGroup("/api/tasks")
				.WithTags("Tasks");

			group.MapPost("/", CreateTaskEndpoint.HandleAsync)
				.WithName("CreateTask")
				.WithSummary("Создать задачу")
				.WithDescription("Создаёт новую задачу и возвращает её")
				.ProducesValidationProblem()
				.AddEndpointFilter<ValidationFilter<CreateTaskRequest>>();

			group.MapGet("/", GetAllTasksEndpoint.HandleAsync)
				.WithName("GetllTasks")
				.WithSummary("Получить все задачи")
				.WithDescription("Возвращает все существующие задачи");

			group.MapGet("/{id:guid}", GetTaskEndpoint.HandleAsync)
				.WithName("GetTaskById")
				.WithSummary("Получить задачу по идентификатору")
				.WithDescription("Возвращает соотвутствующую идентификатору задачу");

			group.MapPut("/{id:guid}", UpdateTaskEndpoint.HandleAsync)
				.WithName("UpdateTask")
				.WithSummary("Обновить задачу")
				.WithDescription("Изменяет параметры задачи на новые значения")
				.ProducesValidationProblem()
				.AddEndpointFilter<ValidationFilter<UpdateTaskRequest>>();

			group.MapDelete("/{id:guid}", DeleteTaskEndpoint.HandleAsync)
				.WithName("DeleteTask")
				.WithSummary("Удалить задачу");

			return endpoints;
		}
	}
}
