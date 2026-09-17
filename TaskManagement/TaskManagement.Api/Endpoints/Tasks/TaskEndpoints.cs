namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class TaskEndpoints
	{
		public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder endpoints)
		{
			var group = endpoints
				.MapGroup("/api/tasks")
				.WithTags("Tasks");

			group.MapPost("/", CreateTaskEndpoint.HandleAsync);
			group.MapGet("/", GetAllTasksEndpoint.HandleAsync);
			group.MapGet("/{id:guid}", GetTaskEndpoint.HandleAsync);
			group.MapPut("/{id:guid}", UpdateTaskEndpoint.HandleAsync);
			group.MapDelete("/{id:guid}", DeleteTaskEndpoint.HandleAsync);

			return endpoints;
		}
	}
}
