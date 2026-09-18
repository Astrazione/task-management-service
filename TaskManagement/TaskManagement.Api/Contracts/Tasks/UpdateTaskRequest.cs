using System.ComponentModel.DataAnnotations;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// Запрос на обновление записи
	/// </summary>
	/// <param name="Title">Название задачи</param>
	/// <param name="Description">Описание задачи</param>
	/// <param name="Status">Статус задачи</param>
	public sealed record UpdateTaskRequest(
		[property: Required]
		[property: StringLength(200)]
		string Title,

		[property: StringLength(2000)]
		string? Description,

		[property: Range(0, 2)]
		TaskItemStatus Status
	);
}
