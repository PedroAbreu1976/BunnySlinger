using Microsoft.EntityFrameworkCore;

namespace BunnySlinger.Idempotency
{
    internal class IdempotentInterceptor<TContext>(
	    TContext context, 
	    BunnyHandlerTypes handlerTypes)
	    where TContext : DbContext
    {
	    public async Task<bool> CanProcessBunnyAsync(Type handlerType, IIdempotentBunny bunny) {
		    var newBunnyLog = new BunnyLog {
				BunnyID = bunny.BunnyID,
				BunnyCatcherType = handlerType.FullName!,
				BunnyType = handlerTypes.HandlerTypes[bunny.GetType()].FullName!,
		    };
		    newBunnyLog.HashCode = newBunnyLog.CreateHashCode();

		    var exists = await context.GetBunnyLogs()
			    .Where(x => x.HashCode == newBunnyLog.HashCode)
			    .Where(x => x.BunnyCatcherType == newBunnyLog.BunnyCatcherType)
			    .Where(x => x.BunnyType == newBunnyLog.BunnyType)
			    .Where(x => x.BunnyID == newBunnyLog.BunnyID)
			    .AnyAsync();
			return !exists;
	    }

	    public async Task BunnyProcessed(Type handlerType, IIdempotentBunny bunny) {
		    var newBunnyLog = new BunnyLog {
			    BunnyID = bunny.BunnyID,
			    BunnyCatcherType = handlerType.FullName!,
			    BunnyType = handlerTypes.HandlerTypes[bunny.GetType()].FullName!,
				HandleTimeStampUTC = DateTime.UtcNow
		    };
		    newBunnyLog.HashCode = newBunnyLog.CreateHashCode();

		    await context.GetBunnyLogs().AddAsync(newBunnyLog);
		    await context.SaveChangesAsync();
	    }
    }
}
