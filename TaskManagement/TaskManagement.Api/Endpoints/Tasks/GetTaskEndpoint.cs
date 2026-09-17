using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class GetTaskEndpoint
	{
		public static async Task<Results<Ok<TaskResponse>, NotFound>> HandleAsync(Guid id, ITaskService taskService, CancellationToken cancellationToken)
		{
			var task = await taskService.GetByIdAsync(id, cancellationToken);
			return task is null 
				? TypedResults.NotFound() 
				: TypedResults.Ok(task);
		}
	}
}
