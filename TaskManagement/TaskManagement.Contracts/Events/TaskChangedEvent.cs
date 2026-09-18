namespace TaskManagement.Contracts.Events
{
	public sealed record TaskChangedEvent(
		Guid EventId,
		Guid TaskId,
		TaskEventType EventType,
		string Title,
		string? Description,
		int Status,
		DateTimeOffset HappenedAt
	);
}
