﻿using ThalesSimulatorLibrary.Core.Cryptography.PIN;
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
    }
}
