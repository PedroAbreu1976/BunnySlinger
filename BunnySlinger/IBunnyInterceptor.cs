namespace BunnySlinger ;

public interface IBunnyInterceptor 
{
	Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType);
}

public interface IBunnyInterceptor<TBunny> : IBunnyInterceptor where TBunny : IBunny
{
	Task<bool> OnBunnyCatch(TBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType);

	Task<bool> IBunnyInterceptor.OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
	{
		if (bunny is TBunny typedBunny)
		{
			return OnBunnyCatch(typedBunny, catcher, handlerType);
		}

		throw new Exception();
	}
}