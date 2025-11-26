using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency
{
    /// <summary>
    /// Represents a log entry for tracking bunny-related events, including details about the bunny,  the type of
    /// catcher, and the associated handling timestamp.
    /// </summary>
    /// <remarks>This class is designed to store information about a specific bunny event, including its
    /// unique identifier,  type, the type of catcher involved, and the time the event was handled. It also includes a
    /// hash code  for efficient indexing and lookup.</remarks>
	[PrimaryKey(nameof(BunnyID), nameof(BunnyType), nameof(BunnyCatcherType))]
	[Index(nameof(HashCode))]
    public class BunnyLog
    {
        /// <summary>
        /// Gets or sets the unique identifier for the bunny.
        /// (This is part of the primary key: <see cref="BunnyID"/> + <see cref="BunnyType"/> + <see cref="BunnyCatcherType"/>)
        /// </summary>
	    [MaxLength(50)]
        public string BunnyID { get; set; }

        /// <summary>
        /// Gets or sets the type of the bunny.
        /// (This is part of the primary key: <see cref="BunnyID"/> + <see cref="BunnyType"/> + <see cref="BunnyCatcherType"/>)
        /// </summary>
        /// <remarks>The maximum length of the value is enforced by the <see
        /// cref="MaxLengthAttribute"/>.</remarks>
        [MaxLength(255)]
        public string BunnyType { get; set; }

        /// <summary>
        /// Gets or sets the type of bunny catcher.
        /// (This is part of the primary key: <see cref="BunnyID"/> + <see cref="BunnyType"/> + <see cref="BunnyCatcherType"/>)
        /// </summary>
        /// <remarks>This property is used to specify or retrieve the type of bunny catcher. Ensure the
        /// value adheres to the maximum length constraint.</remarks>
        [MaxLength(255)]
        public string BunnyCatcherType { get; set; }

        /// <summary>
        /// Gets or sets the hash code value associated with the object.
        /// (This property is indexed for efficient querying.)
        /// </summary>
        public int HashCode { get; set; }
        
        /// <summary>
        /// Gets or sets the timestamp, in Coordinated Universal Time (UTC), indicating when the handle was last
        /// updated.
        /// </summary>
        public DateTime HandleTimeStampUTC { get; set; }
    }
}
