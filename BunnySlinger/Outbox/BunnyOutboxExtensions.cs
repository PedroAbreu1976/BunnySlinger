using BunnySlinger.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BunnySlinger.Outbox;

public static class BunnyOutboxExtensions
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

    public static IServiceCollection AddBunnyOutbox<TDbContext>(this IServiceCollection services, BunnyOutboxOptions options)
        where TDbContext : DbContext
    {
        services.AddSingleton<IOptions<BunnyOutboxOptions>>(sp => new BunnyOutboxOptionsOptions(options));
        return services.AddBunnyOutboxCommonServices<TDbContext>();
    }

    public static IServiceCollection AddBunnyOutbox<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.ConfigureOptions<BunnyOutboxOptionsSetup>();
        return services.AddBunnyOutboxCommonServices<TDbContext>();
    }

    private static IServiceCollection AddBunnyOutboxCommonServices<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped<IBunnyOutbox,BunnyOutbox<TDbContext>>();
        services.AddScoped<IBunnyOutboxProcessor, BunnyOutboxProcessor<TDbContext>>();
        services.AddHostedService<BunnyOutboxWorker>();
        return services;
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
