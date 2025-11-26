using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BunnySlinger.Idempotency
{
	/// <summary>
	/// Provides an interceptor that ensures idempotent processing of bunnies by preventing duplicate handling based on a
	/// unique log entry for each bunny.
	/// </summary>
	/// <remarks>This interceptor is designed to ensure that each bunny is processed only once by maintaining a log
	/// of processed bunnies. It uses the provided database context to check for existing logs and to persist new logs when
	/// a bunny is successfully processed. The interceptor wraps the processing logic in a database transaction to ensure
	/// consistency.</remarks>
	/// <typeparam name="TContext">The type of the database context used to persist bunny logs. Must derive from <see cref="DbContext"/>.</typeparam>
	/// <param name="context"></param>
	/// <param name="handlerTypes"></param>
	/// <param name="logger"></param>
    public class IdempotentInterceptor<TContext>(
	    TContext context, 
	    BunnyHandlerTypes handlerTypes,
	    ILogger<IdempotentInterceptor<TContext>> logger) : IBunnyInterceptor<IIdempotentBunny>
	    where TContext : DbContext
    {
		/// <summary>
		/// Attempts to process a bunny using the specified catcher function and logs the result.
		/// </summary>
		/// <remarks>This method ensures that the bunny is processed in a transactional context. If the catcher
		/// function successfully processes the bunny,  the transaction is committed, and the bunny is marked as processed. If
		/// an exception occurs during processing, the transaction is rolled back,  and the exception is logged.</remarks>
		/// <param name="bunny">The bunny to be processed. Must implement <see cref="IIdempotentBunny"/>.</param>
		/// <param name="catcher">A function that processes the bunny and returns a task representing a boolean result.  The function should return
		/// <see langword="true"/> if the bunny was successfully processed; otherwise, <see langword="false"/>.</param>
		/// <param name="handlerType">The type of the handler responsible for processing the bunny. Used for logging and tracking purposes.</param>
		/// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the bunny was
		/// successfully processed;  otherwise, <see langword="false"/>.</returns>
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
