using ThalesSimulatorLibrary.Core.Utility;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Utility
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData("no", false)]
        [InlineData("1", true)]
        [InlineData("1ABCDEF", true)]
        public void VerifyIsHex(string text, bool expected)
        {
            Assert.Equal(expected, text.IsHex());
        }

        [Fact]
        public void VerifyGetBytesOfHexString()
        {
            var hex = "ABCD0123";
            var bytes = hex.GetBytesOfHexString();

            Assert.Equal(4, bytes.Length);
            Assert.Equal(0xab, bytes[0]);
            Assert.Equal(0xcd, bytes[1]);
            Assert.Equal(0x01, bytes[2]);
            Assert.Equal(0x23, bytes[3]);
        }

        [Fact]
        public void GetBytesOfHexStringInvalidLength()
        {
            Assert.Throws<InvalidOperationException>(() => "123".GetBytesOfHexString());
        }

        [Fact]
        public void GetBytesOfHexStringInvalidString()
        {
            Assert.Throws<InvalidOperationException>(() => "123Z".GetBytesOfHexString());
        }

        [Theory]
        [InlineData("0100", Parity.None, true)]
        [InlineData("0101", Parity.None, true)]
        [InlineData("01011111", Parity.Even, true)]
        [InlineData("01011111", Parity.Odd, false)]
        [InlineData("01011110", Parity.Even, false)]
        [InlineData("01011110", Parity.Odd, true)]
        public void VerifyParityCheck(string text, Parity parity, bool expected)
        {
            var b = Convert.ToByte(text, 2);
            Assert.Equal(expected, b.HasParity(parity));
        }

        [Theory]
        [InlineData("no parity check", Parity.None, true)]
        [InlineData("", Parity.None, true)]
        [InlineData("01", Parity.Odd, true)]
        [InlineData("00", Parity.Even, true)]
        [InlineData("0123456789ABCDEF", Parity.Odd, true)]
        [InlineData("0022446688AACCEE", Parity.Even, true)]
        [InlineData("0122446688AACCEE", Parity.Even, false)]
        [InlineData("1123456789ABCDEF", Parity.Odd, false)]
        [InlineData("U0123456789ABCDEF", Parity.Odd, true)]
        [InlineData("X0022446688AACCEE", Parity.Even, true)]
        [InlineData("T0122446688AACCEE", Parity.Even, false)]
        [InlineData("Y1123456789ABCDEF", Parity.Odd, false)]
        public void VerifyParityCheckOfKey(string key, Parity parity, bool expected)
        {
            Assert.Equal(expected, key.HasParity(parity));
        }

        [Fact]
        public void ParityCheckInvalidKey()
        {
            Assert.Throws<InvalidOperationException>(() => "not a key ".HasParity(Parity.Odd));
        }

        [Theory]
        [InlineData("00000100", Parity.None, "00000100")]
        [InlineData("00000101", Parity.None, "00000101")]
        [InlineData("01011111", Parity.Even, "01011111")]
        [InlineData("01011111", Parity.Odd, "01011110")]
        [InlineData("01011110", Parity.Even, "01011111")]
        [InlineData("01011110", Parity.Odd, "01011110")]
        public void MakeParity(string text, Parity parity, string expected)
        {
            var b = Convert.ToByte(text, 2);
            Assert.Equal(expected, Convert.ToString(b.MakeParity(parity), 2).PadLeft(8, '0'));
        }

        [Theory]
        [InlineData("0000000000000000", "0123456789ABCDEF", "0123456789ABCDEF")]
        [InlineData("1111233345556777", "0123456789ABCDEF", "10326654CCFEAA98")]
        public void XorKeys(string key1, string key2, string expected)
        {
            Assert.Equal(expected, key1.Xor(key2));
        }
    }
}
