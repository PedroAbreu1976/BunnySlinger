namespace BunnySlinger
{
    public interface IBunnyRegister : IDisposable, IConnectionObserver {
	    void AddBunny<T>() where T : IBunny;
	    void AddBunny(Type type);

	    void AddBunnyCatcher<THandler, TMessage>() where THandler : IBunnyCatcher<TMessage>
		    where TMessage : IBunny;

	    void AddBunnyCatcher(Type handlerType, Type msgType);

	    void AddBunnyCatcher<T>(IBunnyCatcher<T> handler) where T : IBunny;

	    Task RegisterAsync();
    }
}
