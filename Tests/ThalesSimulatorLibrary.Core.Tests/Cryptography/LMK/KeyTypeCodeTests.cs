using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using ThalesSimulatorLibrary.Core.Exceptions;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.LMK
{
    public class KeyTypeCodeTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("1234")]
        public void InvalidKeyTypeCode(string text)
        {
            Assert.Throws<InvalidKeyTypeCodeException>(() => new KeyTypeCode(text));
        }

        [Fact]
        public void InvalidVariant()
        {
            Assert.Throws<InvalidVariantException>(() => new KeyTypeCode("A00"));
            Assert.Throws<InvalidVariantException>(() => new KeyTypeCode("A", LmkPair.Pair0405));
        }

        [Fact]
        public void InvalidLmkCode()
        {
            Assert.Throws<InvalidLmkCodeException>(() => new KeyTypeCode("0AA"));
        }

        [Theory]
        [InlineData("0", "00")]
        [InlineData("1", "00")]
        [InlineData("2", "00")]
        [InlineData("3", "00")]
        [InlineData("4", "00")]
        [InlineData("5", "00")]
        [InlineData("6", "00")]
        [InlineData("7", "00")]
        [InlineData("8", "00")]
        [InlineData("9", "00")]
        public void Valid(string variant, string lmkPair)
        {
            _ = new KeyTypeCode($"{variant}{lmkPair}");
        }
    }
}
