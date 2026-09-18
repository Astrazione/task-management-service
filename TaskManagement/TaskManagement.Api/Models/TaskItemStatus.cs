using System.Text.Json.Serialization;

namespace TaskManagement.Api.Models
{
	/// <summary>
	/// Статус задачи
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum TaskItemStatus
	{
		New,
		InProgress,
		Completed
	}
}
