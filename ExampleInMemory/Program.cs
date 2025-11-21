using BunnySlinger;
using BunnySlinger.Extensions;

using ExampleBunnies;

using ExampleInMemory;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateDefaultBuilder();


builder.ConfigureServices((hostContext, services) => {
	services.AddBunnyInMemory();

	services.AddBunnies(typeof(TestBunny).Assembly);
	services.AddBunnyHandlers(typeof(TestBunnyCatcher).Assembly);
	services.AddBunnyInterceptors(typeof(RandomBunnyInterceptor).Assembly);
});

var app = builder.Build();

await app.StartBunnyObserver();



var sender = app.Services.GetRequiredService<IBunnySling>();
Console.WriteLine("----------------------------");
Console.WriteLine("In-Memory Example:");
Console.WriteLine("Start throwing and catching bunnies:");
Console.WriteLine("----------------------------");
var msg = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(msg))
{
	await sender.SlingBunnyAsync(new TestBunny { Message = msg });
	msg = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(msg)) {
		break;
	}
}

await app.RunAsync();
