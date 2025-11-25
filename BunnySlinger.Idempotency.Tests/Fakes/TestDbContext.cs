using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency.Tests.Fakes
{
	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.AddBunnyIdempotency();
            base.OnModelCreating(modelBuilder);
		}
	}
}
