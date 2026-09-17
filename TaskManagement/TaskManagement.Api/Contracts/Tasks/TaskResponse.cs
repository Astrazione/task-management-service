using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// DTO для операций чтения задачи
	/// </summary>
	public sealed record TaskResponse(
		Guid Id,
		string Title,
		string? Description,
		TaskItemStatus Status,
		DateTimeOffset CreatedAt,
		DateTimeOffset UpdatedAt
	);
}
