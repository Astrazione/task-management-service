using Grpc.Core;
using TaskManagement.Contracts.Grpc;

using TaskAuditGrpc = TaskManagement.Contracts.Grpc.TaskAudit;

namespace TaskAudit.Api.Services
{
	public class TaskAuditGrpcService(ILogger<TaskAuditGrpcService> _logger) : TaskAuditGrpc.TaskAuditBase
	{
		public override Task<TaskChangeReply> LogChange(
			TaskChangeRequest request,
			ServerCallContext context)
		{
			_logger.LogInformation(
				"Получено событие {EventId}, задача: {TaskId}, название задачи: {Title}, статус задачи: {Status}, тип события: {EventKind}",
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
