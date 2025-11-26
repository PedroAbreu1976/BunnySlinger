using BunnySlinger.Outbox.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox.Extensions;

/// <summary>
/// Provides extension methods for configuring and starting the Bunny Outbox in a .NET application.
/// </summary>
/// <remarks>This static class contains methods to register the Bunny Outbox services with the dependency
/// injection container and to start the Bunny Outbox worker. The Bunny Outbox is designed to facilitate reliable
/// message processing in applications using Entity Framework Core.</remarks>
public static class DependencyInjectionExtensions {
	public static IServiceCollection AddBunnyOutbox<TDbContext>(this IServiceCollection services, 
		Action<BunnyOutboxConfiguration>? configuration = null)
		where TDbContext : DbContext
	{
		services.ConfigureOptions<BunnyOutboxConfigurationSetup>();
		if(configuration is not null) {
			services.PostConfigure<BunnyOutboxConfiguration>(configuration);
		}
		services.AddScoped<IBunnyOutbox, BunnyOutbox<TDbContext>>();
		services.AddScoped<IBunnyOutboxProcessor, BunnyOutboxProcessor<TDbContext>>();
		services.AddHostedService<BunnyOutboxWorker>();
		return services;
    }

	/// <summary>
	/// Starts the Bunny Outbox worker associated with the specified host.
	/// </summary>
	/// <remarks>This method locates the <see cref="BunnyOutboxWorker"/> registered in the host's service collection
	/// and starts it asynchronously. Ensure that the worker is properly registered before calling this method.</remarks>
	/// <typeparam name="T">The type of the host, which must implement <see cref="IHost"/>.</typeparam>
	/// <param name="host">The host instance containing the Bunny Outbox worker. Cannot be <see langword="null"/>.</param>
	/// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
	/// <returns>The same host instance passed as the <paramref name="host"/> parameter, to allow for method chaining.</returns>
	public static async Task<T> StartBunnyOutbox<T>(this T host, CancellationToken ct = default) where T : IHost {
		var worker = host.Services
			.GetServices<IHostedService>()
			.OfType<BunnyOutboxWorker>()
			.First();

		await worker.StartAsync(ct);

		return host;
	}
}
