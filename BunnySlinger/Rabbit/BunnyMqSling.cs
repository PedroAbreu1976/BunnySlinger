using BunnySlinger.Extensions;
using RabbitMQ.Client;

namespace BunnySlinger.Rabbit
{
    public class BunnyMqSling(IChannelProvider channelProvider) : IBunnySling
    {
        public async Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default)
            where TBunny : IBunny
        {
            // need a semaphor here
	        while (!channelProvider.IsConnectionOpen || ct.IsCancellationRequested) {
		        await channelProvider.InitializeAsync(ct);
	        }
            var channel = await channelProvider.Create(ct);
            var body = bunny.Serialize();
            await channel.BasicPublishAsync(bunny.GetType().Name, string.Empty, true, new BasicProperties(),  body, ct);
        }
    }
}
