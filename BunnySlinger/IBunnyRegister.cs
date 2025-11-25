namespace BunnySlinger
{
	/// <summary>
	/// Defines a contract for managing the registration of bunny-related types and handlers,  as well as observing
	/// connection state changes.
	/// </summary>
	/// <remarks>This interface provides methods to register bunny types and their associated handlers,  enabling a
	/// flexible and extensible mechanism for handling bunny-related operations.  It also supports asynchronous
	/// registration and implements <see cref="IDisposable"/>  for resource cleanup and <see cref="IConnectionObserver"/>
	/// for monitoring connection state.</remarks>
    public interface IBunnyRegister : IDisposable, IConnectionObserver {
		/// <summary>
		/// Adds a new bunny of the specified type to the collection.
		/// </summary>
		/// <remarks>This method requires that the specified type <typeparamref name="T"/> has a parameterless
		/// constructor.</remarks>
		/// <typeparam name="T">The type of bunny to add. Must implement the <see cref="IBunny"/> interface.</typeparam>
	    void AddBunny<T>() where T : IBunny;

		/// <summary>
		/// Adds a bunny of the specified type to the collection.
		/// </summary>
		/// <param name="type">The type of bunny to add. This parameter cannot be null.</param>
	    void AddBunny(Type type);

		/// <summary>
		/// Registers a bunny catcher to handle messages of a specific type.
		/// </summary>
		/// <remarks>This method associates a handler with a specific message type, enabling the system to route
		/// messages of that type to the appropriate handler. The handler type must implement <see
		/// cref="IBunnyCatcher{TMessage}"/>, and the message type must implement <see cref="IBunny"/>.</remarks>
		/// <typeparam name="THandler">The type of the handler that implements <see cref="IBunnyCatcher{TMessage}"/>.</typeparam>
		/// <typeparam name="TMessage">The type of the message that the handler processes, which must implement <see cref="IBunny"/>.</typeparam>
	    void AddBunnyCatcher<THandler, TMessage>() where THandler : IBunnyCatcher<TMessage>
		    where TMessage : IBunny;

		/// <summary>
		/// Registers a handler for processing messages of a specific type.
		/// </summary>
		/// <remarks>This method associates a handler type with a specific message type, enabling the system to route
		/// messages to the appropriate handler.  Both <paramref name="handlerType"/> and <paramref name="msgType"/> must be
		/// non-null and compatible with the expected processing logic.</remarks>
		/// <param name="handlerType">The type of the handler responsible for processing the messages. Must implement the appropriate handler interface.</param>
		/// <param name="msgType">The type of the message that the handler will process.</param>
	    void AddBunnyCatcher(Type handlerType, Type msgType);

		/// <summary>
		/// Registers a handler to process bunnies of a specific type.
		/// </summary>
		/// <remarks>This method associates the specified handler with the type <typeparamref name="T"/>.  When a
		/// bunny of type <typeparamref name="T"/> is encountered, the registered handler will be invoked.</remarks>
		/// <typeparam name="T">The type of bunny that the handler can process. Must implement <see cref="IBunny"/>.</typeparam>
		/// <param name="handler">The handler responsible for processing bunnies of type <typeparamref name="T"/>. Cannot be <see langword="null"/>.</param>
	    void AddBunnyCatcher<T>(IBunnyCatcher<T> handler) where T : IBunny;

		/// <summary>
		/// Registers the current the added bunnies and catchers asynchronously.
		/// </summary>
		/// <returns>A task that represents the asynchronous operation.</returns>
	    Task RegisterAsync();
    }
}
