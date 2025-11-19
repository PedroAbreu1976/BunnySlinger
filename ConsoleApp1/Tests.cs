using BunnySlinger;

using Microsoft.Extensions.Logging;


namespace ConsoleApp1
{
    public class MyCuteBunny :IBunny
    {
        public string Name { get; set; }
    }

    public class MyBunnyCatcher : IBunnyCatcher<MyCuteBunny>
    {

        public static int _count = 0;
        public int id;

        public MyBunnyCatcher()
        {
            _count++;
            id = _count;
        }

        public Task<bool> CatchBunnyAsync(MyCuteBunny bunny)
        {
            Console.WriteLine($"Received  Handler 1 [{id}]: {bunny.Name}");
            return Task.FromResult(true);
        }
    }

    public class MyOtherBunnyCatcher : IBunnyCatcher<MyCuteBunny>
    {
        public Task<bool> CatchBunnyAsync(MyCuteBunny bunny) {
	        //throw new Exception("MyOtherBunnyCatcher throws exception");
            Console.WriteLine($"Received Handler 2: {bunny.Name}");
            return Task.FromResult(true);
        }
    }

    public class ExceptionHandlerBunnyInterceptor(ILogger<ExceptionHandlerBunnyInterceptor> logger) : IBunnyInterceptor
    {

        public async Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher)
        {
            try
            {

                var result = await catcher(bunny);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}");
                return false;
            }
        }
    }

    public class TestBunnyInterceptor(ILogger<ExceptionHandlerBunnyInterceptor> logger) : IBunnyInterceptor<MyCuteBunny>
    {

	    public async Task<bool> OnBunnyCatch(MyCuteBunny bunny, Func<IBunny, Task<bool>> catcher)
	    {
			    var result = await catcher(bunny);
			    logger.LogInformation($"TestBunnyInterceptor = {result}");
			    return result;
	    }
    }
}
