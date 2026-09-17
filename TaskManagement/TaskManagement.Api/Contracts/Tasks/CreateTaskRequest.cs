using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// DTO для создания задачи
	/// </summary>
	public sealed record CreateTaskRequest(
		string Title,
		string? Description,
		TaskItemStatus Status = TaskItemStatus.New
	);
}
