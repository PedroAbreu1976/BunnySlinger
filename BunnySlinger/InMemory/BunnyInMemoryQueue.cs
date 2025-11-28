using System.Threading.Channels;

namespace BunnySlinger.InMemory;

public class BunnyInMemoryQueue(int capacity = 100): IDisposable
{
	private readonly Channel<IBunny> _queue =
		Channel.CreateBounded<IBunny>(new BoundedChannelOptions(capacity)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = false,
			AllowSynchronousContinuations = false
		});

	public ChannelReader<IBunny> Reader => _queue.Reader;
	public ChannelWriter<IBunny> Writer => _queue.Writer;
	
	public event BunnyDispatchedAsyncEventHandler? BunnyDispatched;
	
	public async Task<bool> OnBunnyDispatchedAsync(IBunny bunny, CancellationToken ct = default) {
		if (BunnyDispatched != null) {
			var args = new BunnyDispatchedAsyncEventHandlerArgs(bunny);
			await BunnyDispatched(this, args);
			return args.Handled;
        }
		return true;
	}

	public void ClearEvents() {
		BunnyDispatched = null;
    }

	/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
	public void Dispose() {
		ClearEvents();

	}
}

public delegate Task BunnyDispatchedAsyncEventHandler(object sender, BunnyDispatchedAsyncEventHandlerArgs args);

public class BunnyDispatchedAsyncEventHandlerArgs : EventArgs {
	public BunnyDispatchedAsyncEventHandlerArgs(IBunny bunny) {
		Bunny = bunny;
    }
    public IBunny Bunny { get; }
	
	public bool Handled { get; set; }
}

