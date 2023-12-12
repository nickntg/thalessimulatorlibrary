using System;
using System.Globalization;
using System.Linq;
using ThalesSimulatorLibrary.Core.Cryptography.Key;

namespace ThalesSimulatorLibrary.Core.Utility
{
    public static class Extensions
    {
        public static bool IsHex(this string text)
        {
            return Int128.TryParse(text, NumberStyles.HexNumber, null, out _);
        }

        public static byte[] GetBytesOfHexString(this string text)
        {
            if (text.Length % 2 != 0)
            {
                throw new InvalidOperationException("Text length must be even");
            }

            if (!text.IsHex())
            {
                throw new InvalidOperationException("Text must be hexadecimal");
            }

            return Enumerable.Range(0, text.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(text.Substring(x, 2), 16))
                .ToArray();
        }

        public static string Xor(this string key1, string key2)
        {
            key1 = key1.StripKeySchemeTag();
            key2 = key2.StripKeySchemeTag();

            if (key1.Length != key2.Length)
            {
                throw new InvalidOperationException($"{key1} is not of equal length to {key2}");
            }

            var b1 = GetBytesOfHexString(key1);
            var b2 = GetBytesOfHexString(key2);

            for (var i = 0; i < b1.Length; i++)
            {
                b1[i] = (byte)(b1[i] ^ b2[i]);
            }

            return Convert.ToHexString(b1);
        }

        public static bool HasParity(this string key, Parity parity)
        {
            if (parity == Parity.None)
            {
                return true;
            }

            key = key.StripKeySchemeTag();

            if (!key.IsHex())
            {
                throw new InvalidOperationException($"Key {key} is not hexadecimal");
            }

            var bytes = key.GetBytesOfHexString();
            if (bytes.Any(b => !b.HasParity(parity)))
            {
                return false;
            }

            return true;
        }

        public static bool HasParity(this byte b, Parity parity)
        {
            if (parity == Parity.None)
            {
                return true;
            }

            var ones = Convert.ToString(b, 2).Replace("0", "");
            return (ones.Length % 2 != 0 || parity != Parity.Odd) && (ones.Length % 2 != 1 || parity != Parity.Even);
        }

        public static byte MakeParity(this byte b, Parity parity)
        {
            if (b.HasParity(parity))
            {
                return b;
            }

            return (byte)(b ^ 0x01);
        }
    }
}
