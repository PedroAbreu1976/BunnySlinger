namespace BunnySlinger.Outbox.Options
{
    /// <summary>
    /// Represents configuration options for the Bunny Outbox, which manages the dispatching of messages and handling of
    /// failed or expired messages.
    /// </summary>
    /// <remarks>This class provides settings to control the behavior of the outbox processor, including the
    /// frequency of message dispatch checks, retry limits for failed messages, and expiration policies for undelivered
    /// messages.</remarks>
    public class BunnyOutboxOptions
    {
        /// <summary>
        /// Gets or sets the frequency in milliseconds at which the outbox processor checks for new messages to dispatch.
        /// </summary>
        public int ProcessorFrequency { get; set; } = (int)TimeSpan.FromSeconds(1).TotalMilliseconds;

        /// <summary>
        /// Gets or sets the maximum number of retry attempts for dispatching a message before it is considered failed and moved to the failed outbox.
        /// </summary>
        public int? MaxRetryCount { get; set; }

        /// <summary>
        /// Gets or sets the age in seconds after which undelivered messages are considered expired and moved to the failed outbox.
        /// </summary>
        public int ExpireOlderThan { get; set; } = (int)TimeSpan.FromHours(1).TotalSeconds;
    }
}
