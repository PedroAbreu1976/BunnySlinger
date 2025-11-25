using System.Reflection;

namespace BunnySlinger.Extensions;

/// <summary>
/// Provides extension methods for working with assemblies and types related to the Bunny framework.
/// </summary>
/// <remarks>This static class includes methods for discovering and retrieving types that implement specific
/// interfaces used in the Bunny framework, such as <see cref="IBunny"/>, <see cref="IBunnyCatcher{T}"/>, and <see
/// cref="IBunnyInterceptor"/>. These methods are designed to simplify the process of scanning assemblies for relevant
/// types and organizing them into collections for further processing.</remarks>
public static class TypeExtensions {

    /// <summary>
	/// Retrieves a dictionary mapping handler types to their corresponding message types for all classes in the specified
	/// assembly that implement the <see cref="IBunnyCatcher{T}"/> interface.
	/// </summary>
	/// <remarks>This method identifies all non-abstract classes in the specified assembly that implement the
	/// generic <see cref="IBunnyCatcher{T}"/> interface. It then maps each handler type to the message type specified by
	/// the generic argument of the interface.</remarks>
	/// <param name="assembly">The assembly to scan for handler types.</param>
	/// <returns>A dictionary where the keys are the types of classes implementing the <see cref="IBunnyCatcher{T}"/> interface, and
	/// the values are the message types that the handlers are designed to process.</returns>
    public static Dictionary<Type, Type> GetBunnyHandlerTypes(this Assembly assembly) {
		var handlerTypes = assembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract)
			.SelectMany(t => t.GetInterfaces()
				            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBunnyCatcher<>))
				            .Select(i => new {
					            HandlerType = t,
					            MessageType = i.GetGenericArguments()[0]
				            }))
			.ToDictionary(x => x.HandlerType, x => x.MessageType);
		return handlerTypes;
	}

	/// <summary>
	/// Scans the specified assemblies and retrieves a dictionary mapping bunny handler types to their corresponding
	/// implementation types.
	/// </summary>
	/// <remarks>This method aggregates bunny handler types from all provided assemblies. If multiple assemblies
	/// define the same bunny handler type, the implementation type from the last assembly in the enumeration will
	/// overwrite any previous mappings.</remarks>
	/// <param name="assemblies">A collection of assemblies to scan for bunny handler types.</param>
	/// <returns>A dictionary where the keys are the bunny handler types and the values are their corresponding implementation
	/// types. If no bunny handler types are found, an empty dictionary is returned.</returns>
	public static Dictionary<Type, Type> GetBunnyHandlerTypes(this IEnumerable<Assembly> assemblies) {
		var result = new Dictionary<Type, Type>();
		foreach (var assembly in assemblies) {
			var handlerTypes = assembly.GetBunnyHandlerTypes();
			foreach (var kvp in handlerTypes) {
				result[kvp.Key] = kvp.Value;
			}
		}

		return result;
	}

	/// <summary>
	/// Retrieves a list of all non-abstract classes within the specified assembly that implement the <see cref="IBunny"/>
	/// interface.
	/// </summary>
	/// <remarks>This method filters the types in the specified assembly to include only non-abstract classes that
	/// implement the <see cref="IBunny"/> interface.</remarks>
	/// <param name="assembly">The assembly to search for classes implementing the <see cref="IBunny"/> interface. Cannot be <see
	/// langword="null"/>.</param>
	/// <returns>A list of <see cref="Type"/> objects representing the classes that implement the <see cref="IBunny"/> interface.
	/// The list will be empty if no such classes are found.</returns>
	public static List<Type> GetBunnyTypes(this Assembly assembly) {
		var messageTypes = assembly.GetTypes()
			.Where(t => t.IsClass && !t.IsAbstract && typeof(IBunny).IsAssignableFrom(t))
			.ToList();
		return messageTypes;
	}

	/// <summary>
	/// Retrieves a list of types from the specified assemblies that are associated with Bunny.
	/// </summary>
	/// <remarks>This method iterates through the provided assemblies and aggregates the Bunny-related types from
	/// each assembly.</remarks>
	/// <param name="assemblies">A collection of assemblies to search for Bunny-related types.</param>
	/// <returns>A list of <see cref="Type"/> objects representing the Bunny-related types found in the specified assemblies. The
	/// list will be empty if no such types are found.</returns>
	public static List<Type> GetBunnyTypes(this IEnumerable<Assembly> assemblies) {
		var result = new List<Type>();
		foreach (var assembly in assemblies) {
			var messageTypes = assembly.GetBunnyTypes();
			result.AddRange(messageTypes);
		}

		return result;
	}

	/// <summary>
	/// Retrieves a list of types from the specified assembly that implement the <see cref="IBunnyInterceptor"/> interface.
	/// </summary>
	/// <param name="assembly">The assembly to search for types that implement <see cref="IBunnyInterceptor"/>.</param>
	/// <returns>A list of types that are non-abstract classes and implement the <see cref="IBunnyInterceptor"/> interface. The list
	/// will be empty if no such types are found.</returns>
	public static List<Type> GetBunnyInterceptorTypes(this Assembly assembly) {
		var messageTypes = assembly.GetTypes()
			.Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IBunnyInterceptor).IsAssignableFrom(t))
			.ToList();
		return messageTypes;
	}

	/// <summary>
	/// Retrieves a list of types from the specified assemblies that are identified as Bunny interceptor types.
	/// </summary>
	/// <remarks>This method iterates through the provided assemblies and aggregates all types that are identified
	/// as Bunny interceptor types.</remarks>
	/// <param name="assemblies">A collection of assemblies to search for Bunny interceptor types.</param>
	/// <returns>A list of types representing Bunny interceptor types found in the specified assemblies. The list will be empty if
	/// no such types are found.</returns>
	public static List<Type> GetBunnyInterceptorTypes(this IEnumerable<Assembly> assemblies) {
		var result = new List<Type>();
		foreach (var assembly in assemblies) {
			var messageTypes = assembly.GetBunnyInterceptorTypes();
			result.AddRange(messageTypes);
		}

		return result;
	}
}
