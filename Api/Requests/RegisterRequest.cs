using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Api.Requests;

public class RegisterRequest : LoginRequest, IEmailRequest
{
	[Required]
	[JsonPropertyName("email")]
	public string Email { get; set; } = string.Empty;
}
