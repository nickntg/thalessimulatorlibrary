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

namespace ThalesSim.Tests.Unit.Cryptography
{
    [TestFixture]
    public class KeyTests
    {
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
    }
}
