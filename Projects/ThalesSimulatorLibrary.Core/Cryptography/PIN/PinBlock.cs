
namespace ThalesSimulatorLibrary.Core.Cryptography.PIN
{
    public class PinBlock
    {
        public PinBlockFormat Format { get; set; }
        public string AccountOrPadding { get; set; }
        public string Pin { get; set; }

        public static PinBlock FromClearText(string pin, string accountOrPadding)
        {
            return new PinBlock
            {
                AccountOrPadding = accountOrPadding,
                Format = PinBlockFormat.Unspecified,
                Pin = pin
            };
        }

        public static PinBlock FromBlock(string pinBlock, string accountOrPadding, PinBlockFormat format)
        {
            return null;
        }
    }
}
