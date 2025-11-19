using Microsoft.Extensions.DependencyInjection;


namespace BunnySlinger.InMemory;

public class BunnyInMemoryRegister(IServiceProvider serviceProvider, BunnyInMemoryQueue queue) : IBunnyRegister {
	private readonly List<Action> _handlers = [];

	internal bool SendEvents { get; set; } = false;
	
	
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
					bool result;
					using (var scope = serviceProvider.CreateScope()) {
						// Get the handler instance using the resolved handlerType
						var handler = scope.ServiceProvider.GetRequiredService(handlerType) as IBunnyCatcher;
						var interceptors = scope.ServiceProvider.GetRequiredService<BunnyInterceptors>();
						args.Handled = await interceptors.OnBunnyCatch(args.Bunny, handler!.CatchBunnyAsync);
						//args.Handled = await handler!.CatchBunnyAsync(args.Bunny);
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

     //               var handled = await handler.CatchBunnyAsync(msg);
					//args.Handled = handled;
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

	public Task OnConnectionBokenAsync()
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

