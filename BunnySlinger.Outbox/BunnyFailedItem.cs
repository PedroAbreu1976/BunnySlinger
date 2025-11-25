using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Outbox;

/// <summary>
/// Represents an item that failed to be processed or dispatched, including details about the failure and retry
/// attempts.
/// </summary>
/// <remarks>This class is typically used to track and manage items that could not be successfully dispatched, 
/// providing information such as the failure type, payload, and retry history. It includes metadata  about the failure
/// and supports retry mechanisms.</remarks>
public class BunnyFailedItem {
	/// <summary>
	/// Gets the unique identifier for the entity.
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
    /// Gets the JSON payload of the failed item as a string.
    /// </summary>
    public string Payload { get; init; }

	/// <summary>
	/// Gets or sets the timestamp of the most recent attempt to dispatch the operation.
	/// </summary>
	/// <remarks>This property is typically used to track when the last dispatch attempt occurred,  which can be
	/// useful for retry logic or monitoring purposes.</remarks>
	public DateTime LastDispatchedTryAt { get; set; }

	/// <summary>
	/// Gets or sets the error message associated with a dispatch operation failure.
	/// </summary>
	public string DispatchError { get; set; }

	/// <summary>
	/// Gets or sets the number of retry attempts.
	/// </summary>
    public int RetryCount { get; set; }
}
