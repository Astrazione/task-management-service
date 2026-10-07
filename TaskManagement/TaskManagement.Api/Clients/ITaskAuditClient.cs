using TaskManagement.Api.Contracts.Events;

namespace TaskManagement.Api.Clients
{
	public interface ITaskAuditClient
	{
		Task LogAsync(
			TaskChangedEvent taskEvent,
			CancellationToken cancellationToken
		);
	}
}
