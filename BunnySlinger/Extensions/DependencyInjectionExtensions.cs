using BunnySlinger.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace BunnySlinger.Extensions;

/// <summary>
/// Provides extension methods for configuring Bunny-related services in an <see cref="IServiceCollection"/>  and
/// managing Bunny message handling within an application.
/// </summary>
/// <remarks>This class includes methods for registering in-memory Bunny queues, message handlers, interceptors, 
/// and other related services. It also provides functionality to start Bunny observers for message processing. These
/// methods are designed to simplify the integration of Bunny messaging into .NET applications.</remarks>
public static class DependencyInjectionExtensions
{
	/// <summary>
	/// Adds in-memory implementations of Bunny services to the specified <see cref="IServiceCollection"/>.
	/// </summary>
	/// <remarks>This method registers the following services: <list type="bullet"> <item><description><see
	/// cref="BunnyInMemoryQueue"/> as a singleton.</description></item> <item><description><see
	/// cref="ChannelPublisherWorker"/> as a hosted service.</description></item> <item><description><see
	/// cref="IBunnySling"/> with an implementation of <see cref="BunnyInMemorySling"/> as a
	/// singleton.</description></item> <item><description><see cref="BunnyInterceptors"/> as a scoped
	/// service.</description></item> <item><description><see cref="IBunnyRegister"/> with an implementation of <see
	/// cref="BunnyInMemoryRegister"/> as a singleton.</description></item> </list> Use this method to configure Bunny
	/// services for in-memory operation, typically for testing or development scenarios.</remarks>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the Bunny services will be added.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
	public static IServiceCollection AddBunnyInMemory(this IServiceCollection services) {

		services.AddSingleton<BunnyInMemoryQueue>();
        services.AddHostedService<ChannelPublisherWorker>();
        services.AddSingleton<IBunnySling, BunnyInMemorySling>();
        services.AddScoped<BunnyInterceptors>();

        services.AddSingleton<IBunnyRegister, BunnyInMemoryRegister>();

        return services;
	}

	/// <summary>
	/// Adds Bunny-related services to the specified <see cref="IServiceCollection"/>.
	/// </summary>
	/// <remarks>This method registers a singleton instance of <see cref="BunnyMessageTypes"/> in the service
	/// collection, initialized with the provided assemblies.</remarks>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
	/// <param name="assemblies">An array of <see cref="Assembly"/> instances used to discover Bunny message types.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
	public static IServiceCollection AddBunnies(this IServiceCollection services, params Assembly[] assemblies) {
		services.AddSingleton(_ => new BunnyMessageTypes(assemblies));
		
        return services;
	}

	/// <summary>
	/// Registers Bunny message handlers and their dependencies into the specified service collection.
	/// </summary>
	/// <remarks>This method scans the provided assemblies for types that implement Bunny message handler interfaces
	/// and registers them with a scoped lifetime in the dependency injection container. It also registers a singleton
	/// instance of <see cref="BunnyHandlerTypes"/> to manage the discovered handler types.</remarks>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the handlers will be added.</param>
	/// <param name="assemblies">An array of <see cref="Assembly"/> instances to scan for Bunny message handler types.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
	public static IServiceCollection AddBunnyHandlers(this IServiceCollection services, params Assembly[] assemblies)
	{
		services.AddSingleton(_ => new BunnyHandlerTypes(assemblies));

		var handlerTypeDic = assemblies.GetBunnyHandlerTypes();
		foreach (Type handlerType in handlerTypeDic.Keys)
		{
			services.AddScoped(handlerType);
		}

        return services;
	}

	/// <summary>
	/// Registers Bunny interceptors from the specified assemblies into the service collection.
	/// </summary>
	/// <remarks>This method scans the provided assemblies for types that implement the <see
	/// cref="IBunnyInterceptor"/> interface and registers them with a scoped lifetime in the service collection. Ensure
	/// that the assemblies provided contain the desired interceptor implementations.</remarks>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the interceptors will be added.</param>
	/// <param name="assemblies">An array of assemblies to scan for types implementing <see cref="IBunnyInterceptor"/>.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBunnyInterceptors(this IServiceCollection services, params Assembly[] assemblies) 
	{
		var interceptorTypes = assemblies.GetBunnyInterceptorTypes();
		foreach (var type in interceptorTypes)
		{
			services.AddScoped(typeof(IBunnyInterceptor), type);
		}

		return services;
	}

	/// <summary>
	/// Configures and starts the Bunny observer for the specified host.
	/// </summary>
	/// <remarks>This method initializes the Bunny observer by registering message types and handlers with the <see
	/// cref="IBunnyRegister"/> service. It also starts the <see cref="ChannelPublisherWorker"/> service, if available, to
	/// handle message publishing.</remarks>
	/// <typeparam name="T">The type of the host, which must implement <see cref="IHost"/>.</typeparam>
	/// <param name="host">The host instance on which the Bunny observer will be configured and started.</param>
	/// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the operation to complete.</param>
	/// <returns>The same host instance, allowing for method chaining.</returns>
	public static async Task<T> StartBunnyObserver<T>(this T host, CancellationToken ct = default) where T : IHost {
		var consumer = host.Services.GetRequiredService<IBunnyRegister>();
		var bunnyMessageTypes = host.Services.GetService<BunnyMessageTypes>();
		var bunnyHandlerTypes = host.Services.GetService<BunnyHandlerTypes>();
		var channelPublisherWorker =
			host.Services.GetServices<IHostedService>().OfType<ChannelPublisherWorker>().FirstOrDefault();

        bunnyMessageTypes?.MessageTypes.ForEach(x => consumer.AddBunny(x));
		bunnyHandlerTypes?.HandlerTypes.Keys.ForEach(x => consumer.AddBunnyCatcher(x, bunnyHandlerTypes.HandlerTypes[x]));
		await consumer.RegisterAsync();

		channelPublisherWorker?.StartAsync(ct);

		return host;
	}
}

