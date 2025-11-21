using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Interceptors;

namespace BunnySlinger.Tests
{
    public class BunnyInterceptorsTests
    {
        [Fact]
        public async Task OnBunnyCatch_ExecutesCatcher_WhenNoInterceptors()
        {
            var interceptors = new List<IBunnyInterceptor>();
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new TestBunny { Message = "Hello" };
            bool catcherCalled = false;
            async Task<bool> catcher(IBunny b) { catcherCalled = true; return true; }
            var result = await bunnyInterceptors.OnBunnyCatch(bunny, catcher, typeof(object));
            Assert.True(result);
            Assert.True(catcherCalled);
        }

        [Fact]
        public async Task OnBunnyCatch_ExecutesGlobalInterceptor()
        {
            var dummy = new DummyInterceptor();
            var interceptors = new List<IBunnyInterceptor> { dummy };
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new TestBunny { Message = "Hello" };
            async Task<bool> catcher(IBunny b) { return true; }
            var result = await bunnyInterceptors.OnBunnyCatch(bunny, catcher, typeof(object));
            Assert.True(result);
            Assert.Contains("global", dummy.Calls);
        }

        [Fact]
        public async Task OnBunnyCatch_ExecutesTypedInterceptor_ForMatchingType()
        {
            var dummy = new DummyTypedInterceptor();
            var interceptors = new List<IBunnyInterceptor> { dummy };
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new TestBunny { Message = "Hello" };
            async Task<bool> catcher(IBunny b) { return true; }
            var result = await bunnyInterceptors.OnBunnyCatch(bunny, catcher, typeof(object));
            Assert.True(result);
            Assert.Contains("typed", dummy.Calls);
        }

        [Fact]
        public async Task OnBunnyCatch_DoesNotExecuteTypedInterceptor_ForNonMatchingType()
        {
            var dummy = new DummyTypedInterceptor();
            var interceptors = new List<IBunnyInterceptor> { dummy };
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new NonTestBunny();
            async Task<bool> catcher(IBunny b) { return true; }
            var result = await bunnyInterceptors.OnBunnyCatch(bunny, catcher, typeof(object));
            Assert.True(result);
            Assert.DoesNotContain("typed", dummy.Calls);
        }

        [Fact]
        public void GetInterceptors_ReturnsGlobalAndMatchingTyped()
        {
            var global = new DummyInterceptor();
            var typed = new DummyTypedInterceptor();
            var interceptors = new List<IBunnyInterceptor> { global, typed };
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new TestBunny { Message = "Hello" };
            var result = bunnyInterceptors.GetType()
                .GetMethod("GetInterceptors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(bunnyInterceptors, new object[] { bunny }) as List<IBunnyInterceptor>;
            Assert.Contains(global, result);
            Assert.Contains(typed, result);
        }

        [Fact]
        public void GetInterceptors_ExcludesTypedInterceptor_ForNonMatchingType()
        {
            var global = new DummyInterceptor();
            var typed = new DummyTypedInterceptor();
            var interceptors = new List<IBunnyInterceptor> { global, typed };
            var bunnyInterceptors = new BunnyInterceptors(interceptors);
            var bunny = new NonTestBunny();
            var result = bunnyInterceptors.GetType()
                .GetMethod("GetInterceptors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Invoke(bunnyInterceptors, new object[] { bunny }) as List<IBunnyInterceptor>;
            Assert.Contains(global, result);
            Assert.DoesNotContain(typed, result);
        }
    }
}
