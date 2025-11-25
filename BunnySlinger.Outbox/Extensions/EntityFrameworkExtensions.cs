using Microsoft.EntityFrameworkCore;
using System.Text;

namespace BunnySlinger.Outbox.Extensions;

/// <summary>
/// Provides extension methods for configuring and accessing Bunny-related entities in Entity Framework.
/// </summary>
/// <remarks>This static class includes methods to configure the database model for Bunny-related entities and to
/// retrieve specific <see cref="DbSet{TEntity}"/> instances for working with Bunny outbox, failed, and processed items.
/// These extensions are designed to simplify the integration of Bunny-related data into an Entity Framework
/// context.</remarks>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Configures the <see cref="ModelBuilder"/> to include the Bunny Outbox entities.
    /// </summary>
    /// <remarks>This method maps the <see cref="BunnyOutboxItem"/>, <see cref="BunnyFailedItem"/>, and <see
    /// cref="BunnyProcessedItem"/> entities to the "BunnyOutbox", "BunnyFailed", and "BunnyProcessed" database tables,
    /// respectively.</remarks>
    /// <param name="builder">The <see cref="ModelBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="ModelBuilder"/> instance.</returns>
    public static ModelBuilder AddBunnyOutbox(this ModelBuilder builder)
    {
        builder.Entity<BunnyOutboxItem>().ToTable("BunnyOutbox");
        builder.Entity<BunnyFailedItem>().ToTable("BunnyFailed");
        builder.Entity<BunnyProcessedItem>().ToTable("BunnyProcessed");
        return builder;
    }

    /// <summary>
    /// Retrieves the <see cref="DbSet{TEntity}"/> representing the Bunny outbox items from the specified database
    /// context.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance from which to retrieve the Bunny outbox items.</param>
    /// <returns>A <see cref="DbSet{TEntity}"/> of <see cref="BunnyOutboxItem"/> representing the Bunny outbox items in the
    /// database.</returns>
    public static DbSet<BunnyOutboxItem> GetBunnyOutbox(this DbContext context) 
    {
        return context.Set<BunnyOutboxItem>();
    }

    /// <summary>
    /// Retrieves the <see cref="DbSet{TEntity}"/> representing the collection of failed bunny items in the database.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance used to access the database.</param>
    /// <returns>A <see cref="DbSet{TEntity}"/> of <see cref="BunnyFailedItem"/> representing the failed bunny items.</returns>
    public static DbSet<BunnyFailedItem> GetBunnyFailed(this DbContext context)
    {
        return context.Set<BunnyFailedItem>();
    }

    /// <summary>
    /// Retrieves the <see cref="DbSet{TEntity}"/> for the <see cref="BunnyProcessedItem"/> entity type from the
    /// specified <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance from which to retrieve the <see cref="DbSet{TEntity}"/>.</param>
    /// <returns>A <see cref="DbSet{TEntity}"/> representing the collection of <see cref="BunnyProcessedItem"/> entities in the
    /// database.</returns>
    public static DbSet<BunnyProcessedItem> GetBunnyProcessed(this DbContext context)
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
