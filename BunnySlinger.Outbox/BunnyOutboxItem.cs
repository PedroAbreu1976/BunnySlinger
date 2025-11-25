using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Outbox
{
    /// <summary>
    /// Represents an item in the Bunny outbox, used for storing messages or events to be dispatched.
    /// </summary>
    /// <remarks>This class is typically used in message-driven architectures to ensure reliable delivery of
    /// messages or events. Each item contains metadata such as its creation timestamp, type, and payload, as well as
    /// information about dispatch errors and retry attempts.</remarks>
    public class BunnyOutboxItem
    {
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
        /// Gets or sets the error message associated with a dispatch operation failure.
        /// </summary>
        public string? DispatchError { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        public int RetryCount { get; set; }
    }
}
