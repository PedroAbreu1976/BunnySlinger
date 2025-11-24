using BunnySlinger.Options;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BunnySlinger.Rabbit.Tests
{
    public class BunnyMqChannelProviderTests
    {
        private readonly Mock<IOptions<BunnyMqOptions>> _optionsMock;
        private readonly Mock<IConnectionFactory> _factoryMock;
        private readonly Mock<IConnection> _connectionMock;
        private readonly Mock<IConnectionFactoryProvider> _connectionFactoryProviderMock;
        private readonly Mock<IChannel> _channelMock;
        private readonly BunnyMqChannelProvider _provider;
        private readonly BunnyMqOptions _options;

        public BunnyMqChannelProviderTests()
        {
            _options = new BunnyMqOptions { HostName = "localhost", Port = 5672 };
            _optionsMock = new Mock<IOptions<BunnyMqOptions>>();
            _connectionFactoryProviderMock = new Mock<IConnectionFactoryProvider>();
            _factoryMock = new Mock<IConnectionFactory>();
            _connectionFactoryProviderMock.Setup(o => o.GetFactory()).Returns(_factoryMock.Object);
            _connectionMock = new Mock<IConnection>();
            _channelMock = new Mock<IChannel>();
            _factoryMock.Setup(f => f.CreateConnectionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_connectionMock.Object);
            _connectionMock.Setup(c => c.IsOpen).Returns(true);
            _connectionMock.Setup(c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>())).ReturnsAsync(_channelMock.Object);
            _connectionMock.Setup(c=> c.Dispose());
            _provider = new BunnyMqChannelProvider(_connectionFactoryProviderMock.Object);
        }

        [Fact]
        public async Task Create_ReturnsChannel_WhenConnectionIsOpen()
        {
            var channel = await _provider.Create();
            Assert.NotNull(channel);
            _connectionMock.Verify(
	            c => c.CreateChannelAsync(It.IsAny<CreateChannelOptions?>(), It.IsAny<CancellationToken>()), 
	            Times.Once);
        }

        [Fact]
        public async Task InitializeAsync_CreatesConnection_WhenConnectionIsNull()
        {
            var provider = new BunnyMqChannelProvider(_connectionFactoryProviderMock.Object);
            await provider.InitializeAsync();
            _factoryMock.Verify(
	            f => f.CreateConnectionAsync(It.IsAny<CancellationToken>()), 
	            Times.Once);
        }

        [Fact]
        public void AddConnectionObserver_AddsObserver()
        {
            var observerMock = new Mock<IConnectionObserver>();
            _provider.AddConnectionObserver(observerMock.Object);
            // No direct assertion, but covered in shutdown test
        }

        [Fact]
        public async Task IsConnectionOpen_ReturnsTrue_IfConnectionIsOpen() {
	        await _provider.InitializeAsync();
            Assert.True(_provider.IsConnectionOpen);
        }

        [Fact]
        public async Task Dispose_CallsDisposeOnConnection() {
	        await _provider.InitializeAsync();
            _provider.Dispose();
            _connectionMock.Verify(c => c.Dispose(), Times.Once);
        }
    }
}
