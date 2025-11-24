using BunnySlinger.Extensions;
using Xunit;

namespace BunnySlinger.Tests
{
    public class CollectionExtensionsTests
    {
        [Fact]
        public void ForEach_ExecutesActionOnAllElements()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var results = new List<int>();

            // Act
            items.ForEach(results.Add);

            // Assert
            Assert.Equal(items, results);
        }

        [Fact]
        public void ForEach_WithEmptyCollection_DoesNotExecuteAction()
        {
            // Arrange
            var items = new List<int>();
            var wasCalled = false;

            // Act
            items.ForEach(_ => wasCalled = true);

            // Assert
            Assert.False(wasCalled);
        }

        [Fact]
        public void ForEach_WithNullAction_ThrowsArgumentNullException()
        {
            // Arrange
            var items = new List<int> { 1 };

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => items.ForEach(null));
        }
    }
}
