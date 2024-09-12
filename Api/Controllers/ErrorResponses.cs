using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public static class ErrorResponses
{
	private static ProblemDetails GetErrorResponse(string message, HttpStatusCode code)
		=> TypedResults.Problem(message, statusCode: (int)code).ProblemDetails;

	private static ProblemDetails GetErrorResponse(string title, string message, HttpStatusCode code)
		=> TypedResults.Problem(message, title: title, statusCode: (int)code).ProblemDetails;

	public static BadRequest<ProblemDetails> BadRequest(string message)
		=> TypedResults.BadRequest(GetErrorResponse(message, HttpStatusCode.BadRequest));

	public static Conflict<ProblemDetails> Conflict(string message)
		=> TypedResults.Conflict(GetErrorResponse(message, HttpStatusCode.Conflict));

	public static ProblemHttpResult Unauthorized(string title, string message)
		=> TypedResults.Problem(GetErrorResponse(title, message, HttpStatusCode.Unauthorized));

	public static NotFound<ProblemDetails> NotFound(string message)
		=> TypedResults.NotFound(GetErrorResponse(message, HttpStatusCode.NotFound));
}
