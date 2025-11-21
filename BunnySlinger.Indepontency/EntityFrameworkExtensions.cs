using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency;

public static class EntityFrameworkExtensions
{
    public static ModelBuilder AddBunnyIdempotency(this ModelBuilder builder)
    {
        builder.Entity<BunnyLog>().ToTable("BunnyLog");
        return builder;
    }

    public static DbSet<BunnyLog> GetBunnyLogs(this DbContext context) 
    {
        return context.Set<BunnyLog>();
    }

    public static int CreateHashCode(this BunnyLog bunnyLog) {
	    return HashCode.Combine(bunnyLog.BunnyID, bunnyLog.BunnyType, bunnyLog.BunnyCatcherType);
    }
}
