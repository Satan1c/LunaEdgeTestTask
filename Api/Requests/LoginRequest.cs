using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Requests;

public class LoginRequest : ILoginRequest
{
	[Required]
	[JsonPropertyName("login")]
	public string Login { get; set; } = string.Empty;

	[Required]
	[JsonPropertyName("password")]
	public string Password { get; set; } = string.Empty;
}
