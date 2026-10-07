using Google.Protobuf.WellKnownTypes;
using TaskManagement.Api.Contracts.Events;
using TaskManagement.Api.Contracts.Grpc;
using TaskAuditClient = TaskManagement.Api.Contracts.Grpc.TaskAudit.TaskAuditClient;


namespace TaskManagement.Api.Clients
{
	public sealed class GrpcTaskAuditClient(TaskAuditClient client) : ITaskAuditClient
	{
		public async Task LogAsync(TaskChangedEvent taskEvent, CancellationToken cancellationToken)
		{
			var request = new TaskChangeRequest
			{
				EventId = taskEvent.EventId.ToString(),
				TaskId = taskEvent.TaskId.ToString(),

				EventKind = taskEvent.EventType switch
				{
					TaskEventType.Created => TaskEventKind.Created,
					TaskEventType.Updated => TaskEventKind.Updated,
					TaskEventType.Deleted => TaskEventKind.Deleted,
					_ => TaskEventKind.Uncpecified,
				},

				Title = taskEvent.Title,
				Description = taskEvent.Description ?? string.Empty,
				Status = taskEvent.Status,
				HappenedAt = Timestamp.FromDateTimeOffset(taskEvent.HappenedAt),
			};

			await client.LogChangeAsync(request, cancellationToken: cancellationToken);
		}
	}
}
