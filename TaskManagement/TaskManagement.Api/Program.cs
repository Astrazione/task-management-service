using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Clients.TaskAudit;
using TaskManagement.Api.Data;
using TaskManagement.Api.Endpoints.Tasks;
using TaskManagement.Api.Messaging;
using TaskManagement.Api.Services;
using TaskManagement.Contracts.Grpc;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskDbContext>(opitons =>
{
	opitons.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.Configure<KafkaOptions>(
	builder.Configuration.GetSection(KafkaOptions.SectionName));

builder.Services.AddGrpcClient<TaskAudit.TaskAuditClient>(options =>
{
	options.Address = new Uri(builder.Configuration["Grpc:TaskAuditUrl"]!);
});

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskAuditClient, GrpcTaskAuditClient>();
builder.Services.AddSingleton<ITaskEventProducer, TaskEventProducer>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTaskEndpoints();

await using (var scope = app.Services.CreateAsyncScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

	await dbContext.Database.MigrateAsync();
}

app.Run();
