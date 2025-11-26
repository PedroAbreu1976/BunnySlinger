using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using BunnySlinger.Outbox.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text.Json;

namespace BunnySlinger.Outbox.Tests
{
    public class BunnyOutboxProcessorTests
    {
        private static BunnyOutboxProcessor<TestDbContext> CreateProcessor(
            TestDbContext context,
            IBunnySling bunnySling,
            BunnyOutboxConfiguration? options = null)
        {
            var opts = new BunnyOutboxConfigurationOptions(options ?? new BunnyOutboxConfiguration { MaxRetryCount = 3, ExpireOlderThan = 3600 });
            var messageTypes = new BunnyMessageTypes(new[] { typeof(TestBunny).Assembly });
            return new BunnyOutboxProcessor<TestDbContext>(context, bunnySling, opts, messageTypes);
        }

        [Fact]
        public async Task ProcessAsync_ProcessesOutboxItemAndMovesToProcessed()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var bunny = new TestBunny { Message = "Hello" };
            var outboxItem = new BunnyOutboxItem
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Type = typeof(TestBunny).FullName!,
                Payload = JsonSerializer.Serialize(bunny),
                RetryCount = 0
            };
            context.GetBunnyOutbox().Add(outboxItem);
            await context.SaveChangesAsync();

            var bunnySlingMock = new Mock<IBunnySling>();
            bunnySlingMock.Setup(x => x.SlingBunnyAsync(It.IsAny<IBunny>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var processor = CreateProcessor(context, bunnySlingMock.Object);

            await processor.ProcessAsync();

            Assert.Empty(context.GetBunnyOutbox());
            Assert.Single(context.GetBunnyProcessed());
            var processed = await context.GetBunnyProcessed().FirstAsync();
            Assert.Equal(outboxItem.Id, processed.Id);
            bunnySlingMock.Verify(x => x.SlingBunnyAsync(It.IsAny<IBunny>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_OnException_IncrementsRetryAndMovesToFailedIfLimit()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var bunny = new TestBunny { Message = "Fail" };
            var outboxItem = new BunnyOutboxItem
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddHours(-2), // Expired
                Type = typeof(TestBunny).FullName!,
                Payload = JsonSerializer.Serialize(bunny),
                RetryCount = 3 // At max
            };
            context.GetBunnyOutbox().Add(outboxItem);
            await context.SaveChangesAsync();

            var bunnySlingMock = new Mock<IBunnySling>();
            bunnySlingMock.Setup(x => x.SlingBunnyAsync(It.IsAny<IBunny>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Dispatch failed"));
            var processor = CreateProcessor(context, bunnySlingMock.Object, new BunnyOutboxConfiguration { MaxRetryCount = 3, ExpireOlderThan = 3600 });

            await processor.ProcessAsync();

            Assert.Empty(context.GetBunnyOutbox());
            Assert.Single(context.GetBunnyFailed());
            var failed = await context.GetBunnyFailed().FirstAsync();
            Assert.Equal(outboxItem.Id, failed.Id);
            Assert.Contains("Dispatch failed", failed.DispatchError);
        }

        [Fact]
        public void AddToFailed_ReturnsTrue_WhenRetryCountExceedsMax()
        {
            var bunnySlingMock = new Mock<IBunnySling>();
            var context = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var processor = CreateProcessor(context, bunnySlingMock.Object, new BunnyOutboxConfiguration { MaxRetryCount = 2, ExpireOlderThan = 3600 });
            var item = new BunnyOutboxItem { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Type = "t", Payload = "p", RetryCount = 3 };
            var result = processor.GetType().GetMethod("AddToFailed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(processor, new object[] { item });
            Assert.True((bool)result);
        }

        [Fact]
        public void AddToFailed_ReturnsTrue_WhenCreatedAtIsExpired()
        {
            var bunnySlingMock = new Mock<IBunnySling>();
            var context = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var processor = CreateProcessor(context, bunnySlingMock.Object, new BunnyOutboxConfiguration { MaxRetryCount = 10, ExpireOlderThan = 60 });
            var item = new BunnyOutboxItem { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddMinutes(-120), Type = "t", Payload = "p", RetryCount = 1 };
            var result = processor.GetType().GetMethod("AddToFailed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(processor, new object[] { item });
            Assert.True((bool)result);
        }

        [Fact]
        public void AddToFailed_ReturnsFalse_WhenNotExpiredAndRetryBelowMax()
        {
            var bunnySlingMock = new Mock<IBunnySling>();
            var context = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
            var processor = CreateProcessor(context, bunnySlingMock.Object, new BunnyOutboxConfiguration { MaxRetryCount = 10, ExpireOlderThan = 60 });
            var item = new BunnyOutboxItem { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Type = "t", Payload = "p", RetryCount = 1 };
            var result = processor.GetType().GetMethod("AddToFailed", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.Invoke(processor, new object[] { item });
            Assert.False((bool)result);
        }
    }
}
