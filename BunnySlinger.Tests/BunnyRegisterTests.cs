using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Catchers;
using BunnySlinger.Tests.Interceptors;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BunnySlinger.Tests
{
    public class BunnyRegisterTests
    {
        [Fact]
        public void AddBunny_AddsTypeToList()
        {
            var broker = new Mock<IBunnyBroker>();
            var provider = new Mock<IServiceScopeFactory>();
            var register = new BunnyRegister(broker.Object, provider.Object);
            register.AddBunny<TestBunny>();
            register.AddBunny(typeof(TestBunny));
            // Use reflection to check _bunnyTypes
            var types = (List<Type>)typeof(BunnyRegister).GetField("_bunnyTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(register)!;
            Assert.Contains(typeof(TestBunny), types);
        }

        [Fact]
        public void AddBunnyCatcher_AddsToDictionary()
        {
            var broker = new Mock<IBunnyBroker>();
            var provider = new Mock<IServiceScopeFactory>();
            var register = new BunnyRegister(broker.Object, provider.Object);
            register.AddBunnyCatcher<TestBunnyCatcher, TestBunny>();
            var dict = (Dictionary<Type, Type>)typeof(BunnyRegister).GetField("_handlerBunnyTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(register)!;
            Assert.Equal(typeof(TestBunny), dict[typeof(TestBunnyCatcher)]);
        }

        [Fact]
        public async Task RegisterAsync_RegistersBunniesAndCatchers()
        {
            var broker = new Mock<IBunnyBroker>();
            var provider = new Mock<IServiceScopeFactory>();
            var register = new BunnyRegister(broker.Object, provider.Object);
            register.AddBunny<TestBunny>();
            register.AddBunnyCatcher<TestBunnyCatcher, TestBunny>();
            broker.Setup(b => b.RegisterBunnyAsync(typeof(TestBunny))).Returns(Task.CompletedTask).Verifiable();
            broker.Setup(b => b.RegisterBunnyCatcher(
                typeof(TestBunnyCatcher),
                typeof(TestBunny),
                It.IsAny<Func<IBunny, Task<bool>>>())).Returns(Task.CompletedTask).Verifiable();
            await register.RegisterAsync();
            broker.VerifyAll();
            var isConnected = (bool)typeof(BunnyRegister).GetField("_isConnected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(register)!;
            Assert.True(isConnected);
        }

        [Fact]
        public void Dispose_DisposesSemaphore()
        {
            var broker = new Mock<IBunnyBroker>();
            var provider = new Mock<IServiceScopeFactory>();
            var register = new BunnyRegister(broker.Object, provider.Object);
            register.Dispose();
            var semaphore = (SemaphoreSlim)typeof(BunnyRegister).GetField("_semaphore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(register)!;
            Assert.Throws<ObjectDisposedException>(() => semaphore.Wait());
        }

        [Fact]
        public async Task BrokerEvents_ChangeConnectionStateAndRegister()
        {
            var broker = new Mock<IBunnyBroker>();
            var provider = new Mock<IServiceScopeFactory>();
            var register = new BunnyRegister(broker.Object, provider.Object);
            var isConnectedField = typeof(BunnyRegister).GetField("_isConnected", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            isConnectedField.SetValue(register, false);
            var registerAsyncCalled = false;
            var registerAsync = typeof(BunnyRegister).GetMethod("RegisterAsync")!;
            broker.Raise(b => b.ConnectionEstablishedAsync += null, register, EventArgs.Empty);
            // _isConnected should be true after event
            await Task.Delay(10); // allow async event to run
            Assert.True((bool)isConnectedField.GetValue(register)!);
            broker.Raise(b => b.ConnectionBrokenAsync += null, register, EventArgs.Empty);
            Assert.False((bool)isConnectedField.GetValue(register)!);
        }

        [Fact]
        public async Task DispachtBunnyAsync_ResolvesHandlerAndInterceptor()
        {
            var broker = new Mock<IBunnyBroker>();
            var scopeMock = new Mock<IServiceScope>();
            var providerMock = new Mock<IServiceProvider>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var catcherMock = new Mock<IBunnyCatcher>();
            var interceptors = new BunnyInterceptors(new[] { new DummyInterceptor() });
            serviceScopeFactoryMock.Setup(p => p.CreateScope()).Returns(scopeMock.Object);
            scopeMock.Setup(s => s.ServiceProvider).Returns(providerMock.Object);
            providerMock.Setup(p => p.GetService(typeof(TestBunnyCatcher))).Returns(catcherMock.Object);
            providerMock.Setup(p => p.GetService(typeof(BunnyInterceptors))).Returns(interceptors);
            catcherMock.Setup(c => c.CatchBunnyAsync(It.IsAny<IBunny>())).ReturnsAsync(true);
            var register = new BunnyRegister(broker.Object, serviceScopeFactoryMock.Object);
            var method = typeof(BunnyRegister).GetMethod("DispachtBunnyAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            var result = await (Task<bool>)method.Invoke(register, new object[] { typeof(TestBunnyCatcher), new TestBunny() });
            Assert.True(result);
        }
    }
}
