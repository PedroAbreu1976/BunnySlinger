using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Outbox;
using BunnySlinger.Outbox.Extensions;
using BunnySlinger.Outbox.Options;
using BunnySlinger.Rabbit.Extensions;
using ExampleBunnies;
using ExampleOutbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder();


builder.ConfigureServices((hostContext, services) => {
    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseInMemoryDatabase("MyDatabase");
    });

    services.AddBunnyMq(c => {
	    c.HostName = "localhost";
	    c.Port = 5672;
    });

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

    var sender = app.Services.GetRequiredService<IBunnyOutbox>();
    Console.WriteLine("----------------------------");
    Console.WriteLine("RabbitMQ With Outbox Publisher Example:");
    Console.WriteLine("Start queueing bunnies:");
    Console.WriteLine("----------------------------");

    var msg = Console.ReadLine();
    while (!string.IsNullOrWhiteSpace(msg))
    {
        await sender.QueueBunnyAsync(new TestBunny { Message = msg });
        await db.SaveChangesAsync();
        msg = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(msg))
        {
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

