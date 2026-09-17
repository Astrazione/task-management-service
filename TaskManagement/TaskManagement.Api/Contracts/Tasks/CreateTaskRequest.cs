using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// DTO для создания задачи
	/// </summary>
	public sealed record CreateTaskRequest(
		[property: Required]
		[property: StringLength(200)]
		string Title,

		[property: StringLength(2000)]
		string? Description,

		[property: Range(0, 2)]
		TaskItemStatus Status = TaskItemStatus.New
	);
}
