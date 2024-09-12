using System.Net;
using Api.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Test.Unit.Controllers;

public class ErrorResponsesTests
{
	[Theory]
	[InlineData("Test error message")]
	public void BadRequest_Return_BadRequest(string message)
	{
		// Act
		var response = ErrorResponses.BadRequest(message);

		// Assert
		response.Should().BeOfType<BadRequest<ProblemDetails>>();
		response.Value.Should().NotBeNull();
		response.Value?.Detail.Should().Be(message);
		response.Value?.Status.Should().Be((int)HttpStatusCode.BadRequest);
	}

	[Theory]
	[InlineData("Test error message")]
	public void Conflict_Return_Conflict(string message)
	{
		// Act
		var response = ErrorResponses.Conflict(message);

		// Assert
		response.Should().BeOfType<Conflict<ProblemDetails>>();
		response.Value.Should().NotBeNull();
		response.Value?.Detail.Should().Be(message);
		response.Value?.Status.Should().Be((int)HttpStatusCode.Conflict);
	}

	[Theory]
	[InlineData("Test error title", "Test error message")]
	public void Unauthorized_Return_ProblemHttpResult(string title, string message)
	{
		// Act
		var response = ErrorResponses.Unauthorized(title, message);

		// Assert
		response.Should().BeOfType<ProblemHttpResult>();
		response.ProblemDetails.Title.Should().Be(title);
		response.ProblemDetails.Detail.Should().Be(message);
		response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
	}

	[Theory]
	[InlineData("Test error message")]
	public void NotFound_Return_NotFound(string message)
	{
		// Act
		var response = ErrorResponses.NotFound(message);

		// Assert
		response.Should().BeOfType<NotFound<ProblemDetails>>();
		response.Value.Should().NotBeNull();
		response.Value?.Detail.Should().Be(message);
		response.Value?.Status.Should().Be((int)HttpStatusCode.NotFound);
	}
}
