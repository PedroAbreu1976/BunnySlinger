using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Tests.Fakes;
using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Outbox.Tests
{
    public class EntityFrameworkExtensionsTests
    {
        [Fact]
        public void AddBunnyOutbox_AddsExpectedEntities()
        {
            var modelBuilder = new ModelBuilder();
            modelBuilder.AddBunnyOutbox();
            var model = modelBuilder.Model;
            Assert.NotNull(model.FindEntityType(typeof(BunnyOutboxItem)));
            Assert.NotNull(model.FindEntityType(typeof(BunnyFailedItem)));
            Assert.NotNull(model.FindEntityType(typeof(BunnyProcessedItem)));
        }

        [Fact]
        public void GetBunnyOutbox_ReturnsDbSetOfBunnyOutboxItem()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var dbSet = context.GetBunnyOutbox();
            Assert.IsAssignableFrom<DbSet<BunnyOutboxItem>>(dbSet);
        }

        [Fact]
        public void GetBunnyFailed_ReturnsDbSetOfBunnyFailedItem()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var dbSet = context.GetBunnyFailed();
			Assert.IsAssignableFrom<DbSet<BunnyFailedItem>>(dbSet);
        }

        [Fact]
        public void GetBunnyProcessed_ReturnsDbSetOfBunnyProcessedItem()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var dbSet = context.GetBunnyProcessed();
            Assert.IsAssignableFrom<DbSet<BunnyProcessedItem>>(dbSet);
        }

        [Fact]
        public void ToLog_ReturnsExceptionDetailsIncludingInnerExceptions()
        {
            var inner = new InvalidOperationException("Inner message");
            var ex = new Exception("Outer message", inner);
            var log = typeof(EntityFrameworkExtensions)
                .GetMethod("ToLog", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .Invoke(null, new object[] { ex }) as string;
            Assert.Contains("Outer message", log);
            Assert.Contains("Inner message", log);
            Assert.Contains("==============================================", log);
        }
    }
}
