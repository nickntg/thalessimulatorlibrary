/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/ 

using NUnit.Framework;
using ThalesSim.Core.Cryptography;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Utility;

namespace ThalesSim.Tests.Unit.Utility
{
    [TestFixture]
    public class ExtensionTests
    {
        [Test]
        [TestCase("0", true)]
        [TestCase("1", true)]
        [TestCase("2", true)]
        [TestCase("3", true)]
        [TestCase("4", true)]
        [TestCase("5", true)]
        [TestCase("6", true)]
        [TestCase("7", true)]
        [TestCase("8", true)]
        [TestCase("9", true)]
        [TestCase("A", true)]
        [TestCase("a", true)]
        [TestCase("B", true)]
        [TestCase("b", true)]
        [TestCase("C", true)]
        [TestCase("c", true)]
        [TestCase("D", true)]
        [TestCase("d", true)]
        [TestCase("E", true)]
        [TestCase("e", true)]
        [TestCase("F", true)]
        [TestCase("f", true)]
        [TestCase("0123456789ABCDEFabcdef", true)]
        [TestCase("not hex", false)]
        [TestCase("ABCD 123", false)]
        [TestCase("", false)]
        public void TestIsHex (string text, bool expected)
        {
            Assert.AreEqual(expected, text.IsHex());
        }

        [Test]
        [TestCase("0", true)]
        [TestCase("1", true)]
        [TestCase("0101010011110101", true)]
        [TestCase("", false)]
        [TestCase("0ABCDEF", false)]
        public void TestIsBinary (string text, bool expected)
        {
            Assert.AreEqual(expected, text.IsBinary());
        }

        [Test]
        [TestCase(KeyScheme.DoubleLengthKeyAnsi, "X")]
        [TestCase(KeyScheme.DoubleLengthKeyVariant, "U")]
        [TestCase(KeyScheme.SingleLengthKey, "Z")]
        [TestCase(KeyScheme.TripleLengthKeyAnsi, "Y")]
        [TestCase(KeyScheme.TripleLengthKeyVariant, "T")]
        public void VerifyKeySchemeChar (KeyScheme scheme, string expected)
        {
            Assert.AreEqual(expected, scheme.GetKeySchemeChar());
        }

        [Test]
        [TestCase("Z0123", true)]
        [TestCase("z0123", true)]
        [TestCase("U0123", true)]
        [TestCase("u0123", true)]
        [TestCase("Y0123", true)]
        [TestCase("y0123", true)]
        [TestCase("T0123", true)]
        [TestCase("t0123", true)]
        [TestCase("X0123", true)]
        [TestCase("x0123", true)]
        [TestCase("01234", false)]
        [TestCase("", false)]
        public void TestStartsWithKeyScheme (string text, bool expected)
        {
            Assert.AreEqual(expected, text.StartsWithKeyScheme());
        }

        [Test]
        [TestCase("Z0123", "0123")]
        [TestCase("z0123", "0123")]
        [TestCase("U0123", "0123")]
        [TestCase("u0123", "0123")]
        [TestCase("Y0123", "0123")]
        [TestCase("y0123", "0123")]
        [TestCase("T0123", "0123")]
        [TestCase("t0123", "0123")]
        [TestCase("X0123", "0123")]
        [TestCase("x0123", "0123")]
        [TestCase("01234", "01234")]
        [TestCase("", "")]
        public void TestStripKeyScheme (string text, string expected)
        {
            Assert.AreEqual(expected, text.StripKeyScheme());
        }

        [Test]
        [TestCase("X", KeyScheme.DoubleLengthKeyAnsi)]
        [TestCase("U", KeyScheme.DoubleLengthKeyVariant)]
        [TestCase("Z", KeyScheme.SingleLengthKey)]
        [TestCase("Y", KeyScheme.TripleLengthKeyAnsi)]
        [TestCase("T", KeyScheme.TripleLengthKeyVariant)]
        [TestCase("x", KeyScheme.DoubleLengthKeyAnsi)]
        [TestCase("u", KeyScheme.DoubleLengthKeyVariant)]
        [TestCase("z", KeyScheme.SingleLengthKey)]
        [TestCase("y", KeyScheme.TripleLengthKeyAnsi)]
        [TestCase("t", KeyScheme.TripleLengthKeyVariant)]
        public void TestToKeyScheme (string text, KeyScheme expected)
        {
            Assert.AreEqual(expected, text.GetKeyScheme());
        }

        [Test]
        [TestCase("01", new byte[] { 48, 49})]
        [TestCase("", new byte[] { })]
        public void TestStringToBytes(string text, byte[] expected)
        {
            Assert.AreEqual(expected, text.GetBytes());
        }

        [Test]
        [TestCase(new byte[] {48, 49}, "01")]
        [TestCase(new byte[] { }, "")]
        public void TestBytesToString (byte[] bytes, string expected)
        {
            Assert.AreEqual(expected, bytes.GetString());
        }

        [Test]
        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, "0123456789ABCDEF")]
        [TestCase(new byte[] { }, "")]
        public void TestBytesToHexString (byte[] bytes, string expected)
        {
            Assert.AreEqual(expected, bytes.GetHexString());
        }

        [Test]
        [TestCase("0123456789ABCDEF", new byte[] {0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF})]
        public void TestHexStringToBytes (string text, byte[] expected)
        {
            Assert.AreEqual(expected, text.GetHexBytes());
        }

        [Test]
        [TestCase("0000000000000000", "0123456789ABCDEF", "0123456789ABCDEF")]
        [TestCase("1111233345556777", "0123456789ABCDEF", "10326654CCFEAA98")]
        public void TestStringXor (string text1, string text2, string expected)
        {
            Assert.AreEqual(expected, text1.XorHex(text2));
            Assert.AreEqual(expected, text2.XorHex(text1));
        }

        [Test]
        [TestCase("FF", "11111111")]
        [TestCase("0123", "0000000100100011")]
        public void TestToBinary (string text, string expected)
        {
            Assert.AreEqual(expected, text.GetBinary());
        }

        [Test]
        [TestCase("11111111", "FF")]
        [TestCase("0000000100100011", "0123")]
        public void TestFromBinary (string text, string expected)
        {
            Assert.AreEqual(expected, text.FromBinary());
        }

        [Test]
        [TestCase("00", LmkPair.Pair04_05)]
        [TestCase("01", LmkPair.Pair06_07)]
        [TestCase("02", LmkPair.Pair14_15)]
        [TestCase("03", LmkPair.Pair16_17)]
        [TestCase("04", LmkPair.Pair18_19)]
        [TestCase("05", LmkPair.Pair20_21)]
        [TestCase("06", LmkPair.Pair22_23)]
        [TestCase("07", LmkPair.Pair24_25)]
        [TestCase("08", LmkPair.Pair26_27)]
        [TestCase("09", LmkPair.Pair28_29)]
        [TestCase("0A", LmkPair.Pair30_31)]
        [TestCase("0B", LmkPair.Pair32_33)]
        [TestCase("10", LmkPair.Pair04_05)]
        [TestCase("42", LmkPair.Pair14_15)]
        public void TestLmkPairMapping (string text, LmkPair expected)
        {
            Assert.AreEqual(expected, text.GetLmkPair());
        }
    }
}
