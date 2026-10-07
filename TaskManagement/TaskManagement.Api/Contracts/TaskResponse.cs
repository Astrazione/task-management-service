using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts
{
	/// <summary>
	/// Response containing task details
	/// </summary>
	/// <param name="Id">Task unique identifier</param>
	/// <param name="Title">Task title</param>
	/// <param name="Description">Task description</param>
	/// <param name="Status">Task status</param>
	/// <param name="CreatedAt">Date and time when the task was created</param>
	/// <param name="UpdatedAt">Date and time when the task wa last updated</param>
	public sealed record TaskResponse(
		Guid Id,
		string Title,
		string? Description,
		TaskItemStatus Status,
		DateTimeOffset CreatedAt,
		DateTimeOffset UpdatedAt
	);
}
