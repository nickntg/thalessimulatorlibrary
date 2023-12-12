using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using Xunit;

namespace ThalesSimulatorLibrary.Core.Tests.Cryptography.LMK
{
    public class LmkHexVariantTests
    {
        [Theory]
        [InlineData(1, 1, "A6")]
        [InlineData(1, 2, "5A")]
        [InlineData(1, 3, "6A")]
        [InlineData(1, 4, "DE")]
        [InlineData(1, 5, "2B")]
        [InlineData(1, 6, "50")]
        [InlineData(1, 7, "74")]
        [InlineData(1, 8, "9C")]
        [InlineData(1, 9, "FA")]
        [InlineData(2, 1, "A6")]
        [InlineData(2, 2, "5A")]
        [InlineData(3, 1, "6A")]
        [InlineData(3, 2, "DE")]
        [InlineData(3, 3, "2B")]
        public void VerifyVariant(int variantSize, int variantIndex, string expected)
        {
            switch (variantSize)
            {
                case 1:
                    Assert.Equal(expected, LmkHexVariants.GetVariant(variantIndex));
                    break;
                case 2:
                    Assert.Equal(expected, LmkHexVariants.GetDoubleLengthVariant(variantIndex));
                    break;
                case 3:
                    Assert.Equal(expected, LmkHexVariants.GetTripleLengthVariant(variantIndex));
                    break;
                default:
                    throw new NotImplementedException("Invalid use of variant size");
            }
        }
    }
}
