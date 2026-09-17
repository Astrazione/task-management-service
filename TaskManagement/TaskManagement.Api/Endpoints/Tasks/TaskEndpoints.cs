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
				.AddEndpointFilter<ValidationFilter<CreateTaskRequest>>();

			group.MapGet("/", GetAllTasksEndpoint.HandleAsync);

			group.MapGet("/{id:guid}", GetTaskEndpoint.HandleAsync);

			group.MapPut("/{id:guid}", UpdateTaskEndpoint.HandleAsync)
				.AddEndpointFilter<ValidationFilter<UpdateTaskRequest>>();

			group.MapDelete("/{id:guid}", DeleteTaskEndpoint.HandleAsync);

			return endpoints;
		}
	}
}
