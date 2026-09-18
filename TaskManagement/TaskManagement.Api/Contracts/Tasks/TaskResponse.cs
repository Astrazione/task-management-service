using TaskManagement.Api.Models;

namespace TaskManagement.Api.Contracts.Tasks
{
	/// <summary>
	/// Ответ на получение записи задачи
	/// </summary>
	/// <param name="Id">Уникальный идентификатор задачи</param>
	/// <param name="Title">Название задачи</param>
	/// <param name="Description">Описание задачи</param>
	/// <param name="Status">Статус задачи</param>
	/// <param name="CreatedAt">Дата создания задачи</param>
	/// <param name="UpdatedAt">Дата последнего изменения задачи</param>
	public sealed record TaskResponse(
		Guid Id,
		string Title,
		string? Description,
		TaskItemStatus Status,
		DateTimeOffset CreatedAt,
		DateTimeOffset UpdatedAt
	);
}
