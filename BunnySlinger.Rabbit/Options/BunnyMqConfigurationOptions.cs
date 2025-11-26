using Microsoft.Extensions.Options;


namespace BunnySlinger.Options
{
    internal class BunnyMqConfigurationOptions : IOptions<BunnyMqConfiguration> {

	    public BunnyMqConfigurationOptions(BunnyMqConfiguration options) {
            Value = options;
        }

        /// <summary>
        /// Gets the configured <see cref="BunnyMqConfiguration"/> instance.
        /// </summary>
        /// <value>
        /// The <see cref="BunnyMqConfiguration"/> instance containing the RabbitMQ configuration options.
        /// </value>
        public BunnyMqConfiguration Value { get; }
    }
}
