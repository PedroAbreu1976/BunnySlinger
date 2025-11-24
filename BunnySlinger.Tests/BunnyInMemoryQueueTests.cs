using BunnySlinger.InMemory;
using BunnySlinger.Tests.Bunnies;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BunnySlinger.Tests
{
    public class BunnyInMemoryQueueTests
    {
        [Fact]
        public void Constructor_SetsUpChannelWithDefaultCapacity()
        {
            var queue = new BunnyInMemoryQueue();
            Assert.NotNull(queue.Reader);
            Assert.NotNull(queue.Writer);
        }

        [Fact]
        public async Task WriterAndReader_WorkCorrectly()
        {
            var queue = new BunnyInMemoryQueue();
            var bunny = new TestBunny { Message = "Hello" };
            await queue.Writer.WriteAsync(bunny);
            var readBunny = await queue.Reader.ReadAsync();
            Assert.Equal(bunny, readBunny);
        }

        [Fact]
        public async Task OnBunnyDispatchedAsync_NoHandler_ReturnsTrue()
        {
            var queue = new BunnyInMemoryQueue();
            var bunny = new TestBunny { Message = "Test" };
            var result = await queue.OnBunnyDispatchedAsync(bunny);
            Assert.True(result);
        }

        [Fact]
        public async Task OnBunnyDispatchedAsync_WithHandler_ReturnsHandledValue()
        {
            var queue = new BunnyInMemoryQueue();
            var bunny = new TestBunny { Message = "Test" };
            bool handledCalled = false;
            queue.BunnyDispatched += async (sender, args) => {
                handledCalled = true;
                args.Handled = false;
                await Task.CompletedTask;
            };
            var result = await queue.OnBunnyDispatchedAsync(bunny);
            Assert.True(handledCalled);
            Assert.False(result);
        }
    }
}
