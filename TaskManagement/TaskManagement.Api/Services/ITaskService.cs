using TaskManagement.Api.Contracts.Tasks;

namespace TaskManagement.Api.Services
{
	public interface ITaskService
	{
		Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);

		Task<TaskResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

		Task<IReadOnlyCollection<TaskResponse>> GetAllAsync(CancellationToken cancellationToken);

		Task<TaskResponse?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken);

		Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
	}
}
