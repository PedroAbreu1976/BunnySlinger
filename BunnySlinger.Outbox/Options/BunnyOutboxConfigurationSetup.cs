using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Outbox.Options;

public class BunnyOutboxConfigurationSetup(IConfiguration configuration) : IConfigureOptions<BunnyOutboxConfiguration>
{
	private const string SectionName = "BunnyOutbox";

	public void Configure(BunnyOutboxConfiguration options)
	{
		configuration
			.GetSection(SectionName)
			.Bind(options);
	}
}
