using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts
{
	/// <summary>
	/// Request to create a task
	/// </summary>
	/// <param name="Title">Task title</param>
	/// <param name="Description">Task description</param>
	/// <param name="Status">Task status</param>
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
