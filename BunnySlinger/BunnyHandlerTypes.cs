using System.Collections.ObjectModel;
using BunnySlinger.Extensions;
using System.Reflection;

namespace BunnySlinger;

/// <summary>
/// Represents a collection of handler types for processing bunny-related operations.
/// </summary>
/// <remarks>This class initializes a read-only dictionary that maps handler types to their corresponding
/// implementation types. The handler types are discovered from the provided assemblies during construction.</remarks>
public class BunnyHandlerTypes {

	/// <summary>
	/// Initializes a new instance of the <see cref="BunnyHandlerTypes"/> class,  mapping handler types from the provided
	/// assemblies.
	/// </summary>
	/// <remarks>The constructor scans the specified assemblies for types that qualify as bunny handlers  and
	/// creates a read-only dictionary mapping these types. The resulting dictionary is  accessible via the <see
	/// cref="HandlerTypes"/> property.</remarks>
	/// <param name="assemblies">An array of assemblies to scan for bunny handler types.  Cannot be null or empty.</param>
	public BunnyHandlerTypes(Assembly[] assemblies) {
		HandlerTypes = new ReadOnlyDictionary<Type, Type>(assemblies.GetBunnyHandlerTypes());
	}
	
	/// <summary>
	/// Gets a read-only dictionary that maps bunny handler types to their corresponding bunny types.
	/// </summary>
	/// <remarks>This property is typically used to retrieve the mapping of handler interfaces to their concrete
	/// implementations in scenarios such as dependency injection or message handling frameworks.</remarks>
	public IReadOnlyDictionary<Type, Type> HandlerTypes { get; }
}

