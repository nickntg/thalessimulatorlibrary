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
using ThalesSim.Core.Cryptography.MAC;
using ThalesSim.Core.Utility;

namespace ThalesSim.Tests.Unit.Cryptography.MAC
{
    [TestFixture]
    public class MacTests
    {
        [Test]
        public void X919MacTest()
        {
            var hk = new HexKey("838652DF68A246046DAB6104583B201A");
            const string data = "30303030303030303131313131313131";
            var iv = "0000000000000000";

            // Binary data MACing.
            var hexStr = ("00000000").GetBytes().GetHexString();
            Assert.AreEqual("3F431586CA33D99C", IsoX919Mac.MacHexData(hexStr, hk, iv, IsoX919BlockType.OnlyBlock));

            // Binary un-padded data MACing.
            hexStr = ("000000001").GetBytes().GetHexString();
            Assert.AreEqual("2FE7118F2074398E", IsoX919Mac.MacHexData(hexStr, hk, iv, IsoX919BlockType.OnlyBlock));

            // Successive hex data MACing.
            iv = IsoX919Mac.MacHexData(data, hk, iv, IsoX919BlockType.FirstBlock);
            Assert.AreEqual("A9D4D96683B51333", iv);
            iv = IsoX919Mac.MacHexData(data, hk, iv, IsoX919BlockType.NextBlock);
            Assert.AreEqual("DA46CEC9E61AF065", iv);
            iv = IsoX919Mac.MacHexData(data, hk, iv, IsoX919BlockType.NextBlock);
            Assert.AreEqual("56A27E35442BD07D", iv);
            iv = IsoX919Mac.MacHexData(data, hk, iv, IsoX919BlockType.NextBlock);
            Assert.AreEqual("B12874BED7137303", iv);
            iv = IsoX919Mac.MacHexData(data, hk, iv, IsoX919BlockType.FinalBlock);
            Assert.AreEqual("0D99127F7734AA58", iv);
        }
    }
}
