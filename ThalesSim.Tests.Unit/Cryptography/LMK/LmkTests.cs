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
    }
}
