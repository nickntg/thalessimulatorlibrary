using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using ThalesSimulatorLibrary.Core.Exceptions;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.LMK
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData("00", "Pair0405")]
        [InlineData("01", "Pair0607")]
        [InlineData("02", "Pair1415")]
        [InlineData("03", "Pair1617")]
        [InlineData("04", "Pair1819")]
        [InlineData("05", "Pair2021")]
        [InlineData("06", "Pair2223")]
        [InlineData("07", "Pair2425")]
        [InlineData("08", "Pair2627")]
        [InlineData("09", "Pair2829")]
        [InlineData("0A", "Pair3031")]
        [InlineData("0B", "Pair3233")]
        [InlineData("0C", "Pair3435")]
        [InlineData("0D", "Pair3637")]
        [InlineData("0E", "Pair3839")]
        public void ValidateLmkPairFromLmkCode(string lmkCode, string lmkPair)
        {
            var pair = Enum.Parse<LmkPair>(lmkPair);
            Assert.Equal(pair, lmkCode.GetLmkPairFromLmkCode());
        }

        [Theory]
        [InlineData("")]
        [InlineData("0000")]
        [InlineData("0F")]
        [InlineData("XI")]
        public void InvalidLmkCode(string lmkCode)
        {
            Assert.Throws<InvalidLmkCodeException>(() => lmkCode.GetLmkPairFromLmkCode());
        }
    }
}
