using RabbitMQ.Client;


namespace BunnySlinger.Rabbit;

public interface IConnectionFactoryProvider {
	IConnectionFactory GetFactory();
}
