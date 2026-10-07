using TaskManagement.Api.Contracts.Events;

namespace TaskManagement.Api.Messaging
{
	public interface ITaskEventProducer
	{
		Task PublishAsync(TaskChangedEvent taskEvent, CancellationToken cancellationToken);
	}
}
