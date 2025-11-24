using BunnySlinger.Tests.Bunnies;
using Microsoft.Extensions.Logging;

namespace BunnySlinger.Tests.Catchers
{
    public class TestBunnyCatcher(ILogger<TestBunnyCatcher> logger) : IBunnyCatcher<TestBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestBunny bunny) {
            return Task.FromResult(true);
	    }
    }
}
