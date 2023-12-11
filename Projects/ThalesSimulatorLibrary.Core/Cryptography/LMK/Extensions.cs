using System.Collections.Generic;
using ThalesSimulatorLibrary.Core.Exceptions;

namespace ThalesSimulatorLibrary.Core.Cryptography.LMK
{
    public static class Extensions
    {
        private static readonly Dictionary<string, LmkPair> LmkCodeMap = new()
        {
            { "00", LmkPair.Pair0405 },
            { "01", LmkPair.Pair0607 },
            { "02", LmkPair.Pair1415 },
            { "03", LmkPair.Pair1617 },
            { "04", LmkPair.Pair1819 },
            { "05", LmkPair.Pair2021 },
            { "06", LmkPair.Pair2223 },
            { "07", LmkPair.Pair2425 },
            { "08", LmkPair.Pair2627 },
            { "09", LmkPair.Pair2829 },
            { "0A", LmkPair.Pair3031 },
            { "0B", LmkPair.Pair3233 },
            { "0C", LmkPair.Pair3435 },
            { "0D", LmkPair.Pair3637 },
            { "0E", LmkPair.Pair3839 }
        };

        public static LmkPair GetLmkPairFromLmkCode(this string code)
        {
            if (code is not { Length: 2 } || !LmkCodeMap.TryGetValue(code, out LmkPair value))
            {
                throw new InvalidLmkCodeException($"Invalid LMK code {code}");
            }

            return value;
        }
    }
}
