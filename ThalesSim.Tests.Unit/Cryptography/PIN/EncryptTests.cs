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
using ThalesSim.Core.Cryptography.PIN;

namespace ThalesSim.Tests.Unit.Cryptography.PIN
{
    [TestFixture]
    public class EncryptTests
    {
        [Test]
        public void TestPinEncrypt()
        {
            Assert.AreEqual("01234", Encrypt.EncryptPinForHostStorage("1234"));
            Assert.AreEqual("1234", Encrypt.DecryptPinUnderHostStorage("01234"));
        }

        [Test]
        public void TestPinEncryptThales()
        {
            Assert.AreEqual("01234", Encrypt.EncryptPinForHostStorageThales("1234"));
            Assert.AreEqual("1234", Encrypt.DecryptPinUnderHostStorageThales("01234"));            
        }
    }
}
