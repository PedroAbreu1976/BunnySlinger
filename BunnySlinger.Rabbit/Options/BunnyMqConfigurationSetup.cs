using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Options;

public class BunnyMqConfigurationSetup(IConfiguration configuration) : IConfigureOptions<BunnyMqConfiguration>
{
	private const string SectionName = "BunnyMq";

	public void Configure(BunnyMqConfiguration options)
	{
		configuration
			.GetSection(SectionName)
			.Bind(options);
	}
}
