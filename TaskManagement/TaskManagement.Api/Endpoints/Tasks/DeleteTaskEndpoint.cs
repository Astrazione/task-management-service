using Microsoft.AspNetCore.Http.HttpResults;
using TaskManagement.Api.Contracts.Tasks;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Endpoints.Tasks
{
	public static class DeleteTaskEndpoint
	{
		public static async Task<Results<NoContent, NotFound>> HandleAsync(Guid id, ITaskService taskService, CancellationToken cancellationToken)
		{
			var taskDeleted = await taskService.DeleteAsync(id, cancellationToken);
			return taskDeleted
				? TypedResults.NoContent()
				: TypedResults.NotFound();
		}
	}
}
