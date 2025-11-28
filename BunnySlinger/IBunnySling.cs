namespace BunnySlinger
{
    /// <summary>
    /// Defines a contract for slinging a bunny asynchronously.
    /// </summary>
    /// <remarks>Implementations of this method are expected to handle the provided bunny instance in a manner
    /// specific to the implementation. Callers should ensure that the <paramref name="bunny"/> parameter is not <see
    /// langword="null"/> and that the operation is not prematurely canceled unless necessary.</remarks>
    public interface IBunnySling {
        /// <summary>
        /// Launches the specified bunny asynchronously.
        /// </summary>
        /// <remarks>This method performs the operation asynchronously and respects the provided
        /// cancellation token. Ensure that the bunny instance implements the <see cref="IBunny"/> interface.</remarks>
        /// <typeparam name="TBunny">The type of bunny to be launched. Must implement <see cref="IBunny"/>.</typeparam>
        /// <param name="bunny">The bunny instance to be launched. Cannot be <see langword="null"/>.</param>
        /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the operation to complete. The default value
        /// is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
	    Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny;
    }

    public class BunnySling(IBunnyBroker broker) : IBunnySling {
        public async Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny {
            await broker.SlingBunnyAsync(bunny, ct);
        }
    }
}
