using System.Collections.ObjectModel;
using BunnySlinger.Extensions;
using System.Reflection;

namespace BunnySlinger;

public class BunnyHandlerTypes {

	public BunnyHandlerTypes(Assembly[] assemblies) {
		HandlerTypes = new ReadOnlyDictionary<Type, Type>(assemblies.GetMessageHandlerTypes());
	}
	
	public IReadOnlyDictionary<Type, Type> HandlerTypes { get; }
}

