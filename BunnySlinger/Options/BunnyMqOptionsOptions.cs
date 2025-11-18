using Microsoft.Extensions.Options;


namespace BunnySlinger.Options
{
    internal class BunnyMqOptionsOptions : IOptions<BunnyMqOptions> {

	    public BunnyMqOptionsOptions(BunnyMqOptions options) {
            Value = options;
        }

        /// <summary>
        /// Gets the configured <see cref="BunnyMqOptions"/> instance.
        /// </summary>
        /// <value>
        /// The <see cref="BunnyMqOptions"/> instance containing the RabbitMQ configuration options.
        /// </value>
        public BunnyMqOptions Value { get; }
    }
}
