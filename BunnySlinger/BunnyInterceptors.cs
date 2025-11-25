namespace BunnySlinger;

/// <summary>
/// Provides a mechanism to apply a chain of interceptors to the process of catching a bunny.
/// </summary>
/// <remarks>This class allows the execution of a sequence of interceptors, where each interceptor can perform
/// custom logic before or after the main bunny-catching operation. Interceptors are applied in reverse order, with the
/// last interceptor wrapping the main operation, and so on.</remarks>
/// <param name="interceptors"></param>
public class BunnyInterceptors(IEnumerable<IBunnyInterceptor> interceptors) {
    /// <summary>
	/// Executes the bunny-catching process, applying a chain of interceptors around the provided catcher function.
	/// </summary>
	/// <remarks>This method applies a series of interceptors to the bunny-catching process. Each interceptor has
	/// the opportunity to perform custom logic before or after invoking the next step in the chain. If an interceptor
	/// decides not to call the next step, the chain is terminated early, and the result of the interceptor is
	/// returned.</remarks>
	/// <param name="bunny">The bunny to be caught. This parameter cannot be <see langword="null"/>.</param>
	/// <param name="catcher">The final function responsible for catching the bunny. This function is wrapped by the interceptors.</param>
	/// <param name="handlerType">The type of the handler initiating the bunny-catching process. This parameter is used to provide context to the
	/// interceptors.</param>
	/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the bunny was
	/// successfully caught; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType) {
		// 1. Get the relevant interceptors for this bunny type
		var applicableInterceptors = GetInterceptors(bunny);

		// 2. Start with the final action (the catcher)
		// This is the innermost part of the "onion"
		Func<IBunny, Task<bool>> next = catcher;

		// 3. Wrap the interceptors in reverse order.
		// Last Interceptor wraps Catcher.
		// Second-to-last wraps Last, etc.
		for (int i = applicableInterceptors.Count - 1; i >= 0; i--) {
			var interceptor = applicableInterceptors[i];
			var currentNext = next; // Capture the reference for the closure

			next = async (b) => {
				// Call the interceptor. 
				// If the interceptor returns 'false' and does NOT call 'currentNext',
				// the chain breaks here.
				return await interceptor.OnBunnyCatch(b, currentNext, handlerType);
			};
		}

		// 4. Execute the outermost wrapper
		return await next(bunny);
	}

	private List<IBunnyInterceptor> GetInterceptors(IBunny bunny) {
		var bunnyType = bunny.GetType();
		return interceptors.Where(interceptor => {
				var interceptorType = interceptor.GetType();

				// Find all implementations of IBunnyInterceptor<T> on this interceptor
				var genericInterfaces = interceptorType.GetInterfaces()
					.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBunnyInterceptor<>))
					.ToList();

				// Condition 1: Keep if it does NOT implement IBunnyInterceptor<TBunny> 
				// (It is a global interceptor)
				if (!genericInterfaces.Any()) {
					return true;
				}

				// Condition 2: If it DOES implement it, keep only if T matches the bunny
				// (e.g. if T is Rabbit, and bunny is a Rabbit, keep it. If bunny is a Turtle, discard).
				// We check if T.IsAssignableFrom(bunnyType).
				return genericInterfaces.Any(g => g.GetGenericArguments()[0].IsAssignableFrom(bunnyType));
			})
			.ToList();
	}
}
