namespace BunnySlinger
{
	/// <summary>
	/// Defines a contract for catching bunnies asynchronously.
	/// </summary>
	/// <remarks>Implementations of this interface are responsible for providing the logic to catch a bunny. The
	/// operation is asynchronous and returns a value indicating whether the bunny was successfully caught.</remarks>
	public interface IBunnyCatcher {
		/// <summary>
		/// Attempts to catch the specified bunny asynchronously.
		/// </summary>
		/// <param name="bunny">The bunny to catch. Cannot be null.</param>
		/// <returns><see langword="true"/> if the bunny was successfully caught; otherwise, <see langword="false"/>.</returns>
		Task<bool> CatchBunnyAsync(IBunny bunny);
	}

	/// <summary>
	/// Defines a contract for catching bunnies of a specific type.
	/// </summary>
	/// <remarks>This interface extends <see cref="IBunnyCatcher"/> to provide type-safe operations for catching
	/// bunnies. Implementations should ensure that only bunnies of the specified type <typeparamref name="TBunny"/> are
	/// processed.</remarks>
	/// <typeparam name="TBunny">The type of bunny that this catcher can handle. Must implement the <see cref="IBunny"/> interface.</typeparam>
    public interface IBunnyCatcher<TBunny> : IBunnyCatcher where TBunny : IBunny {
		/// <summary>
		/// Attempts to catch the specified bunny asynchronously.
		/// </summary>
		/// <param name="bunny">The bunny to catch. Cannot be <see langword="null"/>.</param>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the bunny was
		/// successfully caught; otherwise, <see langword="false"/>.</returns>
		Task<bool> CatchBunnyAsync(TBunny bunny);

		/// <inheritdoc/>
		Task<bool> IBunnyCatcher.CatchBunnyAsync(IBunny bunny)
		{
			if (bunny.GetType() != typeof(TBunny))
			{
				throw new ArgumentException($"Invalid bunny type [{bunny.GetType().Name}]", nameof(bunny));
			}
			return CatchBunnyAsync((TBunny)bunny);
		}
    }

}
