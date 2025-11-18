using System.ComponentModel.DataAnnotations;


namespace BunnySlinger.Outbox;

public class BunnyFailedItem {
	[Key]
	public Guid Id { get; init; }
	public DateTime CreatedAt { get; init; }
	public string Type { get; init; }
	public string Payload { get; init; }
	public DateTime LastDispatchedTryAt { get; set; }
	public string DispatchError { get; set; }
    public int RetryCount { get; set; }
}
