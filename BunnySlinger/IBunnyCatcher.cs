namespace BunnySlinger
{
	public interface IBunnyCatcher {
		Task<bool> CatchBunnyAsync(IBunny bunny);
	}

    public interface IBunnyCatcher<TBunny> : IBunnyCatcher where TBunny : IBunny {
		Task<bool> CatchBunnyAsync(TBunny bunny);

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
