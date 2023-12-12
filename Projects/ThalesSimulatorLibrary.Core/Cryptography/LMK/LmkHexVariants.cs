namespace ThalesSimulatorLibrary.Core.Cryptography.LMK
{
    public class LmkHexVariants
    {
        private static readonly string[] SingleLengthVariants = new[] { "A6", "5A", "6A", "DE", "2B", "50", "74", "9C", "FA" };
        private static readonly string[] DoubleLengthVariants = new[] { "A6", "5A" };
        private static readonly string[] TripleLengthVariants = new[] { "6A", "DE", "2B" };

        public static string GetVariant(int index)
        {
            return SingleLengthVariants[index - 1];
        }

        public static string GetDoubleLengthVariant(int index)
        {
            return DoubleLengthVariants[index - 1];
        }

        public static string GetTripleLengthVariant(int index)
        {
            return TripleLengthVariants[index - 1];
        }
    }
}
