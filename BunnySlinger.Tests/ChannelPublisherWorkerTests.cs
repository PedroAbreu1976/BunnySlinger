using BunnySlinger.InMemory;
using BunnySlinger.Tests.Bunnies;

namespace BunnySlinger.Tests
{
    public class ChannelPublisherWorkerTests
    {
        // Helper to expose ExecuteAsync for testing
        private class TestChannelPublisherWorker : ChannelPublisherWorker
        {
            public TestChannelPublisherWorker(BunnyInMemoryQueue queue) : base(queue) { }
            public async Task RunExecuteAsync(CancellationToken ct) => await ExecuteAsync(ct);
        }

        [Fact]
        public async Task ExecuteAsync_DispatchesBunny_WhenBunnyIsAvailable()
        {
            // Arrange
            var queue = new BunnyInMemoryQueue();
            var bunny = new TestBunny { Message = "Hello" };
            bool dispatched = false;
            queue.BunnyDispatched += async (sender, args) =>
            {
                dispatched = true;
                args.Handled = true;
                await Task.CompletedTask;
            };
            await queue.Writer.WriteAsync(bunny);
            var worker = new TestChannelPublisherWorker(queue);
            var cts = new CancellationTokenSource();

            // Act
            var runTask = worker.RunExecuteAsync(cts.Token);
            await Task.Delay(100); // Let the worker process
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

            // Assert
            Assert.True(dispatched);
        }

        [Fact]
        public async Task ExecuteAsync_Stops_WhenCancelled()
        {
            // Arrange
            var queue = new BunnyInMemoryQueue();
            var worker = new TestChannelPublisherWorker(queue);
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<TaskCanceledException>(() => worker.RunExecuteAsync(cts.Token));
        }

        [Fact]
        public async Task ExecuteAsync_DoesNotDispatch_WhenNoBunnyAvailable()
        {
            // Arrange
            var queue = new BunnyInMemoryQueue();
            bool dispatched = false;
            queue.BunnyDispatched += async (sender, args) =>
            {
                dispatched = true;
                await Task.CompletedTask;
            };
            var worker = new TestChannelPublisherWorker(queue);
            var cts = new CancellationTokenSource();

            // Act
            var runTask = worker.RunExecuteAsync(cts.Token);
            await Task.Delay(100);
            cts.Cancel();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);

            // Assert
            Assert.False(dispatched);
        }
    }
}
