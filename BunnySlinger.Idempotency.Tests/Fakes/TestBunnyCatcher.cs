using Microsoft.Extensions.Logging;

namespace BunnySlinger.Idempotency.Tests.Fakes
{
    public class TestBunnyCatcher() : IBunnyCatcher<TestBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestBunny bunny) {
            return Task.FromResult(true);
	    }
    }
}
