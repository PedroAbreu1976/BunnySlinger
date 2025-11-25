using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BunnySlinger.Outbox.Tests
{
    public class BunnyOutboxTests
    {
        [Fact]
        public async Task QueueBunnyAsync_AddsBunnyToOutboxAndReturnsTrue()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new TestDbContext(options);
            var outbox = new BunnyOutbox<TestDbContext>(context);
            var bunny = new TestBunny { Message = "Hello World" };

            var result = await outbox.QueueBunnyAsync(bunny);

            Assert.True(result);
            var items = await context.GetBunnyOutbox().ToListAsync();
            Assert.Single(items);
            var item = items[0];
            Assert.Equal(typeof(TestBunny).FullName, item.Type);
            Assert.Equal(JsonSerializer.Serialize(bunny), item.Payload);
            Assert.Equal(0, item.RetryCount);
            Assert.Null(item.DispatchError);
        }
    }
}
