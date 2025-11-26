using BunnySlinger.Outbox.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace BunnySlinger.Outbox.Tests
{
    public partial class BunnyOutboxWorkerTests
    {
        [Fact]
        public async Task ExecuteAsync_CallsProcessAsyncAndRespectsFrequency()
        {
            var helper = new BunnyOutboxWorkerTestsHelper();
            var options = new OptionsWrapper<BunnyOutboxConfiguration>(new BunnyOutboxConfiguration { ProcessorFrequency = 10 });
            var worker = new BunnyOutboxWorker(helper.ServiceScopeFactory, options);

            using var cts = new CancellationTokenSource();
            cts.CancelAfter(50); // Cancel after a short time

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            helper.ProcessorMock.Verify(p => p.ProcessAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteAsync_StopsOnCancellation()
        {
            // Arrange
            var helper = new BunnyOutboxWorkerTestsHelper();
            var options = new OptionsWrapper<BunnyOutboxConfiguration>(new BunnyOutboxConfiguration { ProcessorFrequency = 10 });
            var worker = new BunnyOutboxWorker(helper.ServiceScopeFactory, options);

            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            // Act
            await worker.StartAsync(cts.Token);

            // Assert
            helper.ProcessorMock.Verify(p => p.ProcessAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
