using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace BunnySlinger.Idempotency
{
    public class IdempotentInterceptor<TContext>(
	    TContext context, 
	    BunnyHandlerTypes handlerTypes,
	    ILogger<IdempotentInterceptor<TContext>> logger) : IBunnyInterceptor<IIdempotentBunny>
	    where TContext : DbContext
    {
	    public async Task<bool> OnBunnyCatch(IIdempotentBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType) {
		    try
		    {
			    var bunnyLog = CreateBunnyLog(handlerType, bunny);

			    var canProcess = await CanProcessBunnyAsync(bunnyLog);
			    if (!canProcess)
			    {
				    return false;
			    }

			    await using var transaction = await context.Database.BeginTransactionAsync();

			    var result = await catcher(bunny);

			    if (result)
			    {
				    await BunnyProcessed(bunnyLog);
			    }

			    await transaction.CommitAsync();

			    return result;
		    }
		    catch (Exception ex)
		    {
			    logger.LogError(ex, ex.Message);
				return false;
		    }
        }

	    //public async Task<bool> OnBunnyCatch(IBunny bunny, Func<IBunny, Task<bool>> catcher, Type handlerType)
	    //{
		   // if (bunny is IIdempotentBunny idempotentBunny) {
			  //  try {
				 //   var bunnyLog = CreateBunnyLog(handlerType, idempotentBunny);

				 //   var canProcess = await CanProcessBunnyAsync(bunnyLog);
				 //   if (!canProcess) {
					//    return false;
				 //   }

				 //   await using var transaction = await context.Database.BeginTransactionAsync();

				 //   var result = await catcher(bunny);

				 //   if (result) {
					//    await BunnyProcessed(bunnyLog);
				 //   }

				 //   await transaction.CommitAsync();

				 //   return result;
			  //  }
			  //  catch (Exception ex) {
					//logger.LogError(ex, ex.Message);
			  //  }
		   // }

		   // return await catcher(bunny);
	    //}

	    private BunnyLog CreateBunnyLog(Type handlerType, IIdempotentBunny bunny) {
		    var result = new BunnyLog
		    {
			    BunnyID = bunny.BunnyID,
			    BunnyCatcherType = handlerType.FullName!,
			    BunnyType = handlerTypes.HandlerTypes[handlerType].FullName!,
		    };
		    result.HashCode = result.CreateHashCode();
		    return result;
	    }

        private async Task<bool> CanProcessBunnyAsync(BunnyLog bunnyLog) {
		    var exists = await context.GetBunnyLogs()
			    .Where(x => x.HashCode == bunnyLog.HashCode)
			    .Where(x => x.BunnyCatcherType == bunnyLog.BunnyCatcherType)
			    .Where(x => x.BunnyType == bunnyLog.BunnyType)
			    .Where(x => x.BunnyID == bunnyLog.BunnyID)
			    .AnyAsync();
			return !exists;
	    }

	    private async Task BunnyProcessed(BunnyLog bunnyLog) {
		    await context.GetBunnyLogs().AddAsync(bunnyLog);
		    await context.SaveChangesAsync();
	    }

	    
    }
}
