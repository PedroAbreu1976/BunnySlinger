using BunnySlinger.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;


namespace BunnySlinger.Rabbit;

public class ConnectionFactoryProvider(IOptions<BunnyMqConfiguration> bunnyMqOptions) : IConnectionFactoryProvider {
	public IConnectionFactory GetFactory() {
		return bunnyMqOptions.Value.GetFactory();
    }
}
