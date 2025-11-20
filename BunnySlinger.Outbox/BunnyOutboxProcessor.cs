using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BunnySlinger.Outbox
{
    public class BunnyOutboxProcessor<TContext>(
        TContext context,
        IBunnySling bunnySling,
        IOptions<BunnyOutboxOptions> options) : IBunnyOutboxProcessor
        where TContext : DbContext
    {
        public async Task ProcessAsync(CancellationToken stoppingToken = default)
        {
            var entities = await context
                .GetBunnyOutbox()
                .ToListAsync(stoppingToken);

            foreach (var entity in entities)
            {
                try
                {
                    var bunny = JsonSerializer.Deserialize(entity.Payload, Type.GetType(entity.Type)!) as IBunny;
                    await bunnySling.SlingBunnyAsync(bunny!, stoppingToken);

                    context.Add(new BunnyProcessedItem
                    {
                        Id = entity.Id,
                        CreatedAt = entity.CreatedAt,
                        Type = entity.Type,
                        Payload = entity.Payload,
                        DispatchedAt = DateTime.UtcNow,
                        LastDispatchError = entity.DispatchError,
                        RetryCount = entity.RetryCount,
                    });
                    context.Remove(entity);
                }
                catch (Exception ex) {
	                var dispatchError = ex.ToLog();
                    entity.RetryCount += 1;
                    entity.DispatchError = dispatchError;

                    if (entity.RetryCount >= options.Value.MaxRetryCount || entity.CreatedAt < DateTime.UtcNow.AddMinutes(-options.Value.ExpireOlderThan))
                    {
                        var failedItem = new BunnyFailedItem
                        {
                            Id = entity.Id,
                            CreatedAt = entity.CreatedAt,
                            Type = entity.Type,
                            Payload = entity.Payload,
                            LastDispatchedTryAt = DateTime.UtcNow,
                            DispatchError = dispatchError,
                            RetryCount = entity.RetryCount,
                        };
                        context.Add(failedItem);
                        context.Remove(entity);
                    }
                }
            }

            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
