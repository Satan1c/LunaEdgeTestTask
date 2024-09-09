using System.ComponentModel.DataAnnotations;
using Api.Controllers;
using Api.Extensions;
using Api.Requests;

namespace Api.Filters;

public class LoginFormValidation : IEndpointFilter
{
	private static readonly EmailAddressAttribute s_emailAddress = new();

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var request = context.Arguments.OfType<ILoginRequest>().FirstOrDefault();

		if (request!.Login.IsNonAscii() || (!s_emailAddress.IsValid(request.Login) && request.Login is { Length: < 3 or > 15 }))
			return ErrorResponses.BadRequest("Invalid login.");

		if (request.Password.IsNonAscii() || request.Password is { Length: < 3 or > 20 })
			return ErrorResponses.BadRequest("Invalid password.");

		return await next(context);
	}
}
