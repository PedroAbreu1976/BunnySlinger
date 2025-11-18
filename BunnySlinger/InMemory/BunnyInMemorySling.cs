namespace BunnySlinger.InMemory;

internal class BunnyInMemorySling(BunnyInMemoryQueue queue) : IBunnySling {
	public async Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny {
		//bool canProceed = await queue.Writer.WaitToWriteAsync(ct);
        await queue.Writer.WriteAsync(bunny, ct);
	}
}

