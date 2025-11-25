using System.Collections.ObjectModel;
using BunnySlinger.Extensions;
using System.Reflection;

namespace BunnySlinger;

/// <summary>
/// Provides a collection of message types used by the Bunny messaging system.
/// </summary>
/// <remarks>This class initializes and exposes a read-only collection of message types derived from the specified
/// assemblies. It also provides an indexer to retrieve a specific message type by its fully qualified name.</remarks>
public class BunnyMessageTypes {
	/// <summary>
	/// Initializes a new instance of the <see cref="BunnyMessageTypes"/> class,  loading message types from the specified
	/// assemblies.
	/// </summary>
	/// <remarks>The constructor scans the provided assemblies for types relevant to the  Bunny messaging system and
	/// initializes the <see cref="MessageTypes"/>  property with a read-only collection of these types.</remarks>
	/// <param name="assemblies">An array of assemblies to scan for message types.  Cannot be null or empty.</param>
	public BunnyMessageTypes(Assembly[] assemblies) {
		MessageTypes = new ReadOnlyCollection<Type>(assemblies.GetBunnyTypes());
	}
	
	/// <summary>
	/// Gets the collection of message types supported by the current instance.
	/// </summary>
	public IEnumerable<Type> MessageTypes { get; }

	/// <summary>
	/// Gets the <see cref="Type"/> associated with the specified fully qualified name.
	/// </summary>
	/// <param name="fullName">The fully qualified name of the type to retrieve.</param>
	/// <returns>The <see cref="Type"/> that matches the specified fully qualified name.</returns>
	public Type this[string fullName] => MessageTypes.First(x => x.FullName == fullName);
}

