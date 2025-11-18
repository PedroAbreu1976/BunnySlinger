namespace BunnySlinger
{
    public interface IConnectionObserver {
	    Task OnConnectionBokenAsync();
		Task OnConnectionEstablishedAsync();
    }
}
