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

namespace ThalesSim.Tests.Unit.Cryptography
{
    [TestFixture]
    public class KeyTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            LmkStorage.LmkStorageFile = "nofile.txt";
            LmkStorage.GenerateTestLmks(false);
        }

        [Test]
        [TestCase("0123456789Abcdef", "0123456789ABCDEF", KeyLength.SingleLength, KeyScheme.SingleLengthKey)]
        [TestCase("Z0123456789Abcdef", "0123456789ABCDEF", KeyLength.SingleLength, KeyScheme.SingleLengthKey)]
        [TestCase("0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF", KeyLength.DoubleLength, KeyScheme.DoubleLengthKeyAnsi)]
        [TestCase("U0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF", KeyLength.DoubleLength, KeyScheme.DoubleLengthKeyVariant)]
        [TestCase("X0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF", KeyLength.DoubleLength, KeyScheme.DoubleLengthKeyAnsi)]
        [TestCase("0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", KeyLength.TripleLength, KeyScheme.TripleLengthKeyAnsi)]
        [TestCase("T0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", KeyLength.TripleLength, KeyScheme.TripleLengthKeyVariant)]
        [TestCase("Y0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", "0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF", KeyLength.TripleLength, KeyScheme.TripleLengthKeyAnsi)]
        public void VerifyHexKeyCreation (string hexKey, string expectedKey, KeyLength expectedLength, KeyScheme expectedScheme)
        {
            var key = new HexKey(hexKey);
            Assert.AreEqual(expectedKey, key.Key);
            Assert.AreEqual(expectedLength, key.Length);
            Assert.AreEqual(expectedScheme, key.Scheme);
        }

        [Test]
        [TestCase("BAB32D775A38E4AB", "001", "13F10E5D868A529E")]
        [TestCase("0406FBB23A5214DF", "002", "0B681FC20E62C734")]
        [TestCase("U8E2584D51A758A044E1F10BC91E50297", "000", "UF1A18CB66180F1D9C4EA40315DB6E3FD")]
        [TestCase("X2EC8A0412B5D0E86E3C1E5ABFA19B3F5", "000", "XE0C25DDAD6FEBC325D02F2F8EAE9266B")]
        [TestCase("XFF43378ED5D85B1BC465BF000335FBF1", "000", "X570E3115102C9D6B70EA920B134AE594")]
        [TestCase("U8463435FC4B4DAA0C49025272C29B12C", "002", "UD3DCC7EA9BCB755D254620B376B3D007")]
        [TestCase("TA5E7D4FE829B0D83C5E7352636C16C7827E197349E34A5CD", "001", "T0DF8F7E6D373863729E6451FA8D0981FCE79EA200829E09B")]
        public void VerifyThalesKey (string thalesKey, string keyTypeCode, string expectedKey)
        {
            Assert.AreEqual(expectedKey, new HexKeyThales(keyTypeCode, thalesKey).ClearKey);
        }
    }
}
