using ThalesSimulatorLibrary.Core.Cryptography.Key;
using ThalesSimulatorLibrary.Core.Exceptions;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.Key
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(KeyScheme.SingleLengthAnsi, "Z")]
        [InlineData(KeyScheme.DoubleLengthVariant, "U")]
        [InlineData(KeyScheme.DoubleLengthAnsi, "X")]
        [InlineData(KeyScheme.TripleLengthVariant, "T")]
        [InlineData(KeyScheme.TripleLengthAnsi, "Y")]
        public void VerifySchemeTag(KeyScheme scheme, string expected)
        {
            Assert.Equal(expected, scheme.GetKeySchemeTag());
        }

        [Fact]
        public void InvalidSchemeForTag()
        {
            Assert.Throws<InvalidKeySchemeException>(() => KeyScheme.Unspecified.GetKeySchemeTag());
        }

        [Theory]
        [InlineData("", KeyScheme.SingleLengthAnsi)]
        [InlineData("Z", KeyScheme.SingleLengthAnsi)]
        [InlineData("U", KeyScheme.DoubleLengthVariant)]
        [InlineData("X", KeyScheme.DoubleLengthAnsi)]
        [InlineData("T", KeyScheme.TripleLengthVariant)]
        [InlineData("Y", KeyScheme.TripleLengthAnsi)]
        public void VerifyScheme(string scheme, KeyScheme expected)
        {
            Assert.Equal(expected, scheme.GetKeyScheme());
        }

        [Fact]
        public void InvalidTag()
        {
            Assert.Throws<InvalidKeyTagException>(() => "L".GetKeyScheme());
        }

        [Theory]
        [InlineData("T123","123")]
        [InlineData("X123","123")]
        [InlineData("Y123","123")]
        [InlineData("U123","123")]
        [InlineData("Z123","123")]
        [InlineData("123", "123")]
        [InlineData("", "")]
        public void VerifyStripKeySchemeTag(string text, string expected)
        {
            Assert.Equal(expected, text.StripKeySchemeTag());
        }
    }
}
