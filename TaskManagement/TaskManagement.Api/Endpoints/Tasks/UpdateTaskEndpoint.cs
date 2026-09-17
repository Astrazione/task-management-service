using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class UpdateTaskEndpoint
	{
		public static async Task<Results<Ok<TaskResponse>, NotFound>> HandleAsync(
			Guid id, 
			UpdateTaskRequest request, 
			ITaskService taskService,
			CancellationToken cancellationToken)
		{
			var task = await taskService.UpdateAsync(id, request, cancellationToken);

			return task is null
				? TypedResults.NotFound()
				: TypedResults.Ok(task);
		}
	}
}
