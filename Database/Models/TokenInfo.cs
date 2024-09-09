using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database.Models;

[Table("tokens")]
public class TokenInfo : BaseModel
{
	[Column("user_id")]
	[Required]
	[JsonPropertyName("user_id")]
	public Guid UserId { get; set; }

	[Column("token")]
	[Required]
	[JsonPropertyName("token")]
	public string Token { get; set; }
}
