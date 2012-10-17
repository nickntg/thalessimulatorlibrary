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

using System;
using NUnit.Framework;
using ThalesSim.Core.Cryptography.LMK;

namespace ThalesSim.Tests.Unit.Cryptography.LMK
{
    [TestFixture]
    public class LmkTests
    {
        [Test]
        [TestCase(1, 1, "A6")]
        [TestCase(1, 2, "5A")]
        [TestCase(1, 3, "6A")]
        [TestCase(1, 4, "DE")]
        [TestCase(1, 5, "2B")]
        [TestCase(1, 6, "50")]
        [TestCase(1, 7, "74")]
        [TestCase(1, 8, "9C")]
        [TestCase(1, 9, "FA")]
        [TestCase(2, 1, "A6")]
        [TestCase(2, 2, "5A")]
        [TestCase(3, 1, "6A")]
        [TestCase(3, 2, "DE")]
        [TestCase(3, 3, "2B")]
        public void VerifyVariant (int variantSize, int variantIndex, string expected)
        {
            switch (variantSize)
            {
                case 1:
                    Assert.AreEqual(expected, Variants.GetVariant(variantIndex));
                    break;
                case 2:
                    Assert.AreEqual(expected, Variants.GetDoubleLengthVariant(variantIndex));
                    break;
                case 3:
                    Assert.AreEqual(expected, Variants.GetTripleLengthVariant(variantIndex));
                    break;
                default:
                    throw new NotImplementedException("Invalid use of variant size");
            }
        }

        [Test]
        public void TestDefaultLmks()
        {
            LmkStorage.LmkStorageFile = "nofile.txt";
            LmkStorage.GenerateTestLmks(false);

            Assert.AreEqual("01010101010101017902CD1FD36EF8BA", LmkStorage.Lmk(LmkPair.Pair00_01));
            Assert.AreEqual("20202020202020203131313131313131", LmkStorage.Lmk(LmkPair.Pair02_03));
            Assert.AreEqual("40404040404040405151515151515151", LmkStorage.Lmk(LmkPair.Pair04_05));
            Assert.AreEqual("61616161616161617070707070707070", LmkStorage.Lmk(LmkPair.Pair06_07));
            Assert.AreEqual("80808080808080809191919191919191", LmkStorage.Lmk(LmkPair.Pair08_09));
            Assert.AreEqual("A1A1A1A1A1A1A1A1B0B0B0B0B0B0B0B0", LmkStorage.Lmk(LmkPair.Pair10_11));
            Assert.AreEqual("C1C1010101010101D0D0010101010101", LmkStorage.Lmk(LmkPair.Pair12_13));
            Assert.AreEqual("E0E0010101010101F1F1010101010101", LmkStorage.Lmk(LmkPair.Pair14_15));
            Assert.AreEqual("1C587F1C13924FEF0101010101010101", LmkStorage.Lmk(LmkPair.Pair16_17));
            Assert.AreEqual("01010101010101010101010101010101", LmkStorage.Lmk(LmkPair.Pair18_19));
            Assert.AreEqual("02020202020202020404040404040404", LmkStorage.Lmk(LmkPair.Pair20_21));
            Assert.AreEqual("07070707070707071010101010101010", LmkStorage.Lmk(LmkPair.Pair22_23));
            Assert.AreEqual("13131313131313131515151515151515", LmkStorage.Lmk(LmkPair.Pair24_25));
            Assert.AreEqual("16161616161616161919191919191919", LmkStorage.Lmk(LmkPair.Pair26_27));
            Assert.AreEqual("1A1A1A1A1A1A1A1A1C1C1C1C1C1C1C1C", LmkStorage.Lmk(LmkPair.Pair28_29));
            Assert.AreEqual("23232323232323232525252525252525", LmkStorage.Lmk(LmkPair.Pair30_31));
            Assert.AreEqual("26262626262626262929292929292929", LmkStorage.Lmk(LmkPair.Pair32_33));
            Assert.AreEqual("2A2A2A2A2A2A2A2A2C2C2C2C2C2C2C2C", LmkStorage.Lmk(LmkPair.Pair34_35));
            Assert.AreEqual("2F2F2F2F2F2F2F2F3131313131313131", LmkStorage.Lmk(LmkPair.Pair36_37));
            Assert.AreEqual("01010101010101010101010101010101", LmkStorage.Lmk(LmkPair.Pair38_39));

            Assert.IsTrue(LmkStorage.CheckLmkStorage());
        }

        [Test]
        public void TestDefaultOldLmks()
        {
            LmkStorage.LmkStorageFile = "nofile.txt";
            LmkStorage.GenerateTestLmks(false);

            LmkStorage.UseOldLmkStorage = true;

            Assert.AreEqual("101010101010101F7902CD1FD36EF8BA", LmkStorage.Lmk(LmkPair.Pair00_01));
            Assert.AreEqual("202020202020202F3131313131313131", LmkStorage.Lmk(LmkPair.Pair02_03));
            Assert.AreEqual("404040404040404F5151515151515151", LmkStorage.Lmk(LmkPair.Pair04_05));
            Assert.AreEqual("616161616161616E7070707070707070", LmkStorage.Lmk(LmkPair.Pair06_07));
            Assert.AreEqual("808080808080808F9191919191919191", LmkStorage.Lmk(LmkPair.Pair08_09));
            Assert.AreEqual("A1A1A1A1A1A1A1AEB0B0B0B0B0B0B0B0", LmkStorage.Lmk(LmkPair.Pair10_11));
            Assert.AreEqual("C1C101010101010ED0D0010101010101", LmkStorage.Lmk(LmkPair.Pair12_13));
            Assert.AreEqual("E0E001010101010EF1F1010101010101", LmkStorage.Lmk(LmkPair.Pair14_15));
            Assert.AreEqual("1C587F1C13924FFE0101010101010101", LmkStorage.Lmk(LmkPair.Pair16_17));
            Assert.AreEqual("010101010101010E0101010101010101", LmkStorage.Lmk(LmkPair.Pair18_19));
            Assert.AreEqual("020202020202020E0404040404040404", LmkStorage.Lmk(LmkPair.Pair20_21));
            Assert.AreEqual("070707070707070E1010101010101010", LmkStorage.Lmk(LmkPair.Pair22_23));
            Assert.AreEqual("131313131313131F1515151515151515", LmkStorage.Lmk(LmkPair.Pair24_25));
            Assert.AreEqual("161616161616161F1919191919191919", LmkStorage.Lmk(LmkPair.Pair26_27));
            Assert.AreEqual("1A1A1A1A1A1A1A1F1C1C1C1C1C1C1C1C", LmkStorage.Lmk(LmkPair.Pair28_29));
            Assert.AreEqual("232323232323232F2525252525252525", LmkStorage.Lmk(LmkPair.Pair30_31));
            Assert.AreEqual("262626262626262F2929292929292929", LmkStorage.Lmk(LmkPair.Pair32_33));
            Assert.AreEqual("2A2A2A2A2A2A2A2F2C2C2C2C2C2C2C2C", LmkStorage.Lmk(LmkPair.Pair34_35));
            Assert.AreEqual("2F2F2F2F2F2F2FFE3131313131313131", LmkStorage.Lmk(LmkPair.Pair36_37));
            Assert.AreEqual("010101010101010E0101010101010101", LmkStorage.Lmk(LmkPair.Pair38_39));

            Assert.IsTrue(LmkStorage.CheckLmkStorage());

            LmkStorage.UseOldLmkStorage = false;
        }
    }
}
