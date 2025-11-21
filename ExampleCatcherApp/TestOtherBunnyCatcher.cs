using BunnySlinger;

using ExampleBunnies;

using Microsoft.Extensions.Logging;


namespace ExampleCatcherApp
{
    public class TestOtherBunnyCatcher(ILogger<TestOtherBunnyCatcher> logger) : IBunnyCatcher<TestBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestBunny bunny) {
            Console.WriteLine($"Received by {nameof(TestOtherBunnyCatcher)}: {bunny.Message}");
            return Task.FromResult(true);
	    }
    }
}
