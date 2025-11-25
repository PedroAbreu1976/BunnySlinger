using BunnySlinger;
using BunnySlinger.Idempotency;
using BunnySlinger.Idempotency.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BunnySlinger.Idempotency.Tests
{
    public class IdempotentInterceptorTests
    {
        private TestDbContext CreateDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(dbName)
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new TestDbContext(options);
        }

        private BunnyHandlerTypes CreateHandlerTypes() {
            return new BunnyHandlerTypes([this.GetType().Assembly]);
        }

        [Fact]
        public async Task OnBunnyCatch_ProcessesIdempotentBunnyOnce()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateDbContext(dbName);
            var handlerType = typeof(TestIdempotentBunnyCatcher);
            var handlerTypes = CreateHandlerTypes();
            var logger = Mock.Of<ILogger<IdempotentInterceptor<TestDbContext>>>();
            var interceptor = new IdempotentInterceptor<TestDbContext>(context, handlerTypes, logger);
            var bunny = new TestIdempotentBunny { BunnyID = "id1" };
            int callCount = 0;
            async Task<bool> catcher(IBunny b) { callCount++; return true; }
            var result1 = await interceptor.OnBunnyCatch(bunny, catcher, handlerType);
            var result2 = await interceptor.OnBunnyCatch(bunny, catcher, handlerType);
            Assert.True(result1);
            Assert.False(result2); // Should not process twice
            Assert.Equal(1, callCount);
        }

        [Fact]
        public async Task OnBunnyCatch_LogsErrorOnException()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateDbContext(dbName);
            
            var handlerTypes = CreateHandlerTypes();
            var handlerType = typeof(TestIdempotentBunnyCatcher);
            var loggerMock = new Mock<ILogger<IdempotentInterceptor<TestDbContext>>>();
            var interceptor = new IdempotentInterceptor<TestDbContext>(context, handlerTypes, loggerMock.Object);
            var bunny = new TestIdempotentBunny { BunnyID = "id2" };
            async Task<bool> catcher(IBunny b) { throw new Exception("fail"); }
            var result = await interceptor.OnBunnyCatch(bunny, catcher, handlerType);
            loggerMock.Verify(l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("fail")),
                It.Is<Exception>(ex => ex.Message == "fail"),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ), Times.Once);
            Assert.False(result);
        }
    }
}
