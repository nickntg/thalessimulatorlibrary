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
    public class KeyTypeCodeTests
    {
        [Test]
        [TestCase("000", LmkPair.Pair04_05, 0)]
        [TestCase("100", LmkPair.Pair04_05, 1)]
        [TestCase("200", LmkPair.Pair04_05, 2)]
        [TestCase("300", LmkPair.Pair04_05, 3)]
        [TestCase("400", LmkPair.Pair04_05, 4)]
        [TestCase("001", LmkPair.Pair06_07, 0)]
        [TestCase("002", LmkPair.Pair14_15, 0)]
        [TestCase("102", LmkPair.Pair14_15, 1)]
        [TestCase("202", LmkPair.Pair14_15, 2)]
        [TestCase("402", LmkPair.Pair14_15, 4)]
        [TestCase("602", LmkPair.Pair14_15, 6)]
        [TestCase("003", LmkPair.Pair16_17, 0)]
        [TestCase("006", LmkPair.Pair22_23, 0)]
        [TestCase("008", LmkPair.Pair26_27, 0)]
        [TestCase("108", LmkPair.Pair26_27, 1)]
        [TestCase("208", LmkPair.Pair26_27, 2)]
        [TestCase("009", LmkPair.Pair28_29, 0)]
        [TestCase("109", LmkPair.Pair28_29, 1)]
        [TestCase("209", LmkPair.Pair28_29, 2)]
        [TestCase("309", LmkPair.Pair28_29, 3)]
        [TestCase("409", LmkPair.Pair28_29, 4)]
        [TestCase("509", LmkPair.Pair28_29, 5)]
        [TestCase("709", LmkPair.Pair28_29, 7)]
        [TestCase("00A", LmkPair.Pair30_31, 0)]
        [TestCase("10A", LmkPair.Pair30_31, 1)]
        [TestCase("20A", LmkPair.Pair30_31, 2)]
        [TestCase("00B", LmkPair.Pair32_33, 0)]
        [TestCase("10B", LmkPair.Pair32_33, 1)]
        [TestCase("20B", LmkPair.Pair32_33, 2)]
        [TestCase("00C", LmkPair.Pair34_35, 0)]
        [TestCase("10C", LmkPair.Pair34_35, 1)]
        [TestCase("00D", LmkPair.Pair36_37, 0)]
        public void TestKeyTypeCode (string keyTypeCode, LmkPair expectedPair, int expectedVariant)
        {
            var ktc = new KeyTypeCode(keyTypeCode);
            Assert.AreEqual(expectedPair, ktc.Pair);
            Assert.AreEqual(expectedVariant, ktc.Variant);
            Assert.AreEqual(ktc.ToString(), keyTypeCode);
        }
    }
}
