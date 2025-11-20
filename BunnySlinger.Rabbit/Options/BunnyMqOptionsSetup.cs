using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Options;

public class BunnyMqOptionsSetup(IConfiguration configuration) : IConfigureOptions<BunnyMqOptions>
{
	private const string SectionName = "BunnyMq";

	public void Configure(BunnyMqOptions options)
	{
		configuration
			.GetSection(SectionName)
			.Bind(options);
	}
}
