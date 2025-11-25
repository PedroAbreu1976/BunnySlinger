using System.Collections.ObjectModel;

using BunnySlinger.Extensions;
using System.Reflection;

namespace BunnySlinger;

public class BunnyMessageTypes {

	public BunnyMessageTypes(Assembly[] assemblies) {
		MessageTypes = new ReadOnlyCollection<Type>(assemblies.GetBunnyTypes());
	}
	
	public IEnumerable<Type> MessageTypes { get; }

	public Type this[string fullName] => MessageTypes.First(x => x.FullName == fullName);
}

