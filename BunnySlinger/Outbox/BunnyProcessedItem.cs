using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Outbox;

public class BunnyProcessedItem {
	[Key]
	public Guid Id { get; init; }

	public DateTime CreatedAt { get; init; }
	public string Type { get; init; }
	public string Payload { get; init; }
	public DateTime DispatchedAt { get; set; }
	public string? LastDispatchError { get; set; }
	public int RetryCount { get; set; }
}
