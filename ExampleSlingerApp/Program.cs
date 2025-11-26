using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Rabbit.Extensions;
using ExampleBunnies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder();


builder.ConfigureServices((hostContext, services) => {
    services.AddBunnyMq(o => {
			o.HostName = "localhost";
			o.Port = 5672;
    });
	services.AddBunnies(typeof(TestBunny).Assembly);
});

var app = builder.Build();


var sender = app.Services.GetRequiredService<IBunnySling>();
Console.WriteLine("----------------------------");
Console.WriteLine("RabbitMQ Publisher Example:");
Console.WriteLine("Start throwing bunnies:");
Console.WriteLine("----------------------------");

var msg = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(msg))
{
	await sender.SlingBunnyAsync(new TestBunny { Message = msg });
	msg = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(msg))
	{
		break;
	}
}


await app.RunAsync();
