using System.Security.Claims;
using Api.Extensions;
using Api.Requests;
using Database.Contexts;
using Database.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Task = Database.Models.Task;
using TaskSerializationContext = Api.Serializers.TaskSerializationContext;

namespace Api.Controllers;

public static class TasksController
{
	public static async ValueTask<Results<BadRequest, Created<TaskRequest>>> CreateTask(
			[FromBody]     TaskRequest  request,
			[FromServices] TasksContext tasksDb,
			HttpContext                 context,
			ClaimsPrincipal             userInfo,
			CancellationToken           token
	)
	{
		var task = request.TaskFromRequest();
		task.UserId = userInfo.GetUserId();
		var result = await tasksDb.CreateTask(task, token);
		return result ? TypedResults.Created(context.Request.Path, request) : TypedResults.BadRequest();
	}

	public static async ValueTask<JsonHttpResult<Task[]>> GetTasks(
			[FromQuery]                          Status?      status,
			[FromQuery]                          Priority?    priority,
			[FromQuery]                          DateTime?    dueDate,
			[FromQuery(Name = "p")]              ushort?      page,
			[FromServices]                       TasksContext tasksDb,
			[FromHeader(Name = "Authorization")] string       authorization,
			ClaimsPrincipal                                   userInfo,
			CancellationToken                                 token
	)
	{
		var userId = userInfo.GetUserId();
		var tasks  = await tasksDb.GetTasks(userId, page ?? 1, status, priority, dueDate, token);
		return TypedResults.Json(tasks, TaskSerializationContext.Default);
	}

	public static async ValueTask<Results<NotFound, JsonHttpResult<Task>>> GetTask(
			Guid                        id,
			[FromServices] TasksContext tasksDb,
			ClaimsPrincipal             userInfo,
			CancellationToken           token
	)
	{
		var userId = userInfo.GetUserId();
		var task   = await tasksDb.FindTask(userId, id, token);
		return task is null ? TypedResults.NotFound() : TypedResults.Json(task, TaskSerializationContext.Default);
	}

	public static async ValueTask<Results<BadRequest, NotFound, Ok<Task>>> UpdateTask(
			Guid                        id,
			[FromBody]     TaskRequest  request,
			[FromServices] TasksContext tasksDb,
			ClaimsPrincipal             userInfo,
			CancellationToken           token
	)
	{
		var task = request.TaskFromRequest();
		task.Id               = id;
		task.UserId           = userInfo.GetUserId();
		var (result, updated) = await tasksDb.UpdateTask(task, token);

		return result switch
		{
			-1 => TypedResults.NotFound(),
			1  => TypedResults.Ok(updated),
			_  => TypedResults.BadRequest()
		};
	}

	public static async ValueTask<Results<BadRequest, NotFound, Ok>> DeleteTask(
			Guid                        id,
			[FromServices] TasksContext tasksDb,
			ClaimsPrincipal             userInfo,
			CancellationToken           token
	)
	{
		var userId = userInfo.GetUserId();
		var result = await tasksDb.DeleteTask(userId, id, token);

		return result switch
		{
			-1 => TypedResults.NotFound(),
			1  => TypedResults.Ok(),
			_  => TypedResults.BadRequest()
		};
	}
}
