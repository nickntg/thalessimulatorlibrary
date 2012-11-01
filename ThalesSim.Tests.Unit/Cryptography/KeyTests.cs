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
            Assert.AreEqual(key.ToString().StripKeyScheme(), hexKey.ToUpper().StripKeyScheme());
        }

        [Test]
        [TestCase("BAB32D775A38E4AB", "001", "13F10E5D868A529E")]
        [TestCase("0406FBB23A5214DF", "002", "0B681FC20E62C734")]
        [TestCase("U8E2584D51A758A044E1F10BC91E50297", "000", "F1A18CB66180F1D9C4EA40315DB6E3FD")]
        [TestCase("X2EC8A0412B5D0E86E3C1E5ABFA19B3F5", "000", "E0C25DDAD6FEBC325D02F2F8EAE9266B")]
        [TestCase("XFF43378ED5D85B1BC465BF000335FBF1", "000", "570E3115102C9D6B70EA920B134AE594")]
        [TestCase("U8463435FC4B4DAA0C49025272C29B12C", "002", "D3DCC7EA9BCB755D254620B376B3D007")]
        [TestCase("TA5E7D4FE829B0D83C5E7352636C16C7827E197349E34A5CD", "001", "0DF8F7E6D373863729E6451FA8D0981FCE79EA200829E09B")]
        public void VerifyThalesKey (string thalesKey, string keyTypeCode, string expectedKey)
        {
            var key = new HexKeyThales(keyTypeCode, false, thalesKey);

            Assert.AreEqual(expectedKey, key.ClearKey);
            Assert.AreEqual(key.ClearKey, key.ClearHexKey.Key);

            if (key.Key.StartsWithKeyScheme())
            {
                var scheme = key.Key.GetKeyScheme();
                
                var otherKey = new HexKeyThales(keyTypeCode, true, key.ClearKey);

                if (scheme == KeyScheme.DoubleLengthKeyAnsi || scheme == KeyScheme.TripleLengthKeyAnsi)
                {
                    Assert.AreEqual(otherKey.KeyAnsi, thalesKey);
                }
                else
                {
                    Assert.AreEqual(otherKey.KeyVariant, thalesKey);
                }
            }
        }

        [Test]
        [TestCase("4ED06495741C280C", "002", "9D788F36127AD985")]
        [TestCase("A5B0D60EF61D60AD", "000", "9D788F36127AD985")]
        [TestCase("F229C2996B3872F7", "402", "9D788F36127AD985")]
        [TestCase("CB33B838319BFEDCD421CCFF7FB7AB8E", "000", "9B0D3ADA2489EB32")]
        [TestCase("Y39B751FB6ED3B9D18358F23EA1F838528438122DE25B0D0C", "002", "2B1CA95E644DBA30")]
        [TestCase("39B751FB6ED3B9D18358F23EA1F838528438122DE25B0D0C", "002", "2B1CA95E644DBA30")]
        [TestCase("T2B77A88FD21C461A1FBFFCF33D8EA327260828236700C6BF", "002", "2B1CA95E644DBA30")]
        public void TestThalesKeyCheckValue (string thalesKey, string keyTypeCode, string expectedKcv)
        {
            var key = new HexKeyThales(keyTypeCode, false, thalesKey);
            Assert.AreEqual(expectedKcv, key.CheckValue);
        }
    }
}
