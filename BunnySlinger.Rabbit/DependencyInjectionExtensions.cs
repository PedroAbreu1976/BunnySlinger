using BunnySlinger.InMemory;
using BunnySlinger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

using BunnySlinger.Extensions;


namespace BunnySlinger.Rabbit;

public static class DependencyInjectionExtensions
{
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
}

