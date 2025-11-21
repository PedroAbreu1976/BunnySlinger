using BunnySlinger.Idempotency;
using Microsoft.EntityFrameworkCore;

namespace ExampleIdempotency;

public class AppDbContext : DbContext {
	public AppDbContext() : base() { }

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
		
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		modelBuilder.AddBunnyIdempotency();
		
		base.OnModelCreating(modelBuilder);
	}
}



