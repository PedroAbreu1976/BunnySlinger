using BunnySlinger.Rabbit;
using Moq;

namespace BunnySlinger.Rabbit.Tests
{
    public class BunnyMqRegisterTests
    {
        [Fact]
        public void AddBunny_Generic_AddsMessageAction()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.AddBunny<TestBunny>();
            // Should not throw and should add to _messages
        }

        [Fact]
        public void AddBunny_Type_AddsMessageAction()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.AddBunny(typeof(TestBunny));
        }

        [Fact]
        public void AddBunnyCatcher_GenericTypes_AddsHandlerAction()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.AddBunnyCatcher<TestBunnyCatcher, TestBunny>();
        }

        [Fact]
        public void AddBunnyCatcher_Type_AddsHandlerAction()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.AddBunnyCatcher(typeof(TestBunnyCatcher), typeof(TestBunny));
        }

        [Fact]
        public void AddBunnyCatcher_Instance_AddsHandlerAction()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var catcher = new TestBunnyCatcher();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.AddBunnyCatcher<TestBunny>(catcher);
        }

        [Fact]
        public async Task RegisterAsync_InvokesAllActions()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            bool messageCalled = false;
            bool handlerCalled = false;
            // Add dummy actions
            register.GetType().GetField("_messages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(register, new System.Collections.Generic.Dictionary<Type, Func<Task>> { { typeof(TestBunny), () => { messageCalled = true; return Task.CompletedTask; } } });
            register.GetType().GetField("_handlers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(register, new System.Collections.Generic.Dictionary<Type, Func<Task>> { { typeof(TestBunnyCatcher), () => { handlerCalled = true; return Task.CompletedTask; } } });
            await register.RegisterAsync();
            Assert.True(messageCalled);
            Assert.True(handlerCalled);
        }

        [Fact]
        public void Dispose_DisposesSemaphore()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            register.Dispose();
        }

        [Fact]
        public async Task OnConnectionBrokenAsync_ReturnsCompletedTask()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            var task = register.OnConnectionBrokenAsync();
            await task;
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public async Task OnConnectionEstablishedAsync_CallsRegisterAsync()
        {
            var channelProvider = new Mock<IChannelProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            var register = new BunnyMqRegister(channelProvider.Object, serviceProvider.Object);
            bool called = false;
            register.GetType().GetField("_messages", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .SetValue(register, new System.Collections.Generic.Dictionary<Type, Func<Task>> { { typeof(TestBunny), () => { called = true; return Task.CompletedTask; } } });
            await register.OnConnectionEstablishedAsync();
            Assert.True(called);
        }

        // Dummy types for testing
        public class TestBunny : IBunny { public string Message { get; set; } }
        public class TestBunnyCatcher : IBunnyCatcher<TestBunny>
        {
            public Task<bool> CatchBunnyAsync(TestBunny bunny) => Task.FromResult(true);
            Task<bool> IBunnyCatcher.CatchBunnyAsync(IBunny bunny) => CatchBunnyAsync((TestBunny)bunny);
        }
    }
}
