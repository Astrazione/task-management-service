using TaskManagement.Contracts.Events;

namespace TaskManagement.Api.Clients.TaskAudit
{
	public interface ITaskAuditClient
	{
		Task LogAsync(
			TaskChangedEvent taskEvent,
			CancellationToken cancellationToken
		);
	}
}
