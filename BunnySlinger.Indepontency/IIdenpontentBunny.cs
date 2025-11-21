using System.ComponentModel.DataAnnotations;


namespace BunnySlinger.Idempotency
{
    public interface IIdempotentBunny : IBunny
    {
        [MaxLength(50)]
        string BunnyID { get; set; }
    }
}
