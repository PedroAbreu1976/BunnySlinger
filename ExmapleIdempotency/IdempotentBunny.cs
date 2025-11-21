using BunnySlinger.Idempotency;

namespace ExampleIdempotency
{

	public class IdempotentBunny : IIdempotentBunny
    {
	    public string BunnyID { get; set; }
        
        public string Message { get; set; }
    }

}
