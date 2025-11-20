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

        var handlerTypeDic = assemblies.GetMessageHandlerTypes();
        foreach (Type handlerType in handlerTypeDic.Keys)
        {
	        services.AddScoped(handlerType);
        }

        services.AddSingleton<IBunnyRegister>(sp => {
	        var bunnyInMemoryQueue = sp.GetRequiredService<BunnyInMemoryQueue>();

            var result = new BunnyInMemoryRegister(sp, bunnyInMemoryQueue);
	        foreach (Type handlerType in handlerTypeDic.Keys)
	        {
		        result.AddBunnyCatcher(handlerType, handlerTypeDic[handlerType]);
	        }

	        return result;
        });

        services.AddInterceptors(assemblies);

        return services;
	}

	public static IServiceCollection AddInterceptors(this IServiceCollection services, params Assembly[] assemblies) 
	{
		var interceptorTypes = assemblies.GetInterceptorTypes();
		foreach (var type in interceptorTypes)
		{
			services.AddScoped(typeof(IBunnyInterceptor), type);
		}

		services.AddScoped<BunnyInterceptors>();
		
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

