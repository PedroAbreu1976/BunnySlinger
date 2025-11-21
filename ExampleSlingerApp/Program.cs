using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using BunnySlinger.Rabbit.Extensions;
using ExampleBunnies;
using ExampleSlingerApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder();


builder.ConfigureServices((hostContext, services) => {
	services.AddDbContext<AppDbContext>(options =>
	{
		// "MyDatabaseName" is the unique name for the in-memory instance.
		// If you use the same name across contexts, they share the data.
		options.UseInMemoryDatabase("MyDatabase");
	});

    services.AddBunnyMq(
		new BunnyMqOptions
		{
			HostName = "localhost",
			Port = 5672
		});

	//services.AddBunnyInMemory();

	services.AddBunnies(typeof(TestBunny).Assembly);
	services.AddBunnyOutbox<AppDbContext>(new BunnyOutboxOptions());
});

var app = builder.Build();

await app.StartBunnyOutbox();

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	// Ensure database is created
	await db.Database.EnsureCreatedAsync();
	Console.WriteLine("In-Memory Database loaded successfully.");

	var sender = app.Services.GetRequiredService<IBunnyOutbox>();
	Console.WriteLine("Start throwing bunnies:");
	var msg = Console.ReadLine();
	while (!string.IsNullOrWhiteSpace(msg))
	{
		await sender.QueueBunnyAsync(new TestBunny { Message = msg });
		await db.SaveChangesAsync();
		msg = Console.ReadLine();
		if (msg == "x") {
			break;
		}
	}

	var outbox = await db.GetBunnyOutbox().CountAsync();
	var failed = await db.GetBunnyFailed().CountAsync();
	var processed = await db.GetBunnyProcessed().CountAsync();
	
	Console.WriteLine($"Outbox: {outbox}");
	Console.WriteLine($"Failed: {failed}");
	Console.WriteLine($"Processed: {processed}");

	Console.ReadKey();
}



await app.RunAsync();
