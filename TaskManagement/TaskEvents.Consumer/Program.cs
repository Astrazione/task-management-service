using TaskEvents.Consumer;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TaskEventConsumer>();

var host = builder.Build();
host.Run();
