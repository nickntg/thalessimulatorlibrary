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
using ThalesSim.Core.Cryptography.PIN;

namespace ThalesSim.Tests.Unit.Cryptography.PIN
{
    [TestFixture]
    public class PinBlockTests
    {
        [Test]
        public void TestPinBlockCreation()
        {
            var pb1 = new PinBlock("1234", "550000025321", PinBlockFormat.AnsiX98);
        }

        [Test]
        public void AnsiX98PinBlock()
        {
            var pb1 = new PinBlock("1234", "550000025321", PinBlockFormat.AnsiX98);
            Assert.AreEqual("041261FFFFFDACDE", pb1.ClearPinBlock);

            var pb2 = new PinBlock("CAE9C83F58DDC12D", "550000025321", PinBlockFormat.AnsiX98,
                                   new HexKey("0123456789ABCDEFABCDEF0123456789"));
            Assert.AreEqual("1234", pb2.Pin);
        }

        [Test]
        public void DieboldPinBlock()
        {
            var pb1 = new PinBlock("1234", "550000025321", PinBlockFormat.Diebold);
            Assert.AreEqual("1234FFFFFFFFFFFF", pb1.ClearPinBlock);

            var pb2 = new PinBlock("4DBA042DD998BE4D", "550000025321", PinBlockFormat.Diebold,
                                   new HexKey("0123456789ABCDEFABCDEF0123456789"));
            Assert.AreEqual("1234", pb2.Pin);            
        }

        [Test]
        [TestCase("1234", "550000025321", PinBlockFormat.AnsiX98, PinBlockFormat.Diebold, "1234FFFFFFFFFFFF")]
        [TestCase("1234", "550000025321", PinBlockFormat.Diebold, PinBlockFormat.AnsiX98, "041261FFFFFDACDE")]
        public void TranslateClearPinBlock (string pin, string accountOrPadding, PinBlockFormat sourceFormat, PinBlockFormat targetFormat, string expected)
        {
            var pb = new PinBlock(pin, accountOrPadding, sourceFormat);
            Assert.AreEqual(expected, pb.Translate(targetFormat));
        }

        [Test]
        [TestCase("CAE9C83F58DDC12D", "550000025321", PinBlockFormat.AnsiX98, PinBlockFormat.AnsiX98, "0123456789ABCDEFABCDEF0123456789", "ABCDEF09876543210123456789ABCDEF", "5BFC613E71F15274")]
        [TestCase("CAE9C83F58DDC12D", "550000025321", PinBlockFormat.AnsiX98, PinBlockFormat.Diebold, "0123456789ABCDEFABCDEF0123456789", "ABCDEF09876543210123456789ABCDEF", "309E6BC5657F19EE")]
        public void TranslateEncryptedPinBlock (string pinBlock, string accountOrPadding, 
            PinBlockFormat sourceFormat, PinBlockFormat targetFormat, 
            string sourceClearKey, string targetClearKey, string expected)
        {
            var pb = new PinBlock(pinBlock, accountOrPadding, sourceFormat, new HexKey(sourceClearKey));
            var key = new HexKey(targetClearKey);
            Assert.AreEqual(expected, pb.Translate(key, targetFormat));
        }
    }
}
