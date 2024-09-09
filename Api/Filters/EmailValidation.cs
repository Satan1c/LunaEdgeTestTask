using System.ComponentModel.DataAnnotations;
using Api.Controllers;
using Api.Requests;

namespace Api.Filters;

public class EmailValidation : IEndpointFilter
{
	private static readonly EmailAddressAttribute s_emailAddress = new();

	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var request = context.Arguments.OfType<IEmailRequest>().FirstOrDefault();
		if (!s_emailAddress.IsValid(request!.Email)) return ErrorResponses.BadRequest("Wrong Email format.");

		return await next(context);
	}
}
