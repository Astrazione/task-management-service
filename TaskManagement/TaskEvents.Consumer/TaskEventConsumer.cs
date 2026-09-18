using Confluent.Kafka;
using System.Text.Json;
using TaskManagement.Contracts.Events;

namespace TaskEvents.Consumer
{
	public class TaskEventConsumer : BackgroundService
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<TaskEventConsumer> _logger;

		public TaskEventConsumer(IConfiguration configuration, ILogger<TaskEventConsumer> logger)
		{
			_configuration = configuration;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			await Task.Yield();
			ConsumerConfig config = GetConsumerConfig();

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
						"Получено событие {EventId}, задача: {TaskId}, название задачи: {Title}, статус задачи: {Status}, тип события: {EventType}",
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
				//штатное завершение работы приложения
			}
			finally
			{
				consumer.Close();
			}
		}

		private ConsumerConfig GetConsumerConfig() =>
			new()
			{
				BootstrapServers = _configuration["Kafka:BootstrapServers"],
				GroupId = "task-events-consumer",
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = true
			};
	}
}
