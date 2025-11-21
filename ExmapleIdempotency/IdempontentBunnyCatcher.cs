using BunnySlinger;
using Microsoft.Extensions.Logging;

namespace ExampleIdempotency
{
    public class IdempontentBunnyCatcher(ILogger<IdempontentBunnyCatcher> logger) : IBunnyCatcher<IdempotentBunny>
    {
	    public Task<bool> CatchBunnyAsync(IdempotentBunny bunny) {
            Console.WriteLine($"Received by {nameof(IdempontentBunnyCatcher)}: {bunny.Message}");
            return Task.FromResult(true);
	    }
    }
}
