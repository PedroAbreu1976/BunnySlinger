using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox.Options;

public class BunnyOutboxConfigurationOptions : IOptions<BunnyOutboxConfiguration>
{

    public BunnyOutboxConfigurationOptions(BunnyOutboxConfiguration options)
    {
        Value = options;
    }

    /// <summary>
    /// Gets the configured <see cref="BunnyOutboxConfiguration"/> instance.
    /// </summary>
    public BunnyOutboxConfiguration Value { get; }
}
