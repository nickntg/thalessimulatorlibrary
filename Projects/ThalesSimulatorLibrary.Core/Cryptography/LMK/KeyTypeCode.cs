using ThalesSimulatorLibrary.Core.Exceptions;

namespace ThalesSimulatorLibrary.Core.Cryptography.LMK
{
    public class KeyTypeCode
    {
        public LmkPair Lmk { get; set; }
        public string Variant { get; set; }

        public KeyTypeCode(string keyTypeCode)
        {
            if (keyTypeCode is not { Length: 3 })
            {
                throw new InvalidKeyTypeCodeException($"Invalid key type code {keyTypeCode}");
            }

            Init(keyTypeCode[..1], keyTypeCode.Substring(1, 2).GetLmkPairFromLmkCode());
        }

        public KeyTypeCode(string variant, LmkPair lmkPair)
        {
            Init(variant, lmkPair);
        }

        private void Init(string variant, LmkPair lmkPair)
        {
            if (variant is not { Length: 1 } || !char.IsDigit(variant.ToCharArray()[0]))
            {
                throw new InvalidVariantException($"Invalid variant {variant}");
            }

            Lmk = lmkPair;
            Variant = variant;
        }
    }
}
