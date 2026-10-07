using Confluent.Kafka;
using System.Text.Json;
using TaskManagement.Api.Contracts.Events;

namespace TaskEvents.Consumer
{
	public class TaskEventConsumer(IConfiguration configuration, ILogger<TaskEventConsumer> logger) : BackgroundService
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly ILogger<TaskEventConsumer> _logger = logger;

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Yield();
			ConsumerConfig config = new()
            {
				BootstrapServers = _configuration["Kafka:BootstrapServers"],
				GroupId = _configuration["Kafka:GroupId"],
				AutoOffsetReset =  _configuration.GetValue<AutoOffsetReset>("Kafka:AutoOffsetReset"),
				EnableAutoCommit = _configuration.GetValue<bool>("Kafka:EnableAutoCommit")
			};

			using var consumer = new ConsumerBuilder<string, string>(config).Build();
			consumer.Subscribe(_configuration["Kafka:Topic"]);

			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					var result = consumer.Consume(stoppingToken);

					var taskEvent = JsonSerializer.Deserialize<TaskChangedEvent>(result.Message.Value);

					if (taskEvent is null) continue;

					_logger.LogInformation(
						"Received event {EventId} via Kafka, task: {TaskId}, task title: {Title}, task status: {Status}, event type: {EventKind}",
						taskEvent.EventId,
						taskEvent.TaskId,
						taskEvent.Title,
						taskEvent.Status,
						taskEvent.EventType
					);

					consumer.Commit();
				}
			}
			catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
			{
				// normal application shutdown
			}
			finally
			{
				consumer.Close();
			}
		}
	}
}
