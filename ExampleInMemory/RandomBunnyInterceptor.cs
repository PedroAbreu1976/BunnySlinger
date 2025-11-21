using BunnySlinger;
using Microsoft.Extensions.Logging;


namespace ExampleInMemory
{
    public class RandomBunnyInterceptor(ILogger<RandomBunnyInterceptor> logger) : IBunnyInterceptor
    {
	    public Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher) {
		    var randomNumber = Random.Shared.Next(1, 4);
		    if (randomNumber == 3) {
				logger.LogWarning($"Bunny was intercepted because... stuff");
				return Task.FromResult(true);
		    }

		    return catcher(bunny);
	    }
    }
}
