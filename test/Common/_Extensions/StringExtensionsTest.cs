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
        public void EnsureTerminatingPunctuation_AddsDefaultPuncuation()
        {
            // Arrange
            var expected = ".";
            var str = "The brown fox jumped over the gate";

            // Act
            str = str.EnsureTerminatingPunctuation();

            // Assert
            Assert.EndsWith(expected, str);
        }

        [Fact]
        public void EnsureTerminatingPunctuation_AddsSpecifiedPuncuation()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate";

            // Act
            str = str.EnsureTerminatingPunctuation(expected);

            // Assert
            Assert.EndsWith(expected, str);
        }

        [Fact]
        public void EnsureTerminatingPunctuation_AddsSpecifiedPuncuationPriorToEndingWhiteSpace()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate  \t\n";

            // Act
            str = str.EnsureTerminatingPunctuation(expected);

            // Assert
            Assert.EndsWith($"{expected}  \t\n", str);
        }

        [Fact]
        public void EnsureTerminatingPunctuation_LeavesExistingPuncuationAndEndingWhiteSpaceAsIs()
        {
            // Arrange
            var expected = "!";
            var str = "The brown fox jumped over the gate.  \t\n";

            // Act
            str = str.EnsureTerminatingPunctuation(expected);

            // Assert
            Assert.EndsWith($".  \t\n", str);
        }
    }
}
