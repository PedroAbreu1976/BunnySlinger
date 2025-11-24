using BunnySlinger.InMemory;
using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Catchers;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BunnySlinger.Tests
{
    public class BunnyInMemoryRegisterTests
    {
        [Fact]
        public void AddBunny_Generic_DoesNotThrow()
        {
            var queue = new BunnyInMemoryQueue();
            var register = new BunnyInMemoryRegister(Mock.Of<IServiceProvider>(), queue);
            register.AddBunny<TestBunny>();
        }

        [Fact]
        public void AddBunny_Type_DoesNotThrow()
        {
            var queue = new BunnyInMemoryQueue();
            var register = new BunnyInMemoryRegister(Mock.Of<IServiceProvider>(), queue);
            register.AddBunny(typeof(TestBunny));
        }

        [Fact]
        public async Task AddBunnyCatcher_GenericTypes_RegistersHandlerAndHandlesBunny()
        {
            var queue = new BunnyInMemoryQueue();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<TestBunnyCatcher, TestBunnyCatcher>()
                .AddSingleton<BunnyInterceptors>(_ => new BunnyInterceptors(Array.Empty<IBunnyInterceptor>()))
                .AddLogging()
                .BuildServiceProvider();
            var register = new BunnyInMemoryRegister(serviceProvider, queue);
            register.AddBunnyCatcher<TestBunnyCatcher, TestBunny>();
            await register.RegisterAsync();
            var args = new BunnyDispatchedAsyncEventHandlerArgs(new TestBunny { Message = "test" });
            await queue.OnBunnyDispatchedAsync(args.Bunny);
        }

        [Fact]
        public async Task AddBunnyCatcher_Type_RegistersHandlerAndHandlesBunny()
        {
            var queue = new BunnyInMemoryQueue();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<TestBunnyCatcher, TestBunnyCatcher>()
                .AddSingleton<BunnyInterceptors>(_ => new BunnyInterceptors(Array.Empty<IBunnyInterceptor>()))
                .AddLogging()
                .BuildServiceProvider();
            var register = new BunnyInMemoryRegister(serviceProvider, queue);
            register.AddBunnyCatcher(typeof(TestBunnyCatcher), typeof(TestBunny));
            await register.RegisterAsync();
            var args = new BunnyDispatchedAsyncEventHandlerArgs(new TestBunny { Message = "test" });
            await queue.OnBunnyDispatchedAsync(args.Bunny);
        }

        [Fact]
        public async Task AddBunnyCatcher_Instance_HandlesBunny()
        {
            var queue = new BunnyInMemoryQueue();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<BunnyInterceptors>(_ => new BunnyInterceptors(Array.Empty<IBunnyInterceptor>()))
                .BuildServiceProvider();
            var catcher = new TestBunnyCatcher(Mock.Of<Microsoft.Extensions.Logging.ILogger<TestBunnyCatcher>>());
            var register = new BunnyInMemoryRegister(serviceProvider, queue);
            register.AddBunnyCatcher<TestBunny>(catcher);
            await register.RegisterAsync();
            var args = new BunnyDispatchedAsyncEventHandlerArgs(new TestBunny { Message = "test" });
            await queue.OnBunnyDispatchedAsync(args.Bunny);
        }

        [Fact]
        public async Task RegisterAsync_InvokesAllHandlers()
        {
            var queue = new BunnyInMemoryQueue();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<BunnyInterceptors>(_ => new BunnyInterceptors(Array.Empty<IBunnyInterceptor>()))
                .BuildServiceProvider();
            var catcher = new TestBunnyCatcher(Mock.Of<Microsoft.Extensions.Logging.ILogger<TestBunnyCatcher>>());
            var register = new BunnyInMemoryRegister(serviceProvider, queue);
            register.AddBunnyCatcher<TestBunny>(catcher);
            await register.RegisterAsync();
        }

        [Fact]
        public async Task OnConnectionBrokenAsync_ReturnsCompletedTask()
        {
            var queue = new BunnyInMemoryQueue();
            var register = new BunnyInMemoryRegister(Mock.Of<IServiceProvider>(), queue);
            var task = register.OnConnectionBrokenAsync();
            await task;
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public async Task OnConnectionEstablishedAsync_ReturnsCompletedTask()
        {
            var queue = new BunnyInMemoryQueue();
            var register = new BunnyInMemoryRegister(Mock.Of<IServiceProvider>(), queue);
            var task = register.OnConnectionEstablishedAsync();
            await task;
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public void Dispose_DoesNotThrow()
        {
            var queue = new BunnyInMemoryQueue();
            var register = new BunnyInMemoryRegister(Mock.Of<IServiceProvider>(), queue);
            register.Dispose();
        }
    }
}
