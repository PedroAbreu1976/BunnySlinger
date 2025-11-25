namespace BunnySlinger.Outbox
{
    /// <summary>
    /// Defines an outbox for queuing bunny messages to be processed asynchronously.
    /// </summary>
    /// <remarks>This interface is typically used to ensure reliable message delivery by queuing messages  for
    /// processing in a durable or transactional manner. Implementations may vary in how  messages are persisted or
    /// transmitted.</remarks>
    public interface IBunnyOutbox
    {
        /// <summary>
        /// Queues a bunny for processing asynchronously.
        /// </summary>
        /// <typeparam name="TBunny">The type of the bunny to be queued. Must implement <see cref="IBunny"/>.</typeparam>
        /// <param name="bunny">The bunny instance to be queued. Cannot be <see langword="null"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. Optional.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the bunny
        /// was successfully queued; otherwise, <see langword="false"/>.</returns>
        Task<bool> QueueBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny;
    }
}
