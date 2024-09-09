using Api.Controllers;
using Api.Extensions;
using Api.Requests;

namespace Api.Filters;

public class TaskRequestValidation : IEndpointFilter
{
	public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
	{
		var request = context.Arguments.OfType<TaskRequest>().FirstOrDefault();

		if (request == null) return await next(context);

		if (request.Title.IsNonAscii() || string.IsNullOrEmpty(request.Title.Trim()) || request.Title is { Length: < 3 or > 15 })
			return ErrorResponses.BadRequest("Invalid title.");

		if (request.DueDate is not null && request.DueDate <= DateTime.UtcNow) return ErrorResponses.BadRequest("Invalid due date.");

		return await next(context);
	}
}
