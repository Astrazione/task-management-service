using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// DTO для обновления задачи
	/// </summary>
	public sealed record UpdateTaskRequest(
		string Title,
		string? Description,
		TaskItemStatus Status
	);
}
