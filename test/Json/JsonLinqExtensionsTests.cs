using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid.Json
{
    public class JsonLinqExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public JsonLinqExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CollapseRootNull()
        {
            // Arrange
            var input = @"null";
            var expected = @"null";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootBool()
        {
            // Arrange
            var input = @"true";
            var expected = @"true";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootNumber()
        {
            // Arrange
            var input = @"1";
            var expected = @"1";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootString()
        {
            // Arrange
            var input = @"""""";
            var expected = @"""""";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootArray()
        {
            // Arrange
            var input = @"[ null, null, { }, { a: null }, [ ], [ null ] ]";
            var expected = @"[]";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootArrayWithOneItem()
        {
            // Arrange
            var input = @"[ null, null, { }, { a: null }, [ ], [ null ], 1 ]";
            var expected = @"[1]";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootObject()
        {
            // Arrange
            var input = @"{ a: null, b: [], c: [ null ], d: { }, e: { a: null } }";
            var expected = @"{}";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseRootObjectWithOneProperty()
        {
            // Arrange
            var input = @"{ a: null, b: [], c: [ null ], d: { }, e: { a: null }, f: 1 }";
            var expected = @"{""f"":1}";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseNestedObject()
        {
            // Arrange
            var input = @"{ a: null, b: [], c: [ null ], d: { }, e: { a: null }, f: { a: null, b: 1 } }";
            var expected = @"{""f"":{""b"":1}}";

            // Act & Assert
            TestCollapse(input, expected);
        }

        [Fact]
        public void CollapseNestedArray()
        {
            // Arrange
            var input = @"{ a: null, b: [], c: [ null ], d: { }, e: { a: null }, f: { a: [], b: [null, 1, null, { }] } }";
            var expected = @"{""f"":{""b"":[1]}}";

            // Act & Assert
            TestCollapse(input, expected);
        }

        private void TestCollapse(string input, string expected)
        {
            // Act
            var actual = Collapse(input);

            // Assert
            Assert.Equal(expected, actual);
        }

        private string Collapse(string input)
        {
            var token = JToken.Parse(input);
            token.Collapse();
            return token.ToString(Formatting.None);
        }
    }
}
