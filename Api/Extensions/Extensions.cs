using System.Security.Claims;
using System.Text.RegularExpressions;
using Api.Requests;
using Database.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Task = Database.Models.Task;

namespace Api.Extensions;

public static partial class Extensions
{
	private static readonly Regex s_nonAsciiRegex = MyRegex();

	public static bool IsNonAscii(this string str) => s_nonAsciiRegex.IsMatch(str);

	public static Task TaskFromRequest(this TaskRequest taskRequest)
		=> new()
		{
			Title       = taskRequest.Title,
			Description = taskRequest.Description,
			DueDate     = taskRequest.DueDate,
			Status      = taskRequest.Status,
			Priority    = taskRequest.Priority
		};

	public static User UserFromRequest(this RegisterRequest registerRequest)
		=> new()
		{
			Email        = registerRequest.Email,
			Username     = registerRequest.Login,
			PasswordHash = JwtGenerator.HashPassword(registerRequest.Password)
		};

	public static TokenInfo TokenInfoFromUser(this User user)
		=> new() { UserId = user.Id, Token = JwtGenerator.GenerateToken(user.Id.ToString()) };

	public static CancellationToken GetToken(this EndpointFilterInvocationContext context)
		=> context.GetArgument<CancellationToken>(context.Arguments.Count - 1);

	public static Guid GetUserId(this ClaimsPrincipal userInfo) => new(userInfo.FindFirstValue(JwtRegisteredClaimNames.Jti)!);

	[GeneratedRegex(@"[^ -~]+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline)]
	private static partial Regex MyRegex();
}
