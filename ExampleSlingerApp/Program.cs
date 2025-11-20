using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Rabbit.Extensions;
using ExampleBunnies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder();


builder.ConfigureServices((hostContext, services) => {
	services.AddBunnyMq(
		new BunnyMqOptions
		{
			HostName = "localhost",
			Port = 5672
		});

	//services.AddBunnyInMemory();

	services.AddBunnies(typeof(TestBunny).Assembly);
});

var app = builder.Build();

var sender = app.Services.GetRequiredService<IBunnySling>();
Console.WriteLine("Start throwing bunnies:");
var msg = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(msg))
{
	await sender.SlingBunnyAsync(new TestBunny { Message = msg });
	msg = Console.ReadLine();
}

await app.RunAsync();
