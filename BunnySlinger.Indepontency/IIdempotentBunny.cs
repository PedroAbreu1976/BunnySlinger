using System.ComponentModel.DataAnnotations;

namespace BunnySlinger.Idempotency
{
    /// <summary>
    /// Represents a bunny with a unique identifier that supports idempotent operations.
    /// </summary>
    /// <remarks>This interface extends <see cref="IBunny"/> and introduces a unique identifier for the bunny.
    /// The <see cref="BunnyID"/> property is constrained to a maximum length of 50 characters.</remarks>
    public interface IIdempotentBunny : IBunny
    {
		/// <summary>
        /// Gets or sets the unique identifier for the bunny.
        /// </summary>
        [MaxLength(50)]
        [Required]
        string BunnyID { get; set; }
    }
}
