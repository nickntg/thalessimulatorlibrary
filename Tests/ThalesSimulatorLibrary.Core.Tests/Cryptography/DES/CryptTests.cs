using ThalesSimulatorLibrary.Core.Cryptography.DES;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.DES
{
    public class CryptTests
    {
        private const string Zeroes = "0000000000000000";

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
        }

        [Fact]
        public void TripleDesDecrypt()
        {
            Assert.Equal("C1E6E95D2166B5C4", Crypt.DesDecrypt("0123456789ABCDEF", "FEDCBA9876543210", "0123456789ABCDEF", Zeroes));
        }
    }
}
