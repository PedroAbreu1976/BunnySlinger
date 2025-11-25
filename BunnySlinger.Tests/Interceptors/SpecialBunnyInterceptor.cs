using BunnySlinger.Tests.Bunnies;

namespace BunnySlinger.Tests.Interceptors
{
	internal class SpecialBunnyInterceptor : IBunnyInterceptor<ISpecialBunny>
	{
		public List<string> Calls = new();
		public Task<bool> OnBunnyCatch(ISpecialBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
		{
			Calls.Add("special");
			return catcher(bunny);
		}
	}
}
