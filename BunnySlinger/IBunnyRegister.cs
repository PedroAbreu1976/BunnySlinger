using BunnySlinger.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace BunnySlinger;

public interface IBunnyRegister : IDisposable
{
    void AddBunny<TBunny>() where TBunny : IBunny;
    void AddBunny(Type type);
    void AddBunnyCatcher<THandler, TBunny>() where THandler : IBunnyCatcher<TBunny> where TBunny : IBunny;
    void AddBunnyCatcher(Type handlerType, Type bunnyType);
    Task RegisterAsync();
}

public class BunnyRegister : IBunnyRegister
{
    private readonly IBunnyBroker _broker;
    private readonly IServiceProvider _serviceProvider;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private readonly List<Type> _bunnyTypes = [];
    private readonly Dictionary<Type, Type> _handlerBunnyTypes = [];
    private bool _isConnected = false;

    public BunnyRegister(IBunnyBroker broker, IServiceProvider serviceProvider)
    {
        _broker = broker;
        _serviceProvider = serviceProvider;

        _broker.ConnectionBrokenAsync += Broker_OnConnectionBrokenAsync;
        _broker.ConnectionEstablishedAsync += Broker_ConnectionEstablishedAsync;
    }

    private async Task Broker_ConnectionEstablishedAsync(object sender, EventArgs args)
    {
        if (!_isConnected)
        {
            await RegisterAsync();
        }
    }

    private Task Broker_OnConnectionBrokenAsync(object sender, EventArgs args)
    {
        _isConnected = false;
        return Task.CompletedTask;
    }

    public void AddBunny<TBunny>() where TBunny : IBunny
    {
        AddBunny(typeof(TBunny));
    }

    public void AddBunny(Type type)
    {
        _bunnyTypes.Add(type);
    }

    public void AddBunnyCatcher<THandler, TBunny>() where THandler : IBunnyCatcher<TBunny> where TBunny : IBunny
    {
        AddBunnyCatcher(typeof(THandler), typeof(TBunny));
    }

    public void AddBunnyCatcher(Type handlerType, Type bunnyType)
    {
        _handlerBunnyTypes.Add(handlerType, bunnyType);
    }


    public async Task RegisterAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await _bunnyTypes.ForEachAsync(async x => await _broker.RegisterBunnyAsync(x));
            foreach (var handlerBunnyType in _handlerBunnyTypes)
            {
                await _broker.RegisterBunnyCatcher(
                    handlerBunnyType.Key,
                    handlerBunnyType.Value,
                    async bunny => await DispachtBunnyAsync(handlerBunnyType.Key, bunny));
            }
            _isConnected = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task<bool> DispachtBunnyAsync(Type handlerType, IBunny bunny)
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService(handlerType) as IBunnyCatcher;
        var interceptors = scope.ServiceProvider.GetRequiredService<BunnyInterceptors>();
        return await interceptors.OnBunnyCatch(bunny, handler!.CatchBunnyAsync, handlerType);
    }

    public void Dispose()
    {
        _semaphore.Dispose();

    }
}
