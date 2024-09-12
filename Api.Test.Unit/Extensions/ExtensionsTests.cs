using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Api.Extensions;
using Api.Requests;
using Database.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Api.Test.Unit.Extensions;

public class ExtensionsTests
{
	[Theory]
	[InlineData("e↨4♪84V85704§4865]485Ï", true)]
	[InlineData("adfjhbsrgdfujkhlkgm",    false)]
	public void IsNonAscii_UnicodeAndClear_ReturnsValidResult(string input, bool expected)
	{
		// Act
		var result = input.IsNonAscii();

		// Assert
		result.Should().Be(expected);
	}

	[Theory]
	[InlineData("Test title", Status.Progress, Priority.High)]
	public void TaskFromRequest_AnyInput_ReturnsSameAsInput(string title, Status status, Priority priority)
	{
		// Arrange
		var request = new TaskRequest { Title = title, Status = status, Priority = priority };

		// Act
		var result = request.TaskFromRequest();

		// Assert
		result.Title.Should().Be(title);
		result.Status.Should().Be(status);
		result.Priority.Should().Be(priority);
	}

	[Theory]
	[InlineData("example@ukr.net", "Example login", "my best password ever")]
	public void UserFromRequest_AnyInput_ReturnsSameAsInput(string email, string login, string password)
	{
		// Arrange
		var passwordHash = JwtGenerator.HashPassword(password);
		var request      = new RegisterRequest { Email = email, Login = login, Password = password };

		// Act
		var result = request.UserFromRequest();

		// Assert
		result.Email.Should().Be(email);
		result.Username.Should().Be(login);
		result.PasswordHash.Should().Be(passwordHash);
	}

	[Theory]
	[InlineData("10d021ad-5fb5-458a-9159-111122c97731")]
	public void TokenInfoFromUser_ValidGuidInput_GeneratesValidTokenInfo(string guidString)
	{
		// Arrange
		var user  = new User { Id = Guid.Parse(guidString) };
		var token = JwtGenerator.GenerateToken(user.Id.ToString());

		// Act
		var result = user.TokenInfoFromUser();

		// Assert
		//TODO: inconsistent tests cuz of DateTime.UtcNow
		result.Token.Should().BeOfType<string>().And.NotBeNull().And.NotBeEmpty();
		result.UserId.Should().Be(user.Id);
	}

	[Fact]
	public void GetToken_ManyElements_FindCorrectToken()
	{
		// Arrange
		var args    = new object?[] { string.Empty, null, new CancellationToken(true) };
		var context = Substitute.For<EndpointFilterInvocationContext>();
		context.Arguments.ReturnsForAnyArgs(args);

		// Act
		var result = context.GetToken();

		// Assert
		result.Should().BeOfType<CancellationToken>();
		result.IsCancellationRequested.Should().BeTrue();
	}

	[Theory]
	[InlineData("10d021ad-5fb5-458a-9159-111122c97731")]
	public void GetUserId_ValidClaimsInput_GeneratesValidGuid(string guidString)
	{
		// Arrange
		var userId   = Guid.Parse(guidString);
		var claim    = new Claim(JwtRegisteredClaimNames.Jti, guidString);
		var identity = new ClaimsIdentity([claim]);
		var claims   = Substitute.For<ClaimsPrincipal>();
		claims.Claims.Returns([claim]);
		claims.Identities.Returns([identity]);
		claims.FindFirst(JwtRegisteredClaimNames.Jti).ReturnsForAnyArgs(claim);

		// Act
		var result = claims.GetUserId();

		// Assert
		result.Should().Be(userId);
	}
}
