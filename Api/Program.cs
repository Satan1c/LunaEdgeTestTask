using System.Runtime;
using Api.Controllers;
using Api.Extensions;
using Api.Filters;
using Api.Serializers;
using Database;
using Serilog;

GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
GCSettings.LatencyMode                   = GCLatencyMode.Interactive;

Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
									  .WriteTo.Console(
													   outputTemplate:
													   "[{Timestamp:HH:mm:ss} {Level,4:u4}] {Message:lj}{NewLine}{Exception}"
													  )
									  .CreateLogger();

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddSerilog()
	   .ConfigureHttpJsonOptions(
								 options => { options.SerializerOptions.TypeInfoResolverChain.Insert(0, TaskSerializationContext.Default); }
								)
	   .AddDatabaseServices()
	   .AddAuth(builder);

/*builder.Services.AddAuthorizationBuilder()
	   .AddPolicy("test", policy => );*/

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

var usersApi = app.MapGroup("/users");

usersApi.MapPost("/register", UsersController.CreateUser)
		.AllowAnonymous()
		.AddEndpointFilter<EmailValidation>()
		.AddEndpointFilter<LoginFormValidation>();

usersApi.MapPost("/login", UsersController.Login).AllowAnonymous().AddEndpointFilter<LoginFormValidation>();

var tasksApi = app.MapGroup("/tasks");

tasksApi.MapPost("/", TasksController.CreateTask)
		.RequireAuthorization()
		.AddEndpointFilter<JwtCheckFilter>()
		.AddEndpointFilter<TaskRequestValidation>();

tasksApi.MapGet("/",          TasksController.GetTasks).RequireAuthorization().AddEndpointFilter<JwtCheckFilter>();
tasksApi.MapGet("/{id:guid}", TasksController.GetTask).RequireAuthorization().AddEndpointFilter<JwtCheckFilter>();

tasksApi.MapPut("/{id:guid}", TasksController.UpdateTask)
		.RequireAuthorization()
		.AddEndpointFilter<JwtCheckFilter>()
		.AddEndpointFilter<TaskRequestValidation>();

tasksApi.MapDelete("/{id:guid}", TasksController.DeleteTask).RequireAuthorization().AddEndpointFilter<JwtCheckFilter>();

app.Run(Environment.GetEnvironmentVariable("applicationUrl"));
