using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using Ardalis.GuardClauses;
using NLog;
using ThalesSimulatorLibrary.Core.Utility;

namespace ThalesSimulatorLibrary.Core.Cryptography.DES
{
    public static class Crypt
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

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
            return DesEncrypt(key.GetBytesOfHexString(), data.GetBytesOfHexString()).GetHex();
        }

        public static string DesDecrypt(this string key, string data)
        {
            return DesDecrypt(key.GetBytesOfHexString(), data.GetBytesOfHexString()).GetHex();
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
                    Logger.Warn(encrypt
                        ? "***DES encrypt with weak or semi-weak key"
                        : "***DES decrypt with weak or semi-weak key");
                }

                desAlg.Mode = CipherMode.ECB;
                desAlg.IV = nullVector;
                desAlg.Padding = PaddingMode.None;

                ICryptoTransform desTransform;

                var mi = desAlg.GetType().GetMethod("_NewEncryptor", BindingFlags.NonPublic | BindingFlags.Instance);
                try
                {
                    desTransform = (ICryptoTransform)mi.Invoke(desAlg, new object[] { key, desAlg.Mode, nullVector, desAlg.FeedbackSize, encrypt ? 0 : 1 });
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex, "Transform invocation error");
                    desTransform = encrypt ? desAlg.CreateEncryptor(key, nullVector) : desAlg.CreateDecryptor(key, nullVector);
                }

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
