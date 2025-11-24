using BunnySlinger.InMemory;
using BunnySlinger.Tests.Bunnies;
using System.Threading.Tasks;
using Xunit;

namespace BunnySlinger.Tests
{
    public class BunnyInMemorySlingTests
    {
        [Fact]
        public async Task SlingBunnyAsync_WritesBunnyToQueue()
        {
            var queue = new BunnyInMemoryQueue();
            var sling = new BunnyInMemorySling(queue);
            var bunny = new TestBunny { Message = "Hello Bunny" };
            await sling.SlingBunnyAsync(bunny);
            var result = await queue.Reader.ReadAsync();
            Assert.Equal(bunny, result);
        }
    }
}

