using RabbitMQ.Client;


namespace BunnySlinger.Rabbit
{
    public interface IChannelProvider : IDisposable
    {
	    Task<IChannel> Create(CancellationToken ct = default);
	    Task InitializeAsync(CancellationToken ct = default);
	    void AddConnectionObserver(IConnectionObserver observer);
	    bool IsConnectionOpen { get; }
    }
}
