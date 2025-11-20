using BunnySlinger;

using ExampleBunnies;

using Microsoft.Extensions.Logging;


namespace ExampleCatcherApp
{
    public class TestOtherBunnyCatcher(ILogger<TestBunnyCatcher> logger) : IBunnyCatcher<TestBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestBunny bunny) {
            logger.LogInformation($"{nameof(TestOtherBunnyCatcher)}: {bunny.Message}");
            return Task.FromResult(true);
	    }
    }
}
