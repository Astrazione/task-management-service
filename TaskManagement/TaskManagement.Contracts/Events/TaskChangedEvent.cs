using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
