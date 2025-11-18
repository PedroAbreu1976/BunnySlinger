using System.Threading.Channels;

namespace BunnySlinger.InMemory;

// It's the Channel that handles the actual message passing
// We can control the capacity and backpressure handling here
public class BunnyInMemoryQueue(int capacity = 100)
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
}

public delegate Task BunnyDispatchedAsyncEventHandler(object sender, BunnyDispatchedAsyncEventHandlerArgs args);

public class BunnyDispatchedAsyncEventHandlerArgs : EventArgs {
	public BunnyDispatchedAsyncEventHandlerArgs(IBunny bunny) {
		Bunny = bunny;
    }
    public IBunny Bunny { get; }
	
	public bool Handled { get; set; }
}

