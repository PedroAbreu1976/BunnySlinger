using BunnySlinger.InMemory;

namespace BunnySlinger;

public interface IBunnyBroker : IBunnySling
{
	event AsyncEventHandler ConnectionEstablishedAsync;
	event AsyncEventHandler ConnectionBrokenAsync;

    Task RegisterBunnyAsync(Type type);
	Task RegisterBunnyCatcher(Type typehandlerType, Type msgType, Func<IBunny, Task<bool>> dispachtBunnyAsync);
}


