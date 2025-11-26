using BunnySlinger.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
		services.AddSingleton<IBunnySling, BunnyMqSling>();
		services.AddScoped<BunnyInterceptors>();
		services.AddSingleton<IBunnyRegister, BunnyMqRegister>();
		services.AddSingleton<IConnectionFactoryProvider, ConnectionFactoryProvider>();

		return services;
    }
}

