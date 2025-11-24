using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BunnySlinger.Rabbit;

public class BunnyMqRegister : IBunnyRegister
{

	private readonly Dictionary<Type,Func<Task>> _messages = [];
	private readonly Dictionary<Type, Func<Task>> _handlers = [];
	private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
	private readonly IChannelProvider _channelProvider;
	private readonly IServiceProvider _serviceProvider;

	public BunnyMqRegister(IChannelProvider channelProvider, IServiceProvider serviceProvider) {
		_channelProvider = channelProvider;
		_serviceProvider = serviceProvider;
		channelProvider.AddConnectionObserver(this);
    }

    public void AddBunny<T>()
		where T : IBunny {
		AddBunny(typeof(T));
	}
	
	public void AddBunny(Type type)
	{
		_messages.Add(
			type, 
			async () => {
			var channel = await _channelProvider.Create();
			string className = type.Name;
			await channel.ExchangeDeclareAsync(className, ExchangeType.Direct);
		});
    }

    public void AddBunnyCatcher<THandler, TMessage>() 
		where THandler : IBunnyCatcher<TMessage>
		where TMessage : IBunny
    {
	    AddBunnyCatcher(typeof(THandler), typeof(TMessage));
    }

    public void AddBunnyCatcher(Type handlerType, Type msgType) {
	    _handlers.Add(
		    handlerType, async () => 
		    {
			    var channel = await _channelProvider.Create();
			    string msgTypeName = msgType.Name;
			    var queue = $"{handlerType.Namespace}.{handlerType.Name}.{msgTypeName}";

			    await channel.ExchangeDeclareAsync(msgTypeName, ExchangeType.Direct);
			    await channel.QueueDeclareAsync(queue, true, false, false, null);
			    await channel.QueueBindAsync(queue, msgTypeName, string.Empty);

			    var consumer = new AsyncEventingBasicConsumer(channel);
			    consumer.ReceivedAsync += async (model, ea) => {
				    var json = Encoding.UTF8.GetString(ea.Body.Span);
				    if (JsonSerializer.Deserialize(json, msgType) is IBunny message) {
					    bool result;
					    using (var scope = _serviceProvider.CreateScope()) {
						    var handler = scope.ServiceProvider.GetRequiredService(handlerType) as IBunnyCatcher;
						    var interceptors = scope.ServiceProvider.GetRequiredService<BunnyInterceptors>();
						    result = await interceptors.OnBunnyCatch(message, handler!.CatchBunnyAsync, handlerType);
					    }

					    if (result) {
						    await channel.BasicAckAsync(ea.DeliveryTag, false);
					    }
					    else {
						    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
					    }
				    }
			    };
			    await channel.BasicConsumeAsync(queue: queue, autoAck: false, consumer: consumer);
		    });
    }

    public void AddBunnyCatcher<T>(IBunnyCatcher<T> handler) where T : IBunny {
	    _handlers.Add(
		    handler.GetType(), 
		    async () => {
			    var channel = await _channelProvider.Create();
			    var msgType = typeof(T);
			    var handlerType = handler.GetType();
			    string msgTypeName = msgType.Name;
			    var queue = $"{handlerType.Namespace}.{handlerType.Name}.{msgTypeName}";

			    await channel.ExchangeDeclareAsync(msgTypeName, ExchangeType.Direct);
			    await channel.QueueDeclareAsync(queue, true, false, false, null);
			    await channel.QueueBindAsync(queue, msgTypeName, string.Empty);

			    var consumer = new AsyncEventingBasicConsumer(channel);
			    consumer.ReceivedAsync += async (model, ea) => {
				    var json = Encoding.UTF8.GetString(ea.Body.Span);
				    var message = JsonSerializer.Deserialize<T>(json);
				    if (message != null) {
					    var interceptors = _serviceProvider.GetRequiredService<BunnyInterceptors>();
					    var result = await interceptors.OnBunnyCatch(message, handler!.CatchBunnyAsync, handlerType);
					    if (result) {
						    await channel.BasicAckAsync(ea.DeliveryTag, false);
					    }
					    else {
						    await channel.BasicNackAsync(ea.DeliveryTag, false, true);
					    }
				    }
			    };
			    await channel.BasicConsumeAsync(queue: queue, autoAck: true, consumer: consumer);
		    });
    }

    public async Task RegisterAsync() {
		await _semaphore.WaitAsync();
		try
		{
			foreach (var action  in _messages.Values) {
				await action();
            }
			foreach (var action in _handlers.Values)
			{
				await action();
			}
        }
		finally
		{
			_semaphore.Release();
		}
    }

	/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
	public void Dispose() {
		_semaphore.Dispose();
	}

	public Task OnConnectionBrokenAsync() {
		return Task.CompletedTask;
	}

	public async Task OnConnectionEstablishedAsync() {
		await RegisterAsync();
    }
}

