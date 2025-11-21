using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;


namespace BunnySlinger.Idempotency
{
	[PrimaryKey(nameof(BunnyID), nameof(BunnyType), nameof(BunnyCatcherType))]
	[Index(nameof(HashCode))]
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
