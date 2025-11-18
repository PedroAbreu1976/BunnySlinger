namespace BunnySlinger
{
    public interface IBunnySling {
	    Task SlingBunnyAsync<TBunny>(TBunny bunny, CancellationToken ct = default) where TBunny : IBunny;
    }
}
