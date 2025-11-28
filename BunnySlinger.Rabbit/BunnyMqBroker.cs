using System.Text;
using System.Text.Json;
using BunnySlinger.Rabbit.Extensions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BunnySlinger.Rabbit;

public class BunnyMqBroker :IBunnyBroker, IDisposable, IConnectionObserver {
	public event AsyncEventHandler? ConnectionEstablishedAsync;
	public event AsyncEventHandler? ConnectionBrokenAsync;

	private readonly IChannelProvider _channelProvider;

	public BunnyMqBroker(IChannelProvider channelProvider)
	{
		_channelProvider = channelProvider;
		channelProvider.AddConnectionObserver(this);
	}

    public async Task RegisterBunnyAsync(Type type) {
	    var channel = await _channelProvider.Create();
	    string className = type.Name;
	    await channel.ExchangeDeclareAsync(className, ExchangeType.Direct);
    }
	public async Task RegisterBunnyCatcher(Type handlerType, Type msgType, Func<IBunny, Task<bool>> dispachtBunnyAsync) {
		var channel = await _channelProvider.Create();
		string msgTypeName = msgType.Name;
		var queue = $"{handlerType.Namespace}.{handlerType.Name}.{msgTypeName}";

		await channel.ExchangeDeclareAsync(msgTypeName, ExchangeType.Direct);
		await channel.QueueDeclareAsync(queue, true, false, false, null);
		await channel.QueueBindAsync(queue, msgTypeName, string.Empty);

		var consumer = new AsyncEventingBasicConsumer(channel);
		consumer.ReceivedAsync += async (model, ea) => {
			var json = Encoding.UTF8.GetString(ea.Body.Span);
			if (JsonSerializer.Deserialize(json, msgType) is IBunny message)
			{
				bool result = await dispachtBunnyAsync(message);

                if (result)
				{
					await channel.BasicAckAsync(ea.DeliveryTag, false);
				}
				else
				{
					await channel.BasicNackAsync(ea.DeliveryTag, false, true);
				}
			}
		};
		await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer);
    }

	public async Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default)
		where TBunny : IBunny
	{
		// need a semaphor here
		while (!_channelProvider.IsConnectionOpen || ct.IsCancellationRequested)
		{
			await _channelProvider.InitializeAsync(ct);
		}
		var channel = await _channelProvider.Create(ct);
		var body = bunny.Serialize();
		await channel.BasicPublishAsync(bunny.GetType().Name, string.Empty, true, new BasicProperties(), body, ct);
	}

    public async Task OnConnectionBrokenAsync() {
		if (ConnectionBrokenAsync != null) {
			await ConnectionBrokenAsync.Invoke(this, EventArgs.Empty);
        }
	}

	public async Task OnConnectionEstablishedAsync() {
		if (ConnectionEstablishedAsync != null)
		{
			await ConnectionEstablishedAsync.Invoke(this, EventArgs.Empty);
		}
    }

	public void Dispose() {
		ConnectionBrokenAsync = null;
		ConnectionEstablishedAsync = null;

	}
}
