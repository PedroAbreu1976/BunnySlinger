using BunnySlinger.Extensions;
using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Catchers;
using BunnySlinger.Tests.Interceptors;

namespace BunnySlinger.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void GetMessageHandlerTypes_Assembly_ReturnsCorrectMapping()
        {
            var assembly = typeof(TestBunnyCatcher).Assembly;
            var result = assembly.GetMessageHandlerTypes();
            Assert.Contains(typeof(TestBunnyCatcher), result.Keys);
            Assert.Equal(typeof(TestBunny), result[typeof(TestBunnyCatcher)]);
        }

        [Fact]
        public void GetMessageHandlerTypes_Assemblies_ReturnsCorrectMapping()
        {
            var assemblies = new[] { typeof(TestBunnyCatcher).Assembly };
            var result = assemblies.GetMessageHandlerTypes();
            Assert.Contains(typeof(TestBunnyCatcher), result.Keys);
            Assert.Equal(typeof(TestBunny), result[typeof(TestBunnyCatcher)]);
        }

        [Fact]
        public void GetMessageTypes_Assembly_ReturnsCorrectTypes()
        {
            var assembly = typeof(TestBunny).Assembly;
            var result = assembly.GetMessageTypes();
            Assert.Contains(typeof(TestBunny), result);
            Assert.Contains(typeof(NonTestBunny), result);
            Assert.All(result, t => Assert.True(typeof(IBunny).IsAssignableFrom(t)));
        }

        [Fact]
        public void GetMessageTypes_Assemblies_ReturnsCorrectTypes()
        {
            var assemblies = new[] { typeof(TestBunny).Assembly };
            var result = assemblies.GetMessageTypes();
            Assert.Contains(typeof(TestBunny), result);
            Assert.Contains(typeof(NonTestBunny), result);
            Assert.All(result, t => Assert.True(typeof(IBunny).IsAssignableFrom(t)));
        }

        [Fact]
        public void GetInterceptorTypes_Assembly_ReturnsCorrectTypes()
        {
            var assembly = typeof(DummyInterceptor).Assembly;
            var result = assembly.GetInterceptorTypes();
            Assert.Contains(typeof(DummyInterceptor), result);
            Assert.Contains(typeof(DummyTypedInterceptor), result);
            Assert.All(result, t => Assert.True(typeof(IBunnyInterceptor).IsAssignableFrom(t)));
        }

        [Fact]
        public void GetInterceptorTypes_Assemblies_ReturnsCorrectTypes()
        {
            var assemblies = new[] { typeof(DummyInterceptor).Assembly };
            var result = assemblies.GetInterceptorTypes();
            Assert.Contains(typeof(DummyInterceptor), result);
            Assert.Contains(typeof(DummyTypedInterceptor), result);
            Assert.All(result, t => Assert.True(typeof(IBunnyInterceptor).IsAssignableFrom(t)));
        }
    }
}
