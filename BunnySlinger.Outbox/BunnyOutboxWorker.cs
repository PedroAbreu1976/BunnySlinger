using BunnySlinger.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox
{
    public class BunnyOutboxWorker(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<BunnyOutboxOptions> options) : BackgroundService
    {
	    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateAsyncScope();
                var bunnyOutboxProcessor = scope.ServiceProvider.GetRequiredService<IBunnyOutboxProcessor>();
                await bunnyOutboxProcessor.ProcessAsync();

                await Task.Delay(TimeSpan.FromMilliseconds(options.Value.ProcessorFrequency), stoppingToken);
            }
        }
    }
}
