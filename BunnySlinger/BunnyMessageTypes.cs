using System.Collections.ObjectModel;

using BunnySlinger.Extensions;
using System.Reflection;

namespace BunnySlinger;

public class BunnyMessageTypes {

	public BunnyMessageTypes(Assembly[] assemblies) {
		MessageTypes = new ReadOnlyCollection<Type>(assemblies.GetMessageTypes());
	}
	
	public IEnumerable<Type> MessageTypes { get; }
}

