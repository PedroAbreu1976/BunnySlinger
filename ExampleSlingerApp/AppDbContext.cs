using BunnySlinger.Outbox.Extensions;

using Microsoft.EntityFrameworkCore;


namespace ExampleSlingerApp;

public class AppDbContext : DbContext {
	public AppDbContext() : base() { }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.AddBunnyOutbox();
		
		base.OnModelCreating(modelBuilder);
	}
}



