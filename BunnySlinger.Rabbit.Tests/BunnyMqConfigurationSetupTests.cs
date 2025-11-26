using BunnySlinger.Options;
using Microsoft.Extensions.Configuration;

namespace BunnySlinger.Rabbit.Tests
{
    public class BunnyMqConfigurationSetupTests
    {
        [Fact]
        public void Configure_BindsConfigurationSectionToOptions()
        {
            // Arrange: Setup configuration with BunnyMq section
            var configData = new Dictionary<string, string>
            {
                {"BunnyMq:HostName", "localhost"},
                {"BunnyMq:Port", "5672"},
                {"BunnyMq:UserName", "guest"},
                {"BunnyMq:Password", "guest"},
                {"BunnyMq:VirtualHost", "/"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
            var setup = new BunnyMqConfigurationSetup(configuration);
            var options = new BunnyMqConfiguration();

            // Act
            setup.Configure(options);

            // Assert
            Assert.Equal("localhost", options.HostName);
            Assert.Equal(5672, options.Port);
            Assert.Equal("guest", options.UserName);
            Assert.Equal("guest", options.Password);
            Assert.Equal("/", options.VirtualHost);
        }
    }
}
