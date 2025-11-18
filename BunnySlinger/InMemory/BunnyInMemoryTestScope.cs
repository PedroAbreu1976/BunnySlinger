namespace BunnySlinger.InMemory;

public class BunnyInMemoryTestScope {
	private readonly BunnyInMemoryRegister _register;
    private readonly BunnyInMemorySling _sling;
    private readonly ChannelPublisherWorker _channelPublisherWorker;

    public BunnyInMemoryTestScope() {
	    BunnyInMemoryQueue queue = new BunnyInMemoryQueue();
        _register = new BunnyInMemoryRegister(null!, queue);
        _sling = new BunnyInMemorySling(queue);
        _channelPublisherWorker = new ChannelPublisherWorker(queue);
    }
    

    public List<BunnyInMemoryLog<T>> AddBunnyCatcher<T>(IBunnyCatcher<T> handler) where T : IBunny {
        var testBunnyHandler = new TestBunnyCatcher<T>(handler);
        var result = new List<BunnyInMemoryLog<T>>();
        testBunnyHandler.BunnyCaught += (object? sender, BunnyInMemoryLog<T> e) => {
	        result.Add(e);
        };

        _register.AddBunnyCatcher(testBunnyHandler);
        
        return result;
    }

    public async Task StartCatching(CancellationToken ct = default)
    {
	    await _register.RegisterAsync();
	    await _channelPublisherWorker.StartAsync(ct);
    }
    
    public async Task<Guid> SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) 
	    where TBunny : IBunny 
    {
        var testBunny = new TestBunny<TBunny>(bunny);
        await _sling.SlingBunnyAsync(testBunny, ct);
        return testBunny.ID;
    }
    
    private class TestBunny<TBunny>(TBunny bunny) : IBunny
		where TBunny : IBunny
    {
        public Guid ID { get; } = Guid.NewGuid();
        internal TBunny Bunny => bunny;
    }
    
    private class TestBunnyCatcher<TBunny>(IBunnyCatcher<TBunny> catcher) : IBunnyCatcher<TestBunny<TBunny>>
	    where TBunny : IBunny 
    {
        public event EventHandler<BunnyInMemoryLog<TBunny>>? BunnyCaught;

        public async Task<bool> CatchBunnyAsync(TestBunny<TBunny> bunny) {
            bool result;
            Exception? error = null;
            try {
		        result = await catcher.CatchBunnyAsync(bunny.Bunny);
	        }
	        catch (Exception ex) {
                result = false;
                error = ex;
            }

            if (BunnyCaught != null) {
                BunnyCaught(this, new BunnyInMemoryLog<TBunny> {
                    BunnyID = bunny.ID,
                    Handled = result,
                    Error = error,
                    Bunny = bunny.Bunny,
                    Handler = catcher
                });
            }

            return result;
        }
    }

}

public class BunnyInMemoryLog<TBunny> where TBunny : IBunny {
    public Guid BunnyID { get; set; }
    public IBunnyCatcher<TBunny> Handler { get; set; }
    public TBunny Bunny { get; set; }
    public bool Handled { get; set; }
    public Exception? Error { get; set; }
}


