using BunnySlinger.Tests.Bunnies;

namespace BunnySlinger.Tests
{
    public class BunnyMessageTypesTests
    {
        [Fact]
        public void Constructor_PopulatesMessageTypes_FromAssemblies()
        {
            // Arrange
            var assemblies = new[] { typeof(TestBunny).Assembly };
            // Act
            var messageTypes = new BunnySlinger.BunnyMessageTypes(assemblies);
            // Assert
            Assert.Contains(typeof(TestBunny), messageTypes.MessageTypes);
            Assert.Contains(typeof(NonTestBunny), messageTypes.MessageTypes);
            // Should only contain types implementing IBunny
            Assert.All(messageTypes.MessageTypes, t => Assert.True(typeof(BunnySlinger.IBunny).IsAssignableFrom(t)));
        }

        [Fact]
        public void Indexer_ReturnsType_ByFullName()
        {
            var assemblies = new[] { typeof(TestBunny).Assembly };
            var messageTypes = new BunnySlinger.BunnyMessageTypes(assemblies);
            var type = messageTypes[typeof(TestBunny).FullName!];
            Assert.Equal(typeof(TestBunny), type);
        }

        [Fact]
        public void Indexer_Throws_WhenTypeNotFound()
        {
            var assemblies = new[] { typeof(TestBunny).Assembly };
            var messageTypes = new BunnySlinger.BunnyMessageTypes(assemblies);
            Assert.Throws<InvalidOperationException>(() =>
                _ = messageTypes["NonExistent.Type.FullName"]);
        }
    }
}
