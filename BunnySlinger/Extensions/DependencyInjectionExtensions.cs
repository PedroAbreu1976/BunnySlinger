using BunnySlinger.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace BunnySlinger.Extensions;

public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddBunnyInMemory(this IServiceCollection services, params Assembly[] assemblies) {

		services.AddSingleton<BunnyInMemoryQueue>();
        services.AddHostedService<ChannelPublisherWorker>();
        services.AddSingleton<IBunnySling, BunnyInMemorySling>();
        services.AddScoped<BunnyInterceptors>();

        services.AddSingleton<IBunnyRegister>(sp => {
	        var bunnyInMemoryQueue = sp.GetRequiredService<BunnyInMemoryQueue>();
	        var bunnyMessageTypes = sp.GetService<BunnyMessageTypes>();
			var bunnyHandlerTypes = sp.GetService<BunnyHandlerTypes>();

            var result = new BunnyInMemoryRegister(sp, bunnyInMemoryQueue);

            if (bunnyMessageTypes is not null) {
	            bunnyMessageTypes.MessageTypes.ForEach(x => result.AddBunny(x));
            }

            if (bunnyHandlerTypes is not null) {
	            foreach (Type handlerType in bunnyHandlerTypes.HandlerTypes.Keys)
	            {
		            result.AddBunnyCatcher(handlerType, bunnyHandlerTypes.HandlerTypes[handlerType]);
	            }
            }

	        return result;
        });

        services.AddBunnyInterceptors(assemblies);

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

    public static async Task<T> StartBunnyObserver<T>(this T host, CancellationToken ct = default)
		where T : IHost
	{
		var consumer = host.Services.GetRequiredService<IBunnyRegister>();
		await consumer.RegisterAsync();

		var channelPublisherWorker = host.Services.GetServices<IHostedService>().
			OfType<ChannelPublisherWorker>().
			FirstOrDefault();
		channelPublisherWorker?.StartAsync(ct);

        return host;
    }
}

