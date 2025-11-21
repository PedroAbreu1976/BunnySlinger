using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger.Idempotency;

public static class DependencyInjectionExtensions {
	public static IServiceCollection AddBunnyIdempotency<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		return services.AddScoped<IBunnyInterceptor, IdempotentInterceptor<TDbContext>>();
	}
}
