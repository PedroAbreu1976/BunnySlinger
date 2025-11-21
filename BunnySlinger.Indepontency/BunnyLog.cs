using System.ComponentModel.DataAnnotations;


namespace BunnySlinger.Idempotency
{
    public class BunnyLog
    {
	    [MaxLength(50)]
        public string BunnyID { get; set; }

        [MaxLength(255)]
        public string BunnyType { get; set; }

        [MaxLength(255)]
        public string BunnyCatcherType { get; set; }
        
        public int HashCode { get; set; }
        
        public DateTime HandleTimeStampUTC { get; set; }
    }
}
