using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger.Idempotency;

/// <summary>
/// Provides extension methods for registering idempotency-related services in a dependency injection container.
/// </summary>
/// <remarks>This class contains methods to simplify the registration of services required for implementing
/// idempotency in applications using Entity Framework Core.</remarks>
public static class DependencyInjectionExtensions {
	/// <summary>
	/// Adds the Bunny Idempotency interceptor to the service collection, enabling idempotency handling for operations
	/// using the specified <see cref="DbContext"/> type.
	/// </summary>
	/// <remarks>This method registers the <see cref="IBunnyInterceptor"/> implementation as a scoped service, using
	/// the specified <typeparamref name="TDbContext"/> to handle idempotency.</remarks>
	/// <typeparam name="TDbContext">The type of the <see cref="DbContext"/> to be used by the idempotency interceptor.</typeparam>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the idempotency interceptor will be added.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance with the idempotency interceptor registered.</returns>
	public static IServiceCollection AddBunnyIdempotency<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		return services.AddScoped<IBunnyInterceptor, IdempotentInterceptor<TDbContext>>();
	}
}
