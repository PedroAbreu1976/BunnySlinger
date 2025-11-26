using BunnySlinger.Extensions;
using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using BunnySlinger.Outbox.Tests.Fakes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger.Outbox.Tests.Helpers
{

	internal static class ServiceHelpers {
		public static IServiceProvider CreateServices(Action<BunnyOutboxConfiguration>? configuration = null) {
			var services = new ServiceCollection();

			var config = new ConfigurationBuilder().Build();
			services.AddSingleton<IConfiguration>(config);
			services.AddBunnyOutbox<TestDbContext>(configuration);

			services.AddDbContext<TestDbContext>(o => { o.UseInMemoryDatabase("TestDatabase"); });
			services.AddBunnyInMemory();
			services.AddBunnies(typeof(ServiceHelpers).Assembly);

			return services.BuildServiceProvider();
		}
	}

}
