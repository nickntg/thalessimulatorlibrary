using ThalesSimulatorLibrary.Core.Cryptography.PIN;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.PIN
{
    public class ExtensionsTests
    {
        [Fact]
        public void GetPinBlockPinIsEmpty()
        {
            Assert.Throws<ArgumentException>(() => "".GetPinBlock(PinBlockFormat.AnsiX98));
        }

        [Theory]
        [InlineData("")]
        [InlineData("123456")]
        public void GetAnsiPinBlockAccountIncorrect(string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => "1234".GetPinBlock(PinBlockFormat.AnsiX98, accountOrPadding));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345")]
        public void GetPinPinBlockIncorrect(string pinBlock)
        {
            Assert.Throws<ArgumentException>(() => pinBlock.GetPin(PinBlockFormat.AnsiX98));
        }

        [Fact]
        public void GetAnsiPinBlock()
        {
            Assert.Equal("0592789FFFEDCBA9", "92389".GetPinBlock(PinBlockFormat.AnsiX98, "400000123456"));
        }

        [Fact]
        public void GetPinAnsiPinBlock()
        {
            Assert.Equal("92389", "0592789FFFEDCBA9".GetPin(PinBlockFormat.AnsiX98, "400000123456"));
        }
    }
}
