using Moq;
using RabbitMQ.Client;

namespace BunnySlinger.Rabbit.Tests
{
    public class BunnyMqSlingTests
    {
        // Local test bunny for this file
        private class TestBunny : IBunny
        {
            public string Message { get; set; }
        }

        [Fact]
        public async Task SlingBunnyAsync_PublishesBunny_WhenConnectionIsOpen()
        {
            var channelProviderMock = new Mock<IChannelProvider>();
            var channelMock = new Mock<IChannel>();
            channelProviderMock.SetupGet(x => x.IsConnectionOpen).Returns(true);
            channelProviderMock.Setup(x => x.Create(It.IsAny<CancellationToken>())).ReturnsAsync(channelMock.Object);
            channelMock.Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask)
                .Verifiable();

            var sling = new BunnyMqSling(channelProviderMock.Object);
            var bunny = new TestBunny { Message = "Hello" };
            await sling.SlingBunnyAsync(bunny);
            channelMock.Verify(x => x.BasicPublishAsync(
                bunny.GetType().Name,
                string.Empty,
                true,
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SlingBunnyAsync_WaitsForConnectionOpen()
        {
            var channelProviderMock = new Mock<IChannelProvider>();
            var channelMock = new Mock<IChannel>();
            var callCount = 0;
            channelProviderMock.SetupGet(x => x.IsConnectionOpen).Returns(() => callCount > 0);
            channelProviderMock.Setup(x => x.InitializeAsync(It.IsAny<CancellationToken>())).Returns(() => { callCount++; return Task.CompletedTask; });
            channelProviderMock.Setup(x => x.Create(It.IsAny<CancellationToken>())).ReturnsAsync(channelMock.Object);
            channelMock.Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);

            var sling = new BunnyMqSling(channelProviderMock.Object);
            var bunny = new TestBunny { Message = "Wait" };
            await sling.SlingBunnyAsync(bunny);
            Assert.True(callCount > 0);
        }

        [Fact]
        public async Task SlingBunnyAsync_SerializesBunnyCorrectly()
        {
            var channelProviderMock = new Mock<IChannelProvider>();
            var channelMock = new Mock<IChannel>();
            channelProviderMock.SetupGet(x => x.IsConnectionOpen).Returns(true);
            channelProviderMock.Setup(x => x.Create(It.IsAny<CancellationToken>())).ReturnsAsync(channelMock.Object);
            ReadOnlyMemory<byte> capturedBody = default;
            channelMock.Setup(x => x.BasicPublishAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<CancellationToken>()))
                .Callback<string, string, bool, BasicProperties, ReadOnlyMemory<byte>, CancellationToken>((_, _, _, _, body, _) => capturedBody = body)
                .Returns(ValueTask.CompletedTask);

            var sling = new BunnyMqSling(channelProviderMock.Object);
            var bunny = new TestBunny { Message = "Serialize" };
            await sling.SlingBunnyAsync(bunny);
            var json = System.Text.Encoding.UTF8.GetString(capturedBody.ToArray());
            Assert.Contains("Serialize", json);
        }
    }
}
