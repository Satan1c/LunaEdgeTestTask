using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Database.Models;

[Table("tasks")]
public class Task : BaseModel
{
	[Column("user_id")]
	[Required]
	[JsonPropertyName("user_id")]
	public Guid UserId { get; set; }

	[Column("title")]
	[Required]
	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[Column("description")]
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[Column("status")]
	[Required]
	[JsonPropertyName("status")]
	public Status Status { get; set; }

	[Column("priority")]
	[Required]
	[JsonPropertyName("priority")]
	public Priority Priority { get; set; }

	[Column("due_date")]
	[JsonPropertyName("due_date")]
	public DateTime? DueDate { get; set; }
}

public enum Status : byte
{
	None,
	Pending,
	Progress,
	Completed
}

public enum Priority : byte
{
	None,
	Low,
	Medium,
	High
}
