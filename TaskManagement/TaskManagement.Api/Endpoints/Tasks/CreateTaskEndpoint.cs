using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class CreateTaskEndpoint
	{
		public static async Task<Created<TaskResponse>> HandleAsync(CreateTaskRequest request, ITaskService taskService, CancellationToken cancellationToken)
		{
			var task = await taskService.CreateAsync(request, cancellationToken);
			return TypedResults.Created($"/api/tasks/{task.Id}", task);
		}
	}
}
