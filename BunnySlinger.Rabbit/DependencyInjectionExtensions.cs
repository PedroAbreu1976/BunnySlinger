using BunnySlinger.Options;
using Microsoft.Extensions.DependencyInjection;
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
		services.AddScoped<BunnyInterceptors>();

		services.AddSingleton<IBunnyRegister, BunnyMqRegister>();

		services.AddBunnyInterceptors(assemblies);

		return services;
    }
}

