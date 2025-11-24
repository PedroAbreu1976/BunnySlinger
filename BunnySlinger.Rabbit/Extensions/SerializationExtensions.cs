using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;


namespace BunnySlinger.Rabbit.Extensions;

internal static class SerializationExtensions {
	internal static T? Deserialize<T>(this BasicDeliverEventArgs e) where T : IBunny {
		return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(e.Body.Span));
	}

	internal static ReadOnlyMemory<byte> Serialize<T>(this T message) where T : IBunny {
		var json = JsonSerializer.Serialize(message, message.GetType());
		var bytes = Encoding.UTF8.GetBytes(json);
		return new ReadOnlyMemory<byte>(bytes);
	}
}
