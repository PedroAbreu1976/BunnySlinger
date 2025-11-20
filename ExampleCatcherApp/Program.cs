using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Rabbit.Extensions;
using ExampleBunnies;
using ExampleCatcherApp;
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
	services.AddBunnyHandlers(typeof(TestBunnyCatcher).Assembly);
	services.AddBunnyInterceptors(typeof(TestBunnyCatcher).Assembly);
});

var app = builder.Build();



await app.StartBunnyObserver();

Console.WriteLine("Start catching bunnies:");

await app.RunAsync();
