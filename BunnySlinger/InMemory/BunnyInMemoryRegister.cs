using Microsoft.Extensions.DependencyInjection;


namespace BunnySlinger.InMemory;

public class BunnyInMemoryRegister(IServiceProvider serviceProvider, BunnyInMemoryQueue queue) : IBunnyRegister {
	private readonly List<Action> _handlers = [];

    public void AddBunny<T>() where T : IBunny {

	}
	public void AddBunny(Type type) {

	}
	public void AddBunnyCatcher<THandler, TMessage>() where THandler : IBunnyCatcher<TMessage> where TMessage : IBunny {
		AddBunnyCatcher(typeof(THandler), typeof(TMessage));
	}

	public void AddBunnyCatcher(Type handlerType, Type msgType) {
		_handlers.Add(() => {
			queue.BunnyDispatched += async (sender, args) => {
				if (args.Bunny.GetType() == msgType)
				{
					using (var scope = serviceProvider.CreateScope()) {
						var handler = scope.ServiceProvider.GetRequiredService(handlerType) as IBunnyCatcher;
						var interceptors = scope.ServiceProvider.GetRequiredService<BunnyInterceptors>();
						args.Handled = await interceptors.OnBunnyCatch(args.Bunny, handler!.CatchBunnyAsync);
					}
				}
			};
		});
    }

	public void AddBunnyCatcher<T>(IBunnyCatcher<T> handler) where T : IBunny {
		_handlers.Add(() => {
			queue.BunnyDispatched += async (sender, args) => {
				if (args.Bunny is T msg) {
					var interceptors = serviceProvider.GetRequiredService<BunnyInterceptors>();
					args.Handled = await interceptors.OnBunnyCatch(msg, handler!.CatchBunnyAsync);
				}
			};
		});
    }
	
	public Task RegisterAsync() {
		foreach (var action in _handlers)
		{
			action();
		}
		

		return Task.CompletedTask;
    }

	public Task OnConnectionBrokenAsync()
	{
		return Task.CompletedTask;
	}
	public Task OnConnectionEstablishedAsync()
	{
		return Task.CompletedTask;
	}

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
	{

	}
}

