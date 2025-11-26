using RabbitMQ.Client;


namespace BunnySlinger.Rabbit
{
	/// <summary>
	/// Defines a provider for creating and managing communication channels.
	/// </summary>
	/// <remarks>This interface provides methods to create channels, initialize the provider, and manage connection
	/// observers.  It also exposes a property to check the connection status. Implementations of this interface are
	/// expected  to handle resource cleanup by implementing <see cref="IDisposable"/>.</remarks>
    public interface IChannelProvider : IDisposable
    {
        /// <summary>
        /// Initializes the connection to the broker and creates a new communication channel asynchronously.
        /// </summary>
        /// <remarks>The operation may be canceled by passing a cancellation token through the <paramref name="ct"/>
        /// parameter.  If the operation is canceled, the returned task will be in a canceled state.</remarks>
        /// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="IChannel"/>
        /// instance.</returns>
        Task<IChannel> Create(CancellationToken ct = default);

		/// <summary>
		/// Asynchronously initializes the connection to the broker, preparing it for use.
		/// </summary>
		/// <remarks>This method must be called before using the component. If the operation is canceled via the
		/// <paramref name="ct"/> parameter, the returned task will be in a canceled state.</remarks>
		/// <param name="ct">A <see cref="CancellationToken"/> that can be used to cancel the initialization operation.</param>
		/// <returns>A <see cref="Task"/> that represents the asynchronous initialization operation.</returns>
	    Task InitializeAsync(CancellationToken ct = default);

		/// <summary>
		/// Registers an observer to receive notifications about connection state changes.
		/// </summary>
		/// <param name="observer">The observer to be notified of connection events. Cannot be <see langword="null"/>.</param>
	    void AddConnectionObserver(IConnectionObserver observer);

		/// <summary>
		/// Gets a value indicating whether the connection to the broker is currently open.
		/// </summary>
	    bool IsConnectionOpen { get; }
    }
}
