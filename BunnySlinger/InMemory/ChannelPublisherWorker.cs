using Microsoft.Extensions.Hosting;

namespace BunnySlinger.InMemory;

public class ChannelPublisherWorker(BunnyInMemoryQueue queue) : BackgroundService {

    protected override async Task ExecuteAsync(CancellationToken ct) {
		while (await queue.Reader.WaitToReadAsync(ct)) {
			var bunny = await queue.Reader.ReadAsync(ct);
			await queue.OnBunnyDispatchedAsync(bunny, ct);
        }
	}
}

