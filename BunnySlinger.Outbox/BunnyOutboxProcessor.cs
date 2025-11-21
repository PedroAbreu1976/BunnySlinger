using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BunnySlinger.Outbox;

public class BunnyOutboxProcessor<TContext>(
	TContext context,
	IBunnySling bunnySling,
	IOptions<BunnyOutboxOptions> options,
	BunnyMessageTypes bunnyMessageTypes) : IBunnyOutboxProcessor where TContext : DbContext {
	public async Task ProcessAsync(CancellationToken stoppingToken = default) {
		var processedDbSet = context.GetBunnyProcessed();
		var outboxDbSet = context.GetBunnyOutbox();
		var failedDbSet = context.GetBunnyFailed();

		var entities = await context.GetBunnyOutbox().ToListAsync(stoppingToken);
		if (entities.Any()) {
			foreach (var entity in entities) {
				try {
					var bunny = JsonSerializer.Deserialize(entity.Payload, bunnyMessageTypes[entity.Type]) as IBunny;
					await bunnySling.SlingBunnyAsync(bunny!, stoppingToken);

					processedDbSet.Add(
						new BunnyProcessedItem {
							Id = entity.Id,
							CreatedAt = entity.CreatedAt,
							Type = entity.Type,
							Payload = entity.Payload,
							DispatchedAt = DateTime.UtcNow,
							LastDispatchError = entity.DispatchError,
							RetryCount = entity.RetryCount,
						});
					outboxDbSet.Remove(entity);
				}
				catch (Exception ex) {
					var dispatchError = ex.ToLog();
					entity.RetryCount += 1;
					entity.DispatchError = dispatchError;

					if (AddToFailed(entity)) {
						failedDbSet.Add(
							new BunnyFailedItem {
								Id = entity.Id,
								CreatedAt = entity.CreatedAt,
								Type = entity.Type,
								Payload = entity.Payload,
								LastDispatchedTryAt = DateTime.UtcNow,
								DispatchError = dispatchError,
								RetryCount = entity.RetryCount,
							});
						outboxDbSet.Remove(entity);
					}
				}
			}

			await context.SaveChangesAsync(stoppingToken);
		}
	}

	private bool AddToFailed(BunnyOutboxItem entity) {
		return entity.RetryCount >= options.Value.MaxRetryCount ||
		       entity.CreatedAt < DateTime.UtcNow.AddMinutes(-options.Value.ExpireOlderThan);

	}
}
