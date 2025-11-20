using BunnySlinger.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace BunnySlinger.Extensions;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddBunnyInMemory(this IServiceCollection services) {

		services.AddSingleton<BunnyInMemoryQueue>();
        services.AddHostedService<ChannelPublisherWorker>();
        services.AddSingleton<IBunnySling, BunnyInMemorySling>();
        services.AddScoped<BunnyInterceptors>();

        services.AddSingleton<IBunnyRegister, BunnyInMemoryRegister>();

        return services;
	}

	public static IServiceCollection AddBunnies(this IServiceCollection services, params Assembly[] assemblies) {
		services.AddSingleton(_ => new BunnyMessageTypes(assemblies));
		
        return services;
	}

	public static IServiceCollection AddBunnyHandlers(this IServiceCollection services, params Assembly[] assemblies)
	{
		services.AddSingleton(_ => new BunnyHandlerTypes(assemblies));

		var handlerTypeDic = assemblies.GetMessageHandlerTypes();
		foreach (Type handlerType in handlerTypeDic.Keys)
		{
			services.AddScoped(handlerType);
		}

        return services;
	}

    public static IServiceCollection AddBunnyInterceptors(this IServiceCollection services, params Assembly[] assemblies) 
	{
		var interceptorTypes = assemblies.GetInterceptorTypes();
		foreach (var type in interceptorTypes)
		{
			services.AddScoped(typeof(IBunnyInterceptor), type);
		}

		return services;
	}

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

