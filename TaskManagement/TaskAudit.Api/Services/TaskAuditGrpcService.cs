using Grpc.Core;
using TaskManagement.Api.Contracts.Grpc;

using TaskAuditGrpc = TaskManagement.Api.Contracts.Grpc.TaskAudit;

namespace TaskAudit.Api.Services
{
	public class TaskAuditGrpcService(ILogger<TaskAuditGrpcService> _logger) : TaskAuditGrpc.TaskAuditBase
	{
		public override Task<TaskChangeReply> LogChange(
			TaskChangeRequest request,
			ServerCallContext context)
		{
			_logger.LogInformation(
				"Received event {EventId} via gRPC, task: {TaskId}, task title: {Title}, task status: {Status}, event type: {EventKind}",
				request.EventId,
				request.TaskId,
				request.Title,
				request.Status,
				request.EventKind
			);

			return Task.FromResult(new TaskChangeReply());
		}
	}
}
