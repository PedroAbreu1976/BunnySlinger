using BunnySlinger.Extensions;
using BunnySlinger.InMemory;
using BunnySlinger.Options;
using BunnySlinger.Rabbit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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

    public static IServiceCollection AddBunnyMq(this IServiceCollection services, BunnyMqOptions options, params Assembly[] assemblies)
	{
		services.AddSingleton<IOptions<BunnyMqOptions>>(sp=> new BunnyMqOptionsOptions(options));
        return services.AddBunnyMqCommonServices(assemblies);
	}

    public static IServiceCollection AddBunnyMq(this IServiceCollection services, params Assembly[] assemblies)
	{
		services.ConfigureOptions<BunnyMqOptionsSetup>();
		return services.AddBunnyMqCommonServices(assemblies);
	}

	private static IServiceCollection AddBunnyMqCommonServices(this IServiceCollection services, params Assembly[] assemblies) 
	{
		services.AddSingleton<IChannelProvider, BunnyMqChannelProvider>();
		services.AddSingleton<IBunnySling, BunnyMqSling>();

		var messageTypes = assemblies.GetMessageTypes();
		var handlerTypeDic = assemblies.GetMessageHandlerTypes();
		
		
		foreach (Type handlerType in handlerTypeDic.Keys)
		{
			services.AddScoped(handlerType);
		}

		services.AddSingleton<IBunnyRegister>(sp => {
			var channelProvider = sp.GetRequiredService<IChannelProvider>();
			var result = new BunnyMqRegister(channelProvider, sp);
			messageTypes.ForEach(x => {
				result.AddBunny(x);
			});
			foreach (Type handlerType in handlerTypeDic.Keys)
			{
				result.AddBunnyCatcher(handlerType, handlerTypeDic[handlerType]);
			}

			return result;
		});

		services.AddInterceptors(assemblies);

		return services;
    }

	private static IServiceCollection AddInterceptors(this IServiceCollection services, params Assembly[] assemblies) 
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

