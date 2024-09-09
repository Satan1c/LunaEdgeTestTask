using System.ComponentModel.DataAnnotations;
using Api.Extensions;
using Api.Requests;
using Database.Contexts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public static class UsersController
{
	private static readonly EmailAddressAttribute s_emailAddress = new();

	public static async ValueTask<Results<StatusCodeHttpResult, Conflict<ProblemDetails>, Created<RegisterRequest>>> CreateUser(
			[FromBody]     RegisterRequest request,
			[FromServices] UsersContext    usersDb,
			HttpContext                    context,
			CancellationToken              token
	)
	{
		if (await usersDb.IsUserExistsByEmail(request.Email, token)) return ErrorResponses.Conflict("User with this email already exists.");
		if (await usersDb.IsUserExistsByUserName(request.Login, token)) return ErrorResponses.Conflict("Username is already taken.");

		var isCreated = await usersDb.CreateUser(request.UserFromRequest(), token);
		return isCreated ? TypedResults.Created(context.Request.Path, request) : TypedResults.StatusCode(500);
	}

	public static async ValueTask<Results<StatusCodeHttpResult, NotFound<ProblemDetails>, Ok<string>>> Login(
			[FromBody]     LoginRequest request,
			[FromServices] UsersContext usersDb,
			CancellationToken           token
	)
	{
		if (!await usersDb.IsUserExists(request.Login, token)) return ErrorResponses.NotFound("User does not exist.");

		var passwordHash = JwtGenerator.HashPassword(request.Password);
		var user         = await usersDb.FindUserByLogin(request.Login, passwordHash, token);

		var tokenInfo = user!.TokenInfoFromUser();
		var isSuccess = await usersDb.CreateOrUpdate(tokenInfo, token);

		return isSuccess ? TypedResults.Ok(tokenInfo.Token) : TypedResults.StatusCode(500);
	}
}
