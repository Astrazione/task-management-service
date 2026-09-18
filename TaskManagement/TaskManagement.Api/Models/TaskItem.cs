using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Api.Models
{
	/// <summary>
	/// Задача пользователя
	/// </summary>
	public sealed class TaskItem
	{
		/// <summary>
		/// Уникальный идентификатор задачи
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Название задачи
		/// </summary>
		[Required]
		[MaxLength(200)]
		[Column(TypeName = "nvarchar(200)")]
		public string Title { get; set; } = null!;

		/// <summary>
		/// Описание задачи
		/// </summary>
		[MaxLength(2000)]
		[Column(TypeName = "nvarchar(2000)")]
		public string? Description { get; set; }

		/// <summary>
		/// Статус задачи
		/// </summary>
		[Required]
		public TaskItemStatus Status { get; set; } = TaskItemStatus.New;

		/// <summary>
		/// Дата создания задачи
		/// </summary>
		[Required]
		public DateTimeOffset CreatedAt { get; set; }

		/// <summary>
		/// Дата последнего изменения задачи
		/// </summary>
		[Required]
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
