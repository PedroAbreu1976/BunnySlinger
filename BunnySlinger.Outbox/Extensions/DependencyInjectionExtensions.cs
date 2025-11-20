using BunnySlinger.Outbox.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;


namespace BunnySlinger.Outbox.Extensions;

public static class DependencyInjectionExtensions {
	public static IServiceCollection AddBunnyOutbox<TDbContext>(this IServiceCollection services, BunnyOutboxOptions options)
		where TDbContext : DbContext
	{
		services.AddSingleton<IOptions<BunnyOutboxOptions>>(sp => new BunnyOutboxOptionsOptions(options));
		return services.AddBunnyOutboxCommonServices<TDbContext>();
	}

	public static IServiceCollection AddBunnyOutbox<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		services.ConfigureOptions<BunnyOutboxOptionsSetup>();
		return services.AddBunnyOutboxCommonServices<TDbContext>();
	}

	private static IServiceCollection AddBunnyOutboxCommonServices<TDbContext>(this IServiceCollection services)
		where TDbContext : DbContext
	{
		services.AddScoped<IBunnyOutbox, BunnyOutbox<TDbContext>>();
		services.AddScoped<IBunnyOutboxProcessor, BunnyOutboxProcessor<TDbContext>>();
		services.AddHostedService<BunnyOutboxWorker>();
		return services;
	}
}
