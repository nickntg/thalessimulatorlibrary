using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using ThalesSimulatorLibrary.Core.Utility;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.LMK
{
    public class StorageTests
    {
        [Fact]
        public void VerifyDefaults()
        {
            File.Delete("lmk.txt");
            Storage.ReadLmks("lmk.txt");

            Assert.Equal("01010101010101017902CD1FD36EF8BA", Storage.Lmk(LmkPair.Pair0001));
            Assert.Equal("20202020202020203131313131313131", Storage.Lmk(LmkPair.Pair0203));
            Assert.Equal("40404040404040405151515151515151", Storage.Lmk(LmkPair.Pair0405));
            Assert.Equal("61616161616161617070707070707070", Storage.Lmk(LmkPair.Pair0607));
            Assert.Equal("80808080808080809191919191919191", Storage.Lmk(LmkPair.Pair0809));
            Assert.Equal("A1A1A1A1A1A1A1A1B0B0B0B0B0B0B0B0", Storage.Lmk(LmkPair.Pair1011));
            Assert.Equal("C1C1010101010101D0D0010101010101", Storage.Lmk(LmkPair.Pair1213));
            Assert.Equal("E0E0010101010101F1F1010101010101", Storage.Lmk(LmkPair.Pair1415));
            Assert.Equal("1C587F1C13924FEF0101010101010101", Storage.Lmk(LmkPair.Pair1617));
            Assert.Equal("01010101010101010101010101010101", Storage.Lmk(LmkPair.Pair1819));
            Assert.Equal("02020202020202020404040404040404", Storage.Lmk(LmkPair.Pair2021));
            Assert.Equal("07070707070707071010101010101010", Storage.Lmk(LmkPair.Pair2223));
            Assert.Equal("13131313131313131515151515151515", Storage.Lmk(LmkPair.Pair2425));
            Assert.Equal("16161616161616161919191919191919", Storage.Lmk(LmkPair.Pair2627));
            Assert.Equal("1A1A1A1A1A1A1A1A1C1C1C1C1C1C1C1C", Storage.Lmk(LmkPair.Pair2829));
            Assert.Equal("23232323232323232525252525252525", Storage.Lmk(LmkPair.Pair3031));
            Assert.Equal("26262626262626262929292929292929", Storage.Lmk(LmkPair.Pair3233));
            Assert.Equal("2A2A2A2A2A2A2A2A2C2C2C2C2C2C2C2C", Storage.Lmk(LmkPair.Pair3435));
            Assert.Equal("2F2F2F2F2F2F2F2F3131313131313131", Storage.Lmk(LmkPair.Pair3637));
            Assert.Equal("01010101010101010101010101010101", Storage.Lmk(LmkPair.Pair3839));

            for (var i = 1; i <= 9; i++)
            {
                var hexVariant = LmkHexVariants.GetVariant(i);
                var hexVariantKey = hexVariant.PadRight(32, '0');

                for (var pair = LmkPair.Pair0001; pair <= LmkPair.Pair3839; pair++)
                {
                    Assert.Equal(Storage.Lmk(pair).Xor(hexVariantKey), Storage.Lmk(pair, i.ToString()));
                }
            }
        }

        [Fact]
        public void CheckLmkStorage()
        {
            Storage.ReadLmks("lmk.txt");
            Assert.True(Storage.CheckLmkStorage());
        }

        [Fact]
        public void AutomaticallyCreateNewLmkSet()
        {
            File.Delete("lmk.txt.1");
            Storage.ReadLmks("lmk.txt");

            Storage.Lmk(LmkPair.Pair0001, 1);

            Assert.True(File.Exists("lmk.txt.1"));
            Assert.True(Storage.CheckLmkStorage());
        }
    }
}
