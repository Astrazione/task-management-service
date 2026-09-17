using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class GetAllTasksEndpoint
	{
		public static async Task<Ok<IReadOnlyCollection<TaskResponse>>> HandleAsync(ITaskService taskService, CancellationToken cancellationToken)
		{
			var tasks = await taskService.GetAllAsync(cancellationToken);
			return TypedResults.Ok(tasks);
		}
	}
}
