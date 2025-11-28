namespace BunnySlinger.InMemory;

public class BunnyInMemoryBroker(BunnyInMemoryQueue queue) : IBunnyBroker, IDisposable
{
	public event AsyncEventHandler? ConnectionEstablishedAsync;
	public event AsyncEventHandler? ConnectionBrokenAsync;

	public Task RegisterBunnyAsync(Type type)
	{
		return Task.CompletedTask;
	}
	public Task RegisterBunnyCatcher(Type handlerType, Type msgType, Func<IBunny, Task<bool>> dispachtBunnyAsync)
	{
		queue.BunnyDispatched += async (sender, args) => {
			if (args.Bunny.GetType() == msgType)
			{
				args.Handled = await dispachtBunnyAsync(args.Bunny);
			}
		};
		return Task.CompletedTask;
	}

	public async Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny
	{
		await queue.Writer.WriteAsync(bunny, ct);
	}

	/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
	public void Dispose()
	{
		ConnectionBrokenAsync = null;
		ConnectionEstablishedAsync = null;
	}
}
