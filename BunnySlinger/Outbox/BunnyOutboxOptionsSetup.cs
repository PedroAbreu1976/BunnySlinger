using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox;

public class BunnyOutboxOptionsSetup(IConfiguration configuration) : IConfigureOptions<BunnyOutboxOptions>
{
	private const string SectionName = "BunnyOutbox";

	public void Configure(BunnyOutboxOptions options)
	{
		configuration
			.GetSection(SectionName)
			.Bind(options);
	}
}
