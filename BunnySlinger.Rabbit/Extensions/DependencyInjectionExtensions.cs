using BunnySlinger.InMemory;
using BunnySlinger.Options;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger.Rabbit.Extensions;

/// <summary>
/// Provides extension methods for registering BunnyMQ services with an <see cref="IServiceCollection"/>.
/// </summary>
/// <remarks>These extension methods allow for the configuration and registration of BunnyMQ services in a
/// dependency injection container. BunnyMQ is a lightweight messaging library for RabbitMQ, and these methods simplify
/// the setup process by adding the required services.</remarks>
public static class DependencyInjectionExtensions
{
	public static IServiceCollection AddBunnyMq(this IServiceCollection services, Action<BunnyMqConfiguration>? configure = null)
	{
		services.ConfigureOptions<BunnyMqConfigurationSetup>();
		if (configure != null) {
			services.PostConfigure(configure);
		}

		services.AddSingleton<IChannelProvider, BunnyMqChannelProvider>();
		services.AddSingleton<IBunnySling, BunnySling>();
		services.AddScoped<BunnyInterceptors>();
		services.AddSingleton<IBunnyBroker, BunnyMqBroker>();
		services.AddSingleton<IBunnyRegister, BunnyRegister>();
        services.AddSingleton<IConnectionFactoryProvider, ConnectionFactoryProvider>();

		return services;
    }
}

