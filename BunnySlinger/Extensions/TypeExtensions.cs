using System.Reflection;
using System.Runtime.InteropServices;


namespace BunnySlinger.Extensions
{
    internal static class TypeExtensions
    {
        public static Dictionary<Type, Type> GetMessageHandlerTypes(this Assembly assembly)
        {
            var handlerTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IBunnyCatcher<>))
                    .Select(i => new { HandlerType = t, MessageType = i.GetGenericArguments()[0] }))
                .ToDictionary(x => x.HandlerType, x => x.MessageType);
            return handlerTypes;
        }
        
        public static Dictionary<Type, Type> GetMessageHandlerTypes(this IEnumerable<Assembly> assemblies)
        {
            var result = new Dictionary<Type, Type>();
            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetMessageHandlerTypes();
                foreach (var kvp in handlerTypes)
                {
                    result[kvp.Key] = kvp.Value;
                }
            }
            return result;
        }
        
        public static List<Type> GetMessageTypes(this Assembly assembly)
        {
            var messageTypes = assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IBunny).IsAssignableFrom(t))
                .ToList();
            return messageTypes;
        }
        
        public static List<Type> GetMessageTypes(this IEnumerable<Assembly> assemblies)
        {
            var result = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var messageTypes = assembly.GetMessageTypes();
                result.AddRange(messageTypes);
            }
            return result;
        }

        public static List<Type> GetInterceptorTypes(this Assembly assembly)
        {
	        var messageTypes = assembly
		        .GetTypes()
		        .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IBunnyInterceptor).IsAssignableFrom(t))
		        .ToList();
	        return messageTypes;
        }

        public static List<Type> GetInterceptorTypes(this IEnumerable<Assembly> assemblies)
        {
	        var result = new List<Type>();
	        foreach (var assembly in assemblies)
	        {
		        var messageTypes = assembly.GetInterceptorTypes();
		        result.AddRange(messageTypes);
	        }
	        return result;
        }
    }
}
