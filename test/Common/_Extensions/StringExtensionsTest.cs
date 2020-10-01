using System;
using Xunit;
using Xunit.Abstractions;

namespace KodeAid
{
    public class StringExtensionsTest
    {
        private readonly ITestOutputHelper _output;

        public StringExtensionsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void EnsureTerminatingPuncuation_AddsDefaultPuncuation()
        {
            // Arrange
            var expected = ".";
            var str = "The brown fox jumped over the gate";

            // Act
            str = str.EnsureTerminatingPuncuation();

            // Assert
            Assert.EndsWith(expected, str);
        }

        [Fact]
        public void EnsureTerminatingPuncuation_AddsSpecifiedPuncuation()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate";

            // Act
            str = str.EnsureTerminatingPuncuation(expected);

            // Assert
            Assert.EndsWith(expected, str);
        }

        [Fact]
        public void EnsureTerminatingPuncuation_AddsSpecifiedPuncuationPriorToEndingWhiteSpace()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate  \t\n";

            // Act
            str = str.EnsureTerminatingPuncuation(expected);

            // Assert
            Assert.EndsWith($"{expected}  \t\n", str);
        }

        [Fact]
        public void EnsureTerminatingPuncuation_LeavesExistingPuncuationAndEndingWhiteSpaceAsIs()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate.  \t\n";

            // Act
            str = str.EnsureTerminatingPuncuation(expected);

            // Assert
            Assert.EndsWith($".  \t\n", str);
        }
    }
}
