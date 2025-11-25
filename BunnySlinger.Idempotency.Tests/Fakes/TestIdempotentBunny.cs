namespace BunnySlinger.Idempotency.Tests.Fakes
{
	public class TestIdempotentBunny : IIdempotentBunny
	{
		public string BunnyID { get; set; }
	}
}
