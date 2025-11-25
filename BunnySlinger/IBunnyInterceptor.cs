namespace BunnySlinger ;

/// <summary>
/// Defines a mechanism for intercepting and optionally modifying the behavior of a bunny-catching operation.
/// </summary>
/// <remarks>Implementations of this interface can be used to intercept calls to a bunny-catching operation, 
/// allowing custom logic to be executed before or after the operation. The interceptor can also  decide whether to
/// proceed with the operation or short-circuit it.</remarks>
public interface IBunnyInterceptor 
{
	/// <summary>
	/// Attempts to catch the specified bunny using the provided catcher function and handler type.
	/// </summary>
	/// <remarks>This method allows for custom bunny-catching logic to be provided via the <paramref
	/// name="catcher"/> function. The <paramref name="handlerType"/> parameter can be used to specify the context or
	/// strategy for the operation.</remarks>
	/// <param name="bunny">The bunny to be caught. Cannot be <see langword="null"/>.</param>
	/// <param name="catcher">A function that defines the logic for catching the bunny. The function takes an <see cref="IBunny"/> as input and
	/// returns a <see cref="Task{TResult}"/> representing the asynchronous operation, with a result of <see
	/// langword="true"/> if the bunny was successfully caught; otherwise, <see langword="false"/>.</param>
	/// <param name="handlerType">The type of the handler responsible for processing the bunny-catching operation. This must be a valid <see
	/// cref="Type"/> and cannot be <see langword="null"/>.</param>
	/// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation. The result is <see langword="true"/> if the
	/// bunny was successfully caught; otherwise, <see langword="false"/>.</returns>
	Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType);
}

/// <summary>
/// Defines an interceptor for handling operations related to a specific type of bunny.
/// </summary>
/// <remarks>This interface provides a mechanism to intercept and process bunny-related operations, allowing
/// custom logic  to be executed before or after the operation is performed. It is designed to work with a specific type
/// of bunny  and extends the generic <see cref="IBunnyInterceptor"/> interface.</remarks>
/// <typeparam name="TBunny">The type of bunny that this interceptor handles. Must implement the <see cref="IBunny"/> interface.</typeparam>
public interface IBunnyInterceptor<TBunny> : IBunnyInterceptor where TBunny : IBunny
{
	/// <summary>
	/// Handles the process of attempting to catch a bunny using the specified catcher function.
	/// </summary>
	/// <param name="bunny">The bunny to be caught. Cannot be null.</param>
	/// <param name="catcher">A function that defines the logic for catching the bunny. The function takes an <see cref="IBunny"/> as input and
	/// returns a <see cref="Task{Boolean}"/> indicating whether the catch was successful.</param>
	/// <param name="handlerType">The type of the handler responsible for processing the catch operation. This is used to identify the context or
	/// logic associated with the operation.</param>
	/// <returns>A <see cref="Task{Boolean}"/> that resolves to <see langword="true"/> if the bunny was successfully caught;
	/// otherwise, <see langword="false"/>.</returns>
	Task<bool> OnBunnyCatch(TBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType);

	/// <inheritdoc/>
    Task<bool> IBunnyInterceptor.OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
	{
		if (bunny is TBunny typedBunny)
		{
			return OnBunnyCatch(typedBunny, catcher, handlerType);
		}

		throw new Exception();
	}
}