using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Database.Models;

namespace Api.Requests;

public class TaskRequest
{
	[JsonPropertyName("description")]
	public string? Description { get; set; }

	[JsonPropertyName("due_date")]
	public DateTime? DueDate { get; set; }

	[Required]
	[JsonPropertyName("title")]
	public string Title { get; set; } = string.Empty;

	[Required]
	[JsonPropertyName("status")]
	public Status Status { get; set; }

	[Required]
	[JsonPropertyName("priority")]
	public Priority Priority { get; set; }
}
