using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database.Models;

public abstract class BaseModel
{
	[Key]
	[Required]
	[Column("id")]
	public Guid Id { get; set; }

	[Column("created_at")]
	[JsonPropertyName("created_at")]
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	[Column("updated_at")]
	[JsonPropertyName("updated_at")]
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
