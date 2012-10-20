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
using ThalesSim.Core.Message;
using ThalesSim.Core.Utility;

namespace ThalesSim.Tests.Unit.Message
{
    [TestFixture]
    public class StreamMessageTests
    {
        [Test]
        public void TestBasicOperations()
        {
            var str = new StreamMessage("1234567890");

            Assert.AreEqual(10, str.CharsLeft);
            Assert.AreEqual("1234", str.Substring(4));
            Assert.AreEqual("1234567890", str.GetRemainingBytes().GetString());
            Assert.IsNullOrEmpty(str.GetTrailer());

            str.Index++;

            Assert.AreEqual(9, str.CharsLeft);
            Assert.AreEqual("2345", str.Substring(4));
            Assert.AreEqual("234567890", str.GetRemainingBytes().GetString());

            str.Index += 5;

            Assert.AreEqual(4, str.CharsLeft);
            Assert.AreEqual("7890", str.Substring(4));
            Assert.AreEqual("7890", str.GetRemainingBytes().GetString());

            str.Index += 4;

            Assert.AreEqual(0, str.CharsLeft);
        }

        [Test]
        public void TestTrailer()
        {
            var str = new StreamMessage("1234" + new byte[] {0x19}.GetString() + "trailer");

            var trailer = str.GetTrailer();

            Assert.AreEqual(new byte[] {0x19}.GetString() + "trailer", trailer);
            Assert.AreEqual(4, str.CharsLeft);
            Assert.AreEqual("1234", str.Substring(4));
        }
    }
}
