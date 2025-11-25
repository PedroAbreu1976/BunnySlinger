using Microsoft.Extensions.Logging;

namespace BunnySlinger.Idempotency.Tests.Fakes
{
    public class TestIdempotentBunnyCatcher() : IBunnyCatcher<TestIdempotentBunny>
    {
	    public Task<bool> CatchBunnyAsync(TestIdempotentBunny bunny) {
            return Task.FromResult(true);
	    }
    }
}
