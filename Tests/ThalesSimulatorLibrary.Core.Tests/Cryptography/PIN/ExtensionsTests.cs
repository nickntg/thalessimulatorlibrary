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

        [Fact]
        public void GetDieboldPinBlock()
        {
            Assert.Equal("92389FFFFFFFFFFF", "92389".GetPinBlock(PinBlockFormat.Diebold));
        }

        [Fact]
        public void GetPinDiebold()
        {
            Assert.Equal("92389", "92389FFFFFFFFFFF".GetPin(PinBlockFormat.Diebold));
        }

        [Theory]
        [InlineData("", "987654321")]
        [InlineData("1234567", "987654321")]
        [InlineData("123456", "98765432")]
        public void GetDocutelPinBlockInvalidInput(string pin, string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => pin.GetPinBlock(PinBlockFormat.Docutel, accountOrPadding));
        }

        [Fact]
        public void GetDocutelPinBlock()
        {
            Assert.Equal("5923890987654321", "92389".GetPinBlock(PinBlockFormat.Docutel, "9876543210"));
        }

        [Fact]
        public void GetPinDocutel()
        {
            Assert.Equal("92389", "5923890987654321".GetPin(PinBlockFormat.Docutel));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678901")]
        public void GetPlusPinBlockInvalidData(string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => "12345".GetPinBlock(PinBlockFormat.Plus, accountOrPadding));
        }

        [Fact]
        public void GetPlusPinBlock()
        {
            Assert.Equal("05921A1CBFFFFFED", "92389".GetPinBlock(PinBlockFormat.Plus, "2283400000123456"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678901")]
        public void GetPinPlusInvalidData(string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => "0123456789ABCDEF".GetPin(PinBlockFormat.Plus, accountOrPadding));
        }

        [Fact]
        public void GetPinPlus()
        {
            Assert.Equal("92389", "05921A1CBFFFFFED".GetPin(PinBlockFormat.Plus, "2283400000123456"));
        }

        [Theory]
        [InlineData("12345", "1512345000000000")]
        [InlineData("1234567890", "1A12345678900000")]
        public void GetIso95641PinBlock(string pin, string expected)
        {
            Assert.Equal(expected, pin.GetPinBlock(PinBlockFormat.Iso95641Format1));
        }

        [Theory]
        [InlineData("1512345000000000", "12345")]
        [InlineData("1A12345678900000", "1234567890")]
        public void GetPinIso95641(string pinBlock, string expected)
        {
            Assert.Equal(expected, pinBlock.GetPin(PinBlockFormat.Iso95641Format1));
        }

        [Theory]
        [InlineData("34567", "2534567FFFFFFFFF")]
        [InlineData("3456789012", "2A3456789012FFFF")]
        public void GetEmvPinBlock(string pin, string expected)
        {
            Assert.Equal(expected, pin.GetPinBlock(PinBlockFormat.Emv));
        }

        [Theory]
        [InlineData("2534567FFFFFFFFF", "34567")]
        [InlineData("2A3456789012FFFF", "3456789012")]
        public void GetPinEmv(string pinBlock, string expected)
        {
            Assert.Equal(expected, pinBlock.GetPin(PinBlockFormat.Emv));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678901")]
        public void GetPayNowPayLaterPinBlockInvalidData(string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => "1234".GetPinBlock(PinBlockFormat.PayNowPayLater, accountOrPadding));
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345678901")]
        public void GetPinPayNowPayLaterInvalidData(string accountOrPadding)
        {
            Assert.Throws<ArgumentException>(() => "0123456789ABCDEF".GetPin(PinBlockFormat.PayNowPayLater, accountOrPadding));
        }

        [Theory]
        [InlineData("34567", "123400000123456", "2534167FFFEDCBA9")]
        [InlineData("3456789012", "123400000123456", "2A3416789000CBA9")]
        public void GetPayNowPayLaterPinBlock(string pin, string accountOrPadding, string expected)
        {
            Assert.Equal(expected, pin.GetPinBlock(PinBlockFormat.PayNowPayLater, accountOrPadding));
        }

        [Theory]
        [InlineData("2534167FFFEDCBA9", "123400000123456", "34567")]
        [InlineData("2A3416789000CBA9", "123400000123456", "3456789012")]
        public void GetPinPayNowPayLater(string pinBlock, string accountOrPadding, string expected)
        {
            Assert.Equal(expected, pinBlock.GetPin(PinBlockFormat.PayNowPayLater, accountOrPadding));
        }

        [Fact]
        public void GetIso956413PinBlockInvalidData()
        {
            Assert.Throws<ArgumentException>(() => "1234".GetPinBlock(PinBlockFormat.Iso95641Format3, ""));
        }

        [Fact]
        public void GetPinIso956413InvalidData()
        {
            Assert.Throws<ArgumentException>(() => "3412AC89ABCDEF67".GetPin(PinBlockFormat.Iso95641Format3, ""));
        }

        [Fact]
        public void GetIso956413PinBlock()
        {
            Assert.Equal("3412AC89ABCDEF67", "1234".GetPinBlock(PinBlockFormat.Iso95641Format3, "4321987654321098"));
        }

        [Fact]
        public void GetPinIso956413()
        {
            Assert.Equal("1234", "3412AC89ABCDEF67".GetPin(PinBlockFormat.Iso95641Format3, "4321987654321098"));
        }
    }
}
