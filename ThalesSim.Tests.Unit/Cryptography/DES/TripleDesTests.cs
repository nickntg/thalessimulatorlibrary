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
using ThalesSim.Core.Cryptography.DES;

namespace ThalesSim.Tests.Unit.Cryptography.DES
{
    [TestFixture]
    public class TripleDesTests
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        [Test]
        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF }, 
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            new byte[] {0xD5, 0xD4, 0x4F, 0xF7, 0x20, 0x68, 0x3D, 0x0D})]
        [TestCase(new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 }, 
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 }, 
            new byte[] { 0x8c, 0xa6, 0x4d, 0xe9, 0xc1, 0xb1, 0x23, 0xa7 })]
        public void TestByteDes (byte[] key, byte[] data, byte[] expected)
        {
            Assert.AreEqual(expected, TripleDes.DesEncrypt(key, data));
            Assert.AreEqual(data, TripleDes.DesDecrypt(key, expected));
        }

        [Test]
        [TestCase(new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF },
            new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF },
            new byte[] { 0x01, 0x23, 0x45, 0x67, 0x89, 0xAB, 0xCD, 0xEF },
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new byte[] { 0xD5, 0xD4, 0x4F, 0xF7, 0x20, 0x68, 0x3D, 0x0D })]
        [TestCase(new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 },
            new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 },
            new byte[] { 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01 },
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new byte[] { 0x8c, 0xa6, 0x4d, 0xe9, 0xc1, 0xb1, 0x23, 0xa7 })]
        [TestCase(new byte[] { 0x62, 0xF8, 0xB3, 0xE9, 0xB3, 0xB9, 0x86, 0x76 },
            new byte[] { 0xEC, 0x4C, 0x23, 0x3B, 0xA2, 0xE0, 0x5B, 0xD9 },
            new byte[] { 0x38, 0x64, 0x51, 0x3B, 0x1F, 0xE5, 0xF8, 0xE0 },
            new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 },
            new byte[] { 0xF8, 0x9F, 0x30, 0xDB, 0xDF, 0x6A, 0x88, 0xDE })]
        public void TestTripleDes (byte[] key1, byte[] key2, byte[] key3, byte[] data, byte[] expected)
        {
            Assert.AreEqual(expected, TripleDes.TripleDesEncrypt(key1, key2, key3, data));
            Assert.AreEqual(data, TripleDes.TripleDesDecrypt(key1, key2, key3, expected));
        }

        [Test]
        [TestCase("401A1A1A1A1A1A1A1C1C1C1C1C1C1C1C", "F1F1F1F1F1F1F1F1C1C1C1C1C1C1C1C1", "5178C9D3D1052B15BF6AEC458B4A4564")]
        public void TestTripleDesVariant(string key, string data, string expected)
        {
            Assert.AreEqual(expected, new HexKey(key).EncryptVariant(data));
            Assert.AreEqual(data, new HexKey(key).DecryptVariant(expected));
        }
    }
}
