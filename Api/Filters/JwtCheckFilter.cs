using System.IdentityModel.Tokens.Jwt;
using Api.Controllers;
using Api.Extensions;
using Database.Contexts;

namespace Api.Filters;

public class JwtCheckFilter(UsersContext usersdb) : IEndpointFilter
{
	private readonly UsersContext m_usersdb = usersdb;

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerValue)) return await next(context);

		var authorization = headerValue[0]?.Split(' ') ?? [string.Empty, string.Empty];

		if (authorization[0] != "Bearer")
			return Results.Problem(statusCode: 400, title: "Invalid Authorization", detail: "The authorization header is invalid.");

		var token  = context.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value ?? string.Empty;
		var userId = string.IsNullOrEmpty(token) ? Guid.Empty : Guid.Parse(token);

		if (!await m_usersdb.IsTokenExist(userId, authorization[1], context.GetToken()))
			return ErrorResponses.Unauthorized("Invalid token", "Invalid token.");

		return await next(context);
	}
}
