using BunnySlinger.Options;
using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace BunnySlinger.Rabbit
{
    public class BunnyMqChannelProvider(IOptions<BunnyMqOptions> bunnyMqOptions) : IChannelProvider
    {
		private IConnection? _connection;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
		private readonly List<IConnectionObserver> _observers = new List<IConnectionObserver>();

        public async Task<IChannel> Create(CancellationToken ct = default) {

	        await InitializeAsync();
		    return await _connection!.CreateChannelAsync(cancellationToken: ct);
	}

		public async Task InitializeAsync(CancellationToken ct = default) {
			await _semaphore.WaitAsync();
			try
			{
				if (_connection is null || !_connection.IsOpen) {
					var factory = bunnyMqOptions.Value.GetFactory();// new ConnectionFactory() { HostName = "localhost", Port = 5672 };
                    _connection = await factory.CreateConnectionAsync(ct);
                    if (_connection.IsOpen) {
                        _connection.ConnectionShutdownAsync += _connection_ConnectionShutdownAsync;
                    }
				}
			}
			finally
			{
				_semaphore.Release();
			}
        }

        private async Task _connection_ConnectionShutdownAsync(object sender, RabbitMQ.Client.Events.ShutdownEventArgs @event) {
	        _connection = null;
	        foreach (var observer in _observers)
	        {
		        await observer.OnConnectionBokenAsync();
	        }
			
            while (_connection == null || !_connection.IsOpen) {
		        await Task.Delay(200);
				await InitializeAsync();
            }
			
	        foreach (var observer in _observers)
	        {
		        await observer.OnConnectionEstablishedAsync();
	        }
        }

        public void AddConnectionObserver(IConnectionObserver observer) {
			_observers.Add(observer);
        }

        public bool IsConnectionOpen => _connection?.IsOpen == true;

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
        /// <returns>A task that represents the asynchronous dispose operation.</returns>
        public void Dispose() {
			_connection?.Dispose();
			_connection = null;
			_semaphore.Dispose();
			
			
		}
    }
}
