using BunnySlinger.Outbox.Extensions;

using Microsoft.EntityFrameworkCore;


namespace BunnySlinger.Outbox.Tests.Fakes
{
	public class TestDbContext : DbContext
	{
		public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.AddBunnyOutbox();
            base.OnModelCreating(modelBuilder);
		}
	}
}
