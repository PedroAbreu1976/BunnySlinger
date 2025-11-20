namespace BunnySlinger
{
    public interface IConnectionObserver {
	    Task OnConnectionBrokenAsync();
		Task OnConnectionEstablishedAsync();
    }
}
