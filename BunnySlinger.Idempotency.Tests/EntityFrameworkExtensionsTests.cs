using BunnySlinger.Idempotency.Tests.Fakes;
using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency.Tests
{
    public class EntityFrameworkExtensionsTests
    {
        [Fact]
        public void AddBunnyIdempotency_AddsBunnyLogEntity()
        {
            var modelBuilder = new ModelBuilder();
            modelBuilder.AddBunnyIdempotency();
            var model = modelBuilder.Model;
            Assert.NotNull(model.FindEntityType(typeof(BunnyLog)));
        }

        [Fact]
        public void GetBunnyLogs_ReturnsDbSetOfBunnyLog()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase("TestDbLogs")
                .Options;
            using var context = new TestDbContext(options);
            var dbSet = context.GetBunnyLogs();
            Assert.IsAssignableFrom<DbSet<BunnyLog>>(dbSet);
        }

        [Fact]
        public void CreateHashCode_ReturnsConsistentHashCode()
        {
            var bunnyLog = new BunnyLog
            {
                BunnyID = "id1",
                BunnyType = "typeA",
                BunnyCatcherType = "catcherB"
            };
            var hash1 = bunnyLog.CreateHashCode();
            var hash2 = bunnyLog.CreateHashCode();
            Assert.Equal(hash1, hash2);
        }
    }
}
