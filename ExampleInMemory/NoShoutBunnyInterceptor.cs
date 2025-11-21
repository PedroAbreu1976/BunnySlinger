using BunnySlinger;

using ExampleBunnies;
using Microsoft.Extensions.Logging;


namespace ExampleInMemory
{
    public class NoShoutTestBunnyInterceptor(ILogger<NoShoutTestBunnyInterceptor> logger) : IBunnyInterceptor<TestBunny>
    {
	    public Task<bool> OnBunnyCatch(TestBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType) {
		    if (bunny.Message.ToUpper() == bunny.Message) {
                logger.LogWarning($"Bunny refused because it carries a shouting message: {bunny.Message}");
                return Task.FromResult(true);
		    }

		    return catcher(bunny);
	    }
    }
}
