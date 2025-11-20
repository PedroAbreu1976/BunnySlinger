using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox.Options;

internal class BunnyOutboxOptionsOptions : IOptions<BunnyOutboxOptions>
{

    public BunnyOutboxOptionsOptions(BunnyOutboxOptions options)
    {
        Value = options;
    }

    /// <summary>
    /// Gets the configured <see cref="BunnyOutboxOptions"/> instance.
    /// </summary>
    public BunnyOutboxOptions Value { get; }
}
