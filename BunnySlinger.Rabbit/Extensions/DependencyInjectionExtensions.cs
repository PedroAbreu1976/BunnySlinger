using BunnySlinger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Rabbit.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddBunnyMq(this IServiceCollection services, BunnyMqOptions options)
	{
		services.AddSingleton<IOptions<BunnyMqOptions>>(sp=> new BunnyMqOptionsOptions(options));
        return services.AddBunnyMqCommonServices();
	}

    public static IServiceCollection AddBunnyMq(this IServiceCollection services)
	{
		services.ConfigureOptions<BunnyMqOptionsSetup>();
		return services.AddBunnyMqCommonServices();
	}

	private static IServiceCollection AddBunnyMqCommonServices(this IServiceCollection services) 
	{
		services.AddSingleton<IChannelProvider, BunnyMqChannelProvider>();
		services.AddSingleton<IBunnySling, BunnyMqSling>();
		services.AddScoped<BunnyInterceptors>();
		services.AddSingleton<IBunnyRegister, BunnyMqRegister>();

		return services;
    }
}

