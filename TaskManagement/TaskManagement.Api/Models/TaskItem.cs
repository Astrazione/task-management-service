using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagement.Api.Models
{
	public sealed class TaskItem
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();

		[Required]
		[MaxLength(200)]
		[Column(TypeName = "nvarchar(200)")]
		public string Title { get; set; } = null!;

		[MaxLength(2000)]
		[Column(TypeName = "nvarchar(2000)")]
		public string? Description { get; set; }

		[Required]
		public TaskItemStatus Status { get; set; } = TaskItemStatus.New;

		[Required]
		public DateTimeOffset CreatedAt { get; set; }

		[Required]
		public DateTimeOffset UpdatedAt { get; set; }
	}
}
