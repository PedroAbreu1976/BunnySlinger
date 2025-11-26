using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency;

/// <summary>
/// Provides extension methods for enhancing Entity Framework Core functionality with custom behaviors related to the
/// <see cref="BunnyLog"/> entity.
/// </summary>
/// <remarks>This static class includes methods for configuring the <see cref="BunnyLog"/> entity in the model,
/// retrieving its associated <see cref="DbSet{TEntity}"/>, and generating hash codes for instances of <see
/// cref="BunnyLog"/>. These methods are designed to simplify common operations and improve maintainability when working
/// with <see cref="BunnyLog"/> in an Entity Framework Core context.</remarks>
public static class EntityFrameworkExtensions
{
    /// <summary>
    /// Configures the <see cref="ModelBuilder"/> to include the mapping for the <see cref="BunnyLog"/> entity.
    /// </summary>
    /// <param name="builder">The <see cref="ModelBuilder"/> instance to configure.</param>
    /// <returns>The configured <see cref="ModelBuilder"/> instance.</returns>
    public static ModelBuilder AddBunnyIdempotency(this ModelBuilder builder)
    {
        builder.Entity<BunnyLog>().ToTable("BunnyLog");
        return builder;
    }

    /// <summary>
    /// Retrieves the <see cref="DbSet{TEntity}"/> representing the collection of <see cref="BunnyLog"/> entities in the
    /// database context.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance from which to retrieve the <see cref="BunnyLog"/> entities.</param>
    /// <returns>A <see cref="DbSet{TEntity}"/> that can be used to query and interact with the <see cref="BunnyLog"/> entities
    /// in the database.</returns>
    public static DbSet<BunnyLog> GetBunnyLogs(this DbContext context) 
    {
        return context.Set<BunnyLog>();
    }

    /// <summary>
    /// Generates a hash code for the specified <see cref="BunnyLog"/> instance.
    /// </summary>
    /// <param name="bunnyLog">The <see cref="BunnyLog"/> instance for which to generate the hash code.</param>
    /// <returns>An integer representing the hash code of the <paramref name="bunnyLog"/> instance, based on its <c>BunnyID</c>,
    /// <c>BunnyType</c>, and <c>BunnyCatcherType</c> properties.</returns>
    public static int CreateHashCode(this BunnyLog bunnyLog) {
	    return HashCode.Combine(bunnyLog.BunnyID, bunnyLog.BunnyType, bunnyLog.BunnyCatcherType);
    }
}
