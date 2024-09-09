using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database.Models;

[Table("users")]
public class User : BaseModel
{
	[Column("user_name")]
	[Required]
	[JsonPropertyName("user_name")]
	public string Username { get; set; }

	[Column("email")]
	[Required]
	[JsonPropertyName("email")]
	public string Email { get; set; }

	[Column("password_hash")]
	[Required]
	[JsonPropertyName("password_hash")]
	public string PasswordHash { get; set; }
}
