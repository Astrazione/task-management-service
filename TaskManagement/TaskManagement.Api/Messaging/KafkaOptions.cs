namespace TaskManagement.Api.Messaging
{
	public sealed class KafkaOptions
	{
		public const string SectionName = "Kafka";

		public string BootstrapServers { get; init; } = null!;

		public string Topic { get; init; } = "task-events";
	}
}
