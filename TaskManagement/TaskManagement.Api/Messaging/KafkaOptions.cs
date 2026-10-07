using Confluent.Kafka;

namespace TaskManagement.Api.Messaging
{
	public sealed class KafkaOptions
	{
		public const string SectionName = "Kafka";

		public string BootstrapServers { get; init; } = null!;
		public string Topic { get; init; } = null!;
		public bool EnableIdempotence { get; init; }
		public Acks Acks { get; init; }
	}
}
