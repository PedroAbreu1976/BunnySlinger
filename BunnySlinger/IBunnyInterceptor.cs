namespace BunnySlinger ;

public interface IBunnyInterceptor 
{
	Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher);
}

public interface IBunnyInterceptor<TBunny> : IBunnyInterceptor where TBunny : IBunny
{
	Task<bool> OnBunnyCatch(TBunny bunny, Func<IBunny, Task<bool>> catcher);

	Task<bool> IBunnyInterceptor.OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher)
	{
		if (bunny is TBunny typedBunny)
		{
			return OnBunnyCatch(typedBunny, catcher);
		}

		throw new Exception();
	}
}