using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BunnySlinger.Outbox.Extensions;

public static class EntityFrameworkExtensions
{
    public static ModelBuilder AddBunnyOutbox(this ModelBuilder builder)
    {
        builder.Entity<BunnyOutboxItem>().ToTable("BunnyOutbox");
        builder.Entity<BunnyFailedItem>().ToTable("BunnyFailed");
        builder.Entity<BunnyProcessedItem>().ToTable("BunnyProcessed");
        return builder;
    }

    internal static DbSet<BunnyOutboxItem> GetBunnyOutbox(this DbContext context) 
    {
        return context.Set<BunnyOutboxItem>();
    }

    internal static DbSet<BunnyFailedItem> GetBunnyFailed(this DbContext context)
    {
        return context.Set<BunnyFailedItem>();
    }

    internal static DbSet<BunnyProcessedItem> GetBunnyProcessed(this DbContext context)
    {
        return context.Set<BunnyProcessedItem>();
    }

    internal static string ToLog(this Exception ex) {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(ex.Message);
        sb.AppendLine(ex.StackTrace);
        while (ex.InnerException != null) {
	        ex = ex.InnerException;
            sb.AppendLine("==============================================");
	        sb.AppendLine(ex.Message);
	        sb.AppendLine(ex.StackTrace);
        }

        return sb.ToString();
    }
}
