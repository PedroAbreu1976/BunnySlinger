//https://medium.com/@nikitinsn6/a-beginners-guide-to-rabbitmq-and-how-to-use-it-in-net-446662e53ea2
//https://www.rabbitmq.com/tutorials/tutorial-three-dotnet

using BunnySlinger;
using BunnySlinger.Extensions;
using BunnySlinger.Options;
using BunnySlinger.Rabbit;
using ConsoleApp1;
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

	services.AddBunnies(typeof(MyCuteBunny).Assembly);
	services.AddBunnyHandlers(typeof(MyCuteBunny).Assembly);
	services.AddBunnyInterceptors(typeof(MyCuteBunny).Assembly);
});

var app = builder.Build();



await app.StartBunnyObserver();
var sender = app.Services.GetRequiredService<IBunnySling>();
var msg = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(msg))
{
	await sender.SlingBunnyAsync(new MyCuteBunny { Name = msg });
	msg = Console.ReadLine();
}

//var a = new BunnyInMemoryTestScope();
//var log1 = a.AddBunnyCatcher(new MyBunnyCatcher());
//var log2 = a.AddBunnyCatcher(new MyOtherBunnyCatcher());
//await a.StartCatching();

//var id = a.SlingBunnyAsync(
//	new MyCuteBunny {
//		Name = "Bugs Bunny"
//	});

//Console.WriteLine(log1.Count);
//Console.WriteLine(log2.Count);

await app.RunAsync();


