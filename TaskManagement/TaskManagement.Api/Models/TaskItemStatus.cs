using System.Text.Json.Serialization;

namespace TaskManagement.Api.Models
{
	/// <summary>
	/// User task status
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum TaskItemStatus
	{
		New,
		InProgress,
		Completed
	}
}
