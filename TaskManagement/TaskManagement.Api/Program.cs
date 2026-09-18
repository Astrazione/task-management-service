using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;
using TaskManagement.Api.Clients.TaskAudit;
using TaskManagement.Api.Data;
using TaskManagement.Api.Endpoints.Tasks;
using TaskManagement.Api.Messaging;
using TaskManagement.Api.Services;
using TaskManagement.Contracts.Grpc;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// API
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

	options.IncludeXmlComments(xmlPath);
});

services.ConfigureHttpJsonOptions(options =>
{
	options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// DB
builder.Services.AddDbContext<TaskDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

// Kafka
builder.Services.Configure<KafkaOptions>(
	builder.Configuration.GetSection(KafkaOptions.SectionName));

builder.Services.AddSingleton<ITaskEventProducer, TaskEventProducer>();

// gRPC
builder.Services.AddGrpcClient<TaskAudit.TaskAuditClient>(options =>
{
	options.Address = new Uri(builder.Configuration["Grpc:TaskAuditUrl"]!);
});

builder.Services.AddScoped<ITaskAuditClient, GrpcTaskAuditClient>();

// Application
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
	try
	{
		await next(context);
	}
	catch (BadHttpRequestException)
	{
		context.Response.StatusCode = StatusCodes.Status400BadRequest;
		await context.Response.WriteAsJsonAsync(new
		{
			error = "Invalid request body. Check the provided values."
		});
	}
});

// Endpoints
app.MapTaskEndpoints();

// DB migrations
await using (var scope = app.Services.CreateAsyncScope())
{
	var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();

	await dbContext.Database.MigrateAsync();
}

app.Run();
