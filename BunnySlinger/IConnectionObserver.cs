namespace BunnySlinger
{
    /// <summary>
    /// Defines a contract for observing connection state changes.
    /// </summary>
    /// <remarks>Implement this interface to handle events related to the establishment and loss of a
    /// connection.</remarks>
    public interface IConnectionObserver {
        /// <summary>
        /// Handles the event when a connection is unexpectedly broken.
        /// </summary>
        /// <remarks>This method is invoked to perform any necessary cleanup or logging when a connection
        /// is lost.  Implementations should ensure that the method is non-blocking and resilient to
        /// exceptions.</remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation. The task completes when the handling of the
        /// broken connection is finished.</returns>
	    Task OnConnectionBrokenAsync();

        /// <summary>
        /// Invoked when a connection is successfully established.
        /// </summary>
        /// <remarks>This method is intended to handle any logic that should occur immediately after a
        /// connection is established. Override this method to implement custom behavior.</remarks>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
		Task OnConnectionEstablishedAsync();
    }
}
