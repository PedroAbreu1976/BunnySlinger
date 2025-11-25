using BunnySlinger.Outbox.Options;
using BunnySlinger.Outbox.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace BunnySlinger.Outbox.Tests
{
    public class DependencyInjectionExtensionsTests
    {
        [Fact]
        public void AddBunnyOutbox_WithOptions_RegistersExpectedServices()
        {
            var options = new BunnyOutboxOptions();
            var provider = ServiceHelpers.CreateServices(options);

            var opts = provider.GetService<IOptions<BunnyOutboxOptions>>();
            Assert.NotNull(opts);
            Assert.Equal(options, opts.Value);

            Assert.NotNull(provider.GetService<IBunnyOutbox>());
            Assert.NotNull(provider.GetService<IBunnyOutboxProcessor>());
            Assert.NotNull(provider.GetService<IHostedService>());
        }

        [Fact]
        public void AddBunnyOutbox_WithoutOptions_RegistersExpectedServices()
        {
            var provider = ServiceHelpers.CreateServices();

            Assert.NotNull(provider.GetService<IBunnyOutbox>());
            Assert.NotNull(provider.GetService<IBunnyOutboxProcessor>());
            Assert.NotNull(provider.GetService<IHostedService>());
            Assert.NotNull(provider.GetService<IOptions<BunnyOutboxOptions>>());
        }

        [Fact]
        public async Task StartBunnyOutbox_CallsWorkerStartAsync()
        {
            var workerMock = new Mock<BunnySlinger.Outbox.BunnyOutboxWorker>(MockBehavior.Strict, null, null);
            workerMock.Setup(w => w.StartAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

            var services = new ServiceCollection();
            services.AddSingleton<IHostedService>(workerMock.Object);
            var provider = services.BuildServiceProvider();

            var hostMock = new Mock<IHost>();
            hostMock.Setup(h => h.Services).Returns(provider);

            var result = await BunnySlinger.Outbox.Extensions.DependencyInjectionExtensions
	            .StartBunnyOutbox(hostMock.Object);

            workerMock.Verify(w => w.StartAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(hostMock.Object, result);
        }
    }
}
