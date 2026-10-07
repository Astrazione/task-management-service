using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TaskManagement.Api.Contracts.Events;

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
				EnableIdempotence = _options.EnableIdempotence,
				Acks = _options.Acks
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
