using BunnySlinger.Extensions;
using BunnySlinger.InMemory;
using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Catchers;
using BunnySlinger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Reflection;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace BunnySlinger.Tests
{
    public class DependencyInjectionExtensionsTests
    {
        [Fact]
        public void AddBunnyInMemory_RegistersExpectedServices()
        {
            var services = new ServiceCollection();
            services.AddBunnyInMemory();
            var provider = services.BuildServiceProvider();

            Assert.NotNull(provider.GetService<BunnyInMemoryQueue>());
            Assert.NotNull(provider.GetServices<IHostedService>().OfType<ChannelPublisherWorker>().FirstOrDefault());
            Assert.NotNull(provider.GetService<IBunnySling>());
            Assert.NotNull(provider.GetService<BunnyInterceptors>());
            Assert.NotNull(provider.GetService<IBunnyRegister>());
        }

        [Fact]
        public void AddBunnies_RegistersBunnyMessageTypes()
        {
            var services = new ServiceCollection();
            var assembly = typeof(TestBunny).Assembly;
            services.AddBunnies(assembly);
            var provider = services.BuildServiceProvider();
            var types = provider.GetService<BunnyMessageTypes>();
            Assert.NotNull(types);
            Assert.Contains(typeof(TestBunny), types.MessageTypes);
        }

        [Fact]
        public void AddBunnyHandlers_RegistersHandlerTypesAndScopedHandlers()
        {
            var services = new ServiceCollection();
            services.AddLogging(); // Register logging for ILogger<TestBunnyCatcher>
            var assembly = typeof(TestBunnyCatcher).Assembly;
            services.AddBunnyHandlers(assembly);
            var provider = services.BuildServiceProvider();
            var handlerTypes = provider.GetService<BunnyHandlerTypes>();
            Assert.NotNull(handlerTypes);
            Assert.Contains(typeof(TestBunnyCatcher), handlerTypes.HandlerTypes.Keys);
            Assert.Equal(typeof(TestBunny), handlerTypes.HandlerTypes[typeof(TestBunnyCatcher)]);
            // Handler should be registered as scoped
            using (var scope = provider.CreateScope())
            {
                Assert.NotNull(scope.ServiceProvider.GetService(typeof(TestBunnyCatcher)));
            }
        }

        [Fact]
        public void AddBunnyInterceptors_RegistersInterceptorTypes()
        {
            var services = new ServiceCollection();
            var assembly = typeof(BunnySlinger.Tests.Interceptors.DummyInterceptor).Assembly;
            services.AddBunnyInterceptors(assembly);
            var provider = services.BuildServiceProvider();
            var interceptors = provider.GetServices<IBunnyInterceptor>();
            Assert.True(interceptors.Any());
        }

        [Fact]
        public async Task StartBunnyObserver_RegistersBunniesAndHandlersAndStartsWorker()
        {
            var registerMock = new Mock<IBunnyRegister>();
            var messageTypes = new BunnyMessageTypes(new[] { typeof(TestBunny).Assembly });
            var handlerTypes = new BunnyHandlerTypes(new[] { typeof(TestBunnyCatcher).Assembly });
            var workerMock = new Mock<ChannelPublisherWorker>(MockBehavior.Loose, new BunnyInMemoryQueue());
            var hostMock = new Mock<IHost>();
            var services = new ServiceCollection();
            services.AddSingleton(registerMock.Object);
            services.AddSingleton(messageTypes);
            services.AddSingleton(handlerTypes);
            services.AddSingleton<IHostedService>(workerMock.Object);
            var provider = services.BuildServiceProvider();
            hostMock.Setup(h => h.Services).Returns(provider);

            registerMock.Setup(r => r.AddBunny(It.IsAny<Type>())).Verifiable();
            registerMock.Setup(r => r.AddBunnyCatcher(It.IsAny<Type>(), It.IsAny<Type>())).Verifiable();
            registerMock.Setup(r => r.RegisterAsync()).Returns(Task.CompletedTask).Verifiable();
            workerMock.Setup(w => w.StartAsync(It.IsAny<CancellationToken>())).Verifiable();

            var result = await DependencyInjectionExtensions.StartBunnyObserver(hostMock.Object);

            registerMock.Verify(r => r.AddBunny(It.IsAny<Type>()), Times.AtLeastOnce());
            registerMock.Verify(r => r.AddBunnyCatcher(It.IsAny<Type>(), It.IsAny<Type>()), Times.AtLeastOnce());
            registerMock.Verify(r => r.RegisterAsync(), Times.Once());
            workerMock.Verify(w => w.StartAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(hostMock.Object, result);
        }
    }
}
