using BunnySlinger.Extensions;
using BunnySlinger.Idempotency;
using BunnySlinger.Idempotency.Tests.Fakes;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger.Idempotency.Tests
{
    public class DependencyInjectionExtensionsTests
    {

        [Fact]
        public void AddBunnyIdempotency_RegistersIdempotentInterceptorAsScoped()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddLogging();
            services.AddDbContext<TestDbContext>(o => { o.UseInMemoryDatabase("TestDatabase"); });
            services.AddBunnies(this.GetType().Assembly);
            services.AddBunnyHandlers(this.GetType().Assembly);
            services.AddBunnyIdempotency<TestDbContext>();
            var provider = services.BuildServiceProvider();

            // Assert
            var interceptor = provider.GetService<IBunnyInterceptor>();
            Assert.NotNull(interceptor);
            Assert.IsType<IdempotentInterceptor<TestDbContext>>(interceptor);
        }
    }
}
