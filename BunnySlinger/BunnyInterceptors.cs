namespace BunnySlinger;

public class BunnyInterceptors(IEnumerable<IBunnyInterceptor> interceptors) {
	public async Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher) {
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
				return await interceptor.OnBunnyCatch(b, currentNext);
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
