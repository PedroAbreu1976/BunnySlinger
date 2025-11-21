
using BunnySlinger.Tests.Bunnies;


namespace BunnySlinger.Tests.Interceptors
{
	internal class DummyInterceptor : IBunnyInterceptor
	{
		public List<string> Calls = new();
		public Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
		{
			Calls.Add("global");
			return catcher(bunny);
		}
		
		
	}

	internal class DummyTypedInterceptor : IBunnyInterceptor<TestBunny>
	{
		public List<string> Calls = new();
		public Task<bool> OnBunnyCatch(TestBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
		{
			Calls.Add("typed");
			return catcher(bunny);
		}
	}
}
