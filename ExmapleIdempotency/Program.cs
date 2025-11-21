using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Idempotency;
using ExampleIdempotency;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


var builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((hostContext, services) => {
	services.AddDbContext<AppDbContext>(options =>
	{
		options.UseInMemoryDatabase("MyDatabase");
		options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
	});
    services.AddBunnyInMemory();
	services.AddBunnies(typeof(IdempotentBunny).Assembly);
	services.AddBunnyHandlers(typeof(IdempontentBunnyCatcher).Assembly);
	services.AddBunnyIdempotency<AppDbContext>();
});

var app = builder.Build();

await app.StartBunnyObserver();

using (var scope = app.Services.CreateScope()) {
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	// Ensure database is created
	await db.Database.EnsureCreatedAsync();

	var sender = app.Services.GetRequiredService<IBunnySling>();
	Console.WriteLine("----------------------------");
	Console.WriteLine("Idempotency Example:");
	Console.WriteLine("For each message one bunny will be sent twice, only once it will be processed");
	Console.WriteLine("Start throwing and catching bunnies:");
	Console.WriteLine("----------------------------");
	var msg = Console.ReadLine();
	while (!string.IsNullOrWhiteSpace(msg)) {
		var bunny = new IdempotentBunny {
			BunnyID = Guid.NewGuid().ToString(),
			Message = msg
		};

        await sender.SlingBunnyAsync(bunny);
        await sender.SlingBunnyAsync(bunny);


        msg = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(msg)) {
			break;
		}
	}
}

await app.RunAsync();
