using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TaskManagement.Contracts.Events;

namespace TaskManagement.Api.Messaging
{
	public sealed class TaskEventProducer : ITaskEventProducer, IDisposable
	{
		private readonly KafkaOptions _options;
		private readonly IProducer<string, string> _producer;

		public TaskEventProducer(IOptions<KafkaOptions> options)
		{
			_options = options.Value;

			var config = new ProducerConfig
			{
				BootstrapServers = _options.BootstrapServers,
				EnableIdempotence = true,
				Acks = Acks.All
			};

			_producer = new ProducerBuilder<string, string>(config).Build();
		}

		public async Task PublishAsync(TaskChangedEvent taskEvent, CancellationToken cancellationToken)
		{
			var message = new Message<string, string>
			{
				Key = taskEvent.TaskId.ToString(),
				Value = JsonSerializer.Serialize(taskEvent)
			};

			await _producer.ProduceAsync(
				_options.Topic,
				message,
				cancellationToken
			);
		}

		public void Dispose() => _producer.Dispose();
	}
}
