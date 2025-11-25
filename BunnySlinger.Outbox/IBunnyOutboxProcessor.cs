namespace BunnySlinger.Outbox
{
    /// <summary>
    /// Defines a contract for processing outbox messages in an asynchronous manner.
    /// </summary>
    /// <remarks>Implementations of this interface are responsible for processing messages stored in an
    /// outbox,  typically as part of a message-driven architecture. This ensures reliable message delivery  by handling
    /// messages that need to be sent or processed asynchronously.</remarks>
    public interface IBunnyOutboxProcessor
    {
        /// <summary>
        /// Processes the operation asynchronously.
        /// </summary>
        /// <param name="stoppingToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ProcessAsync(CancellationToken stoppingToken = default);
    }
}
