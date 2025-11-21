using BunnySlinger.Outbox.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BunnySlinger.Outbox
{
    public class BunnyOutbox<TContext>(TContext context) : IBunnyOutbox
        where TContext : DbContext
    {
        public async Task<bool> QueueBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny
        {
            context.GetBunnyOutbox().Add(new BunnyOutboxItem
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Type = typeof(TBunny).FullName!,
                Payload = JsonSerializer.Serialize(bunny),
            });
            var result = await context.SaveChangesAsync(ct);
            return result > 0;
        }
    }
}
