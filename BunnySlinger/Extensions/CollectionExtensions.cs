namespace BunnySlinger.Extensions
{
	/// <summary>
	/// Provides extension methods for collections.
	/// </summary>
	/// <remarks>This class contains utility methods that extend the functionality of collection types,  such as
	/// <see cref="IEnumerable{T}"/>.</remarks>
    public static class CollectionExtensions
    {
		/// <summary>
		/// Executes the specified action on each element of the <see cref="IEnumerable{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the collection.</typeparam>
		/// <param name="collection">The collection of elements to iterate over. Cannot be <see langword="null"/>.</param>
		/// <param name="action">The action to perform on each element. Cannot be <see langword="null"/>.</param>
	    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
		    foreach (var item in collection) {
			    action(item);
		    }
	    }

	    public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T,Task> action)
	    {
		    foreach (var item in collection)
		    {
			    await action(item);
		    }
	    }
    }
}
