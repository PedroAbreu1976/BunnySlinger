using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Outbox;

/// <summary>
/// Represents an item that has been processed by the Bunny system, including metadata about its creation, type,
/// payload, and dispatch status.
/// </summary>
/// <remarks>This class is used to track the lifecycle of a processed item, including its creation timestamp,
/// type, payload data, and dispatch attempts. It includes properties for retry tracking and error reporting in case of
/// dispatch failures.</remarks>
public class BunnyProcessedItem {
	/// <summary>
	/// Gets the unique identifier for this entity.
	/// </summary>
    [Key]
	public Guid Id { get; init; }

	/// <summary>
	/// Gets the date and time when the object was created.
	/// </summary>
    public DateTime CreatedAt { get; init; }

	/// <summary>
	/// Gets the type of the bunny represented as a string.
	/// </summary>
    public string Type { get; init; }

	/// <summary>
	/// Gets the JSON payload data associated with the current instance.
	/// </summary>
    public string Payload { get; init; }

	/// <summary>
	/// Gets or sets the date and time when the item was dispatched.
	/// </summary>
	public DateTime DispatchedAt { get; set; }

	/// <summary>
	/// Gets or sets the error message from the most recent dispatch attempt, if any.
	/// </summary>
	public string? LastDispatchError { get; set; }

	/// <summary>
	/// Gets or sets the number of retry attempts.
	/// </summary>
    public int RetryCount { get; set; }
}
