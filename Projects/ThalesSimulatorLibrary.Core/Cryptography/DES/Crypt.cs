using System.IO;
using System.Security.Cryptography;
using Ardalis.GuardClauses;
using NLog;
using ThalesSimulatorLibrary.Core.Cryptography.LMK;
using ThalesSimulatorLibrary.Core.Utility;

namespace ThalesSimulatorLibrary.Core.Cryptography.DES
{
    public static class Crypt
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string LeftPad = "0000000000000000";
        private const string RightPad = "00000000000000";

        public static byte[] DesEncrypt(this byte[] key, byte[] data)
        {
            return DesOperation(key, data, true);
        }

        public static byte[] DesDecrypt(this byte[] key, byte[] data)
        {
            return DesOperation(key, data, false);
        }

        public static string DesEncrypt(this string key, string data)
        {
            Guard.Against.NullOrEmpty(key, nameof(key), "Key must not be empty");
            Guard.Against.InvalidInput(key, nameof(key), s => s.Length is 16 or 32 or 48,
                "Invalid key length");

            if (key.Length == 16)
            {
                return DesEncrypt(key.GetBytesOfHexString(), data.GetBytesOfHexString()).GetHex();
            }

            if (key.Length == 32)
            {
                return DesEncrypt(key[..16], key[16..], key[..16], data);
            }

            return DesEncrypt(key[..16], key.Substring(16, 16), key[32..], data);
        }

        public static string DesDecrypt(this string key, string data)
        {
            Guard.Against.NullOrEmpty(key, nameof(key), "Key must not be empty");
            Guard.Against.InvalidInput(key, nameof(key), s => s.Length is 16 or 32 or 48,
                "Invalid key length");

            if (key.Length == 16)
            {
                return DesDecrypt(key.GetBytesOfHexString(), data.GetBytesOfHexString()).GetHex();
            }

            if (key.Length == 32)
            {
                return DesDecrypt(key[..16], key[16..], key[..16], data);
            }

            return DesDecrypt(key[..16], key.Substring(16, 16), key[32..], data);
        }

        public static byte[] DesEncrypt(byte[] key1, byte[] key2, byte[] key3, byte[] data)
        {
            var r1 = key1.DesEncrypt(data);
            var r2 = key2.DesDecrypt(r1);
            return key3.DesEncrypt(r2);
        }

        public static byte[] DesDecrypt(byte[] key1, byte[] key2, byte[] key3, byte[] data)
        {
            var r1 = key3.DesDecrypt(data);
            var r2 = key2.DesEncrypt(r1);
            return key1.DesDecrypt(r2);
        }

        public static string DesEncrypt(string key1, string key2, string key3, string data)
        {
            return DesEncrypt(key1.GetBytesOfHexString(), key2.GetBytesOfHexString(), key3.GetBytesOfHexString(),
                data.GetBytesOfHexString()).GetHex();
        }

        public static string DesDecrypt(string key1, string key2, string key3, string data)
        {
            return DesDecrypt(key1.GetBytesOfHexString(), key2.GetBytesOfHexString(), key3.GetBytesOfHexString(),
                data.GetBytesOfHexString()).GetHex();
        }

        public static string DesEncryptVariant(LmkPair pair, string variant, string data)
        {
            Guard.Against.NullOrEmpty(data, nameof(data), "Data should not be empty");
            Guard.Against.InvalidInput(data, nameof(data), s => s.IsHex() && s.Length is 32 or 48,
                "Data must be 32 or 48 hex digits long");

            var lmk = Storage.Lmk(pair, variant);

            if (data.Length == 32)
            {
                var lmkFirst =
                    lmk.Xor($"{LeftPad}{LmkHexVariants.GetDoubleLengthVariant(1)}{RightPad}");
                var r1 = DesEncrypt(lmkFirst, data[..16]);
                var lmkSecond =
                    lmk.Xor($"{LeftPad}{LmkHexVariants.GetDoubleLengthVariant(2)}{RightPad}");
                var r2 = DesEncrypt(lmkSecond, data[16..]);
                return $"{r1}{r2}";
            }
            else
            {
                var lmkFirst = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(1)}{RightPad}");
                var r1 = DesEncrypt(lmkFirst, data[..16]);
                var lmkSecond = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(2)}{RightPad}");
                var r2 = DesEncrypt(lmkSecond, data.Substring(16, 16));
                var lmkThird = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(3)}{RightPad}");
                var r3 = DesEncrypt(lmkThird, data[32..]);
                return $"{r1}{r2}{r3}";
            }
        }

        public static string DesDecryptVariant(LmkPair pair, string variant, string data)
        {
            Guard.Against.NullOrEmpty(data, nameof(data), "Data should not be empty");
            Guard.Against.InvalidInput(data, nameof(data), s => s.IsHex() && s.Length is 32 or 48,
                "Data must be 32 or 48 hex digits long");

            var lmk = Storage.Lmk(pair, variant);

            if (data.Length == 32)
            {
                var lmkFirst =
                    lmk.Xor($"{LeftPad}{LmkHexVariants.GetDoubleLengthVariant(1)}{RightPad}");
                var r1 = DesDecrypt(lmkFirst, data[..16]);
                var lmkSecond =
                    lmk.Xor($"{LeftPad}{LmkHexVariants.GetDoubleLengthVariant(2)}{RightPad}");
                var r2 = DesDecrypt(lmkSecond, data[16..]);
                return $"{r1}{r2}";
            }
            else
            {
                var lmkFirst = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(1)}{RightPad}");
                var r1 = DesDecrypt(lmkFirst, data[..16]);
                var lmkSecond = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(2)}{RightPad}");
                var r2 = DesDecrypt(lmkSecond, data.Substring(16, 16));
                var lmkThird = lmk.Xor($"{LeftPad}{LmkHexVariants.GetTripleLengthVariant(3)}{RightPad}");
                var r3 = DesDecrypt(lmkThird, data[32..]);
                return $"{r1}{r2}{r3}";
            }
        }

        private static byte[] DesOperation(byte[] key, byte[] data, bool encrypt)
        {
            Guard.Against.Null(key, nameof(key), "Key cannot be null");
            Guard.Against.InvalidInput(key, nameof(key), bytes => bytes.Length == 8, "Key must be 8 bytes");
            Guard.Against.Null(data, nameof(data), "Data cannot be null");
            Guard.Against.InvalidInput(data, nameof(data), bytes => bytes.Length == 8, "Data must be 8 bytes");

            using (var desAlg = System.Security.Cryptography.DES.Create())
            {
                var nullVector = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                var result = new byte[8];

                if (System.Security.Cryptography.DES.IsWeakKey(key) || System.Security.Cryptography.DES.IsSemiWeakKey(key))
                {
                    Logger.Warn("***Weak or semi-weak key detected***");
                }

                desAlg.Mode = CipherMode.ECB;
                desAlg.IV = nullVector;
                desAlg.Padding = PaddingMode.None;

                var desTransform = encrypt ? desAlg.CreateEncryptor(key, nullVector) : desAlg.CreateDecryptor(key, nullVector);

                using (var outStream = new MemoryStream(result))
                {
                    using (var cryptoStream = new CryptoStream(outStream, desTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, 8);
                        cryptoStream.Close();
                    }
                }

                return result;
            }
        }
    }
}
