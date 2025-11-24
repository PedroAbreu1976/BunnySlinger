using System;
using System.Linq;
using System.Reflection;
using BunnySlinger;
using BunnySlinger.Tests.Bunnies;
using BunnySlinger.Tests.Catchers;

using Xunit;

namespace BunnySlinger.Tests
{
    public class BunnyHandlerTypesTests
    {
        [Fact]
        public void Constructor_PopulatesHandlerTypes_FromAssemblies()
        {
            // Arrange: use the assembly containing TestBunnyCatcher
            var assemblies = new[] { typeof(TestBunnyCatcher).Assembly };
            // Act
            var handlerTypes = new BunnyHandlerTypes(assemblies);
            // Assert
            // Should contain TestBunnyCatcher as key, TestBunny as value
            Assert.Contains(typeof(TestBunnyCatcher), handlerTypes.HandlerTypes.Keys);
            Assert.Equal(typeof(TestBunny), handlerTypes.HandlerTypes[typeof(TestBunnyCatcher)]);
        }

        [Fact]
        public void HandlerTypes_IsReadOnly()
        {
            var assemblies = new[] { typeof(TestBunnyCatcher).Assembly };
            var handlerTypes = new BunnyHandlerTypes(assemblies);
            Assert.IsAssignableFrom<System.Collections.ObjectModel.ReadOnlyDictionary<Type, Type>>(handlerTypes.HandlerTypes);
        }
    }
}
