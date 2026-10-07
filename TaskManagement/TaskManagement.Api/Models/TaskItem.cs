using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Api.Models
{
	/// <summary>
	/// User task
	/// </summary>
	public sealed class TaskItem
	{
		/// <summary>
		/// Task unique identifier
		/// </summary>
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		/// <summary>
		/// Task title
		/// </summary>
		[Required]
		[MaxLength(200)]
		[Column(TypeName = "nvarchar(200)")]
		public string Title { get; set; } = null!;

		/// <summary>
		/// Task description
		/// </summary>
		[MaxLength(2000)]
		[Column(TypeName = "nvarchar(2000)")]
		public string? Description { get; set; }

		/// <summary>
		/// Task status
		/// </summary>
		[Required]
		public TaskItemStatus Status { get; set; } = TaskItemStatus.New;

		/// <summary>
		/// Date and time then the task was created
		/// </summary>
		[Required]
		public DateTimeOffset CreatedAt { get; set; }

		/// <summary>
		/// Date and time when the task was last updated
		/// </summary>
		[Required]
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
