using BunnySlinger;
using ExampleBunnies;
using Microsoft.Extensions.Logging;

namespace ExampleInMemory
{
    public class TestBunnyCatcher(ILogger<TestBunnyCatcher> logger) : IBunnyCatcher<TestBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestBunny bunny) {
            logger.LogInformation($"{nameof(TestBunnyCatcher)}: {bunny.Message}");
            return Task.FromResult(true);
	    }
    }
}
