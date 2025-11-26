using BunnySlinger.Options;
using BunnySlinger.Rabbit.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BunnySlinger.Rabbit.Tests
{
    public class DependencyInjectionExtensionsTests
    {
        [Fact]
        public void AddBunnyMq_WithOptions_RegistersExpectedServices()
        {
            var services = new ServiceCollection();
            // Add minimal configuration for BunnyMqOptionsSetup
            var config = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddBunnyMq(o => {
                o.HostName = "testhost";
                o.Port = 1000;
            });
            var provider = services.BuildServiceProvider();

            // IOptions<BunnyMqOptions> should be registered as singleton
            var opts = provider.GetService<IOptions<BunnyMqConfiguration>>();
            Assert.NotNull(opts);
            Assert.Equal("testhost", opts.Value.HostName);
            Assert.Equal(1000, opts.Value.Port);

            // IChannelProvider should be registered as singleton
            var channelProvider1 = provider.GetService<IChannelProvider>();
            var channelProvider2 = provider.GetService<IChannelProvider>();
            Assert.NotNull(channelProvider1);
            Assert.Same(channelProvider1, channelProvider2);

            // IBunnySling should be registered as singleton
            var sling1 = provider.GetService<IBunnySling>();
            var sling2 = provider.GetService<IBunnySling>();
            Assert.NotNull(sling1);
            Assert.Same(sling1, sling2);

            // BunnyInterceptors should be registered as scoped
            using (var scope1 = provider.CreateScope())
            using (var scope2 = provider.CreateScope())
            {
                var interceptors1 = scope1.ServiceProvider.GetService<BunnyInterceptors>();
                var interceptors2 = scope2.ServiceProvider.GetService<BunnyInterceptors>();
                Assert.NotNull(interceptors1);
                Assert.NotNull(interceptors2);
                Assert.NotSame(interceptors1, interceptors2);
            }

            // IBunnyRegister should be registered as singleton
            var reg1 = provider.GetService<IBunnyRegister>();
            var reg2 = provider.GetService<IBunnyRegister>();
            Assert.NotNull(reg1);
            Assert.Same(reg1, reg2);
        }

        [Fact]
        public void AddBunnyMq_WithoutOptions_RegistersExpectedServices()
        {
            var services = new ServiceCollection();
            // Add minimal configuration for BunnyMqOptionsSetup
            var config = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(config);
            services.AddBunnyMq();
            var provider = services.BuildServiceProvider();

            // IChannelProvider should be registered as singleton
            var channelProvider1 = provider.GetService<IChannelProvider>();
            var channelProvider2 = provider.GetService<IChannelProvider>();
            Assert.NotNull(channelProvider1);
            Assert.Same(channelProvider1, channelProvider2);

            // IBunnySling should be registered as singleton
            var sling1 = provider.GetService<IBunnySling>();
            var sling2 = provider.GetService<IBunnySling>();
            Assert.NotNull(sling1);
            Assert.Same(sling1, sling2);

            // BunnyInterceptors should be registered as scoped
            using (var scope1 = provider.CreateScope())
            using (var scope2 = provider.CreateScope())
            {
                var interceptors1 = scope1.ServiceProvider.GetService<BunnyInterceptors>();
                var interceptors2 = scope2.ServiceProvider.GetService<BunnyInterceptors>();
                Assert.NotNull(interceptors1);
                Assert.NotNull(interceptors2);
                Assert.NotSame(interceptors1, interceptors2);
            }

            // IBunnyRegister should be registered as singleton
            var reg1 = provider.GetService<IBunnyRegister>();
            var reg2 = provider.GetService<IBunnyRegister>();
            Assert.NotNull(reg1);
            Assert.Same(reg1, reg2);
        }
    }
}
