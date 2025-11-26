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
	/// <summary>
	/// Adds BunnyMQ services to the specified <see cref="IServiceCollection"/> with the provided configuration options.
	/// </summary>
	/// <param name="services">The <see cref="IServiceCollection"/> to which the BunnyMQ services will be added.</param>
	/// <param name="options">The configuration options for BunnyMQ. This parameter cannot be null.</param>
	/// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBunnyMq(this IServiceCollection services, BunnyMqOptions options)
	{
		services.AddSingleton<IOptions<BunnyMqOptions>>(sp=> new BunnyMqOptionsOptions(options));
        return services.AddBunnyMqCommonServices();
	}

    /// <summary>
    /// Adds BunnyMQ services and configuration to the specified <see cref="IServiceCollection"/>.
    /// Make sure the options are set on appsettings or configuration provider.
    /// </summary>
    /// <remarks>This method configures the necessary options for BunnyMQ and registers its common services. It
    /// should be called during application startup to enable BunnyMQ functionality.</remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to which the BunnyMQ services will be added.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance, allowing for method chaining.</returns>
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
		services.AddSingleton<IConnectionFactoryProvider, ConnectionFactoryProvider>();

		return services;
    }
}

