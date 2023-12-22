using ThalesSimulatorLibrary.Core.Cryptography.DES;
using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using ThalesSimulatorLibrary.Core.Tests.TestHelpers;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.DES
{
    public class CryptTests
    {
        private const string Zeroes = "0000000000000000";

        public CryptTests()
        {
            StorageHelpers.ReadLmks();
        }

        [Fact]
        public void DesEncrypt()
        {
            Assert.Equal("D5D44FF720683D0D", "0123456789ABCDEF".DesEncrypt(Zeroes));
        }

        [Fact]
        public void DesDecrypt()
        {
            Assert.Equal("14AAD7F4DBB4E094", "0123456789ABCDEF".DesDecrypt(Zeroes));
        }

        [Fact]
        public void TripleDesEncrypt()
        {
            Assert.Equal("08D7B4FB629D0885", Crypt.DesEncrypt("0123456789ABCDEF", "FEDCBA9876543210", "0123456789ABCDEF", Zeroes));
            Assert.Equal("08D7B4FB629D0885", "0123456789ABCDEFFEDCBA98765432100123456789ABCDEF".DesEncrypt(Zeroes));
            Assert.Equal("08D7B4FB629D0885", "0123456789ABCDEFFEDCBA9876543210".DesEncrypt(Zeroes));
        }

        [Fact]
        public void TripleDesDecrypt()
        {
            Assert.Equal("C1E6E95D2166B5C4", Crypt.DesDecrypt("0123456789ABCDEF", "FEDCBA9876543210", "0123456789ABCDEF", Zeroes));
            Assert.Equal("C1E6E95D2166B5C4", "0123456789ABCDEFFEDCBA98765432100123456789ABCDEF".DesDecrypt(Zeroes));
            Assert.Equal("C1E6E95D2166B5C4", "0123456789ABCDEFFEDCBA9876543210".DesDecrypt(Zeroes));
        }

        [Theory]
        [InlineData("")]
        [InlineData("0123456789ABCDE")]
        [InlineData("0123456789ABCDEF0123456789ABCDE")]
        [InlineData("0123456789ABCDEF0123456789ABCDEF0123456789ABCDE")]
        public void DesEncryptIncorrectKeySize(string key)
        {
            Assert.Throws<ArgumentException>(() => key.DesEncrypt("0123456789ABCDEF"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("0123456789ABCDE")]
        [InlineData("0123456789ABCDEF0123456789ABCDE")]
        [InlineData("0123456789ABCDEF0123456789ABCDEF0123456789ABCDE")]
        public void DesDecryptIncorrectKeySize(string key)
        {
            Assert.Throws<ArgumentException>(() => key.DesDecrypt("0123456789ABCDEF"));
        }

        [Theory]
        [InlineData(LmkPair.Pair2829, "2", "F1F1F1F1F1F1F1F1C1C1C1C1C1C1C1C1", "5178C9D3D1052B15BF6AEC458B4A4564")]
        public void EncryptWithLmkVariant(LmkPair pair, string variant, string data, string expected)
        {
            Assert.Equal(expected, Crypt.DesEncryptVariant(pair, variant, data));
        }

        [Theory]
        [InlineData(LmkPair.Pair2829, "2", "5178C9D3D1052B15BF6AEC458B4A4564", "F1F1F1F1F1F1F1F1C1C1C1C1C1C1C1C1")]
        public void DecryptWithLmkVariant(LmkPair pair, string variant, string data, string expected)
        {
            Assert.Equal(expected, Crypt.DesDecryptVariant(pair, variant, data));
        }
    }
}
