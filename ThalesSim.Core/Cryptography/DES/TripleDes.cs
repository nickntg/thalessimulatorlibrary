/*
 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/ 

using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using ThalesSim.Core.Utility;
using log4net;

namespace ThalesSim.Core.Cryptography.DES
{
    public class TripleDes
    {
        public static byte[] DesEncrypt (byte[] key, byte[] data)
        {
            var log = LogManager.GetLogger("DesEncrypt");

            try
            {
                return DesOperation(key, data, true, log);
            }
            catch (Exception ex)
            {
                log.Error("DESEncrypt error", ex);
                throw;
            }
        }

        public static byte[] DesDecrypt(byte[] key, byte[] data)
        {
            var log = LogManager.GetLogger("DesDecrypt");

            try
            {
                return DesOperation(key, data, false, log);
            }
            catch (Exception ex)
            {
                log.Error("DESEncrypt error", ex);
                throw;
            }
        }

        public static string DesEncrypt (string key, string data)
        {
            return DesEncrypt(key.GetHexBytes(), data.GetHexBytes()).GetHexString();
        }

        public static string DesDecrypt (string key, string data)
        {
            return DesDecrypt(key.GetHexBytes(), data.GetHexBytes()).GetHexString();
        }

        public static byte[] TripleDesEncrypt (byte[] key1, byte[] key2, byte[] key3, byte[] data)
        {
            var result = DesEncrypt(key1, data);
            var result2 = DesDecrypt(key2, result);
            return DesEncrypt(key3, result2);
        }

        public static byte[] TripleDesDecrypt(byte[] key1, byte[] key2, byte[] key3, byte[] data)
        {
            var result = DesDecrypt(key3, data);
            result = DesEncrypt(key2, result);
            return DesDecrypt(key1, result);
        }

        public static string TripleDesEncrypt (string key1, string key2, string key3, string data)
        {
            return TripleDesEncrypt(key1.GetHexBytes(), key2.GetHexBytes(), key3.GetHexBytes(), data.GetHexBytes()).GetHexString();
        }

        public static string TripleDesDecrypt(string key1, string key2, string key3, string data)
        {
            return TripleDesDecrypt(key1.GetHexBytes(), key2.GetHexBytes(), key3.GetHexBytes(), data.GetHexBytes()).GetHexString();
        }

        private static byte[] DesOperation (byte[] key, byte[] data, bool encrypt, ILog log)
        {
            if (key == null || data == null)
            {
                throw new InvalidOperationException("Key or data cannot be null");
            }

            if (key.Length != 8 || data.Length != 8)
            {
                throw new InvalidOperationException("Key or data not eight bytes");
            }

            using (var desAlg = System.Security.Cryptography.DES.Create())
            {
                var nullVector = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                var result = new byte[8];

                if (System.Security.Cryptography.DES.IsWeakKey(key) || System.Security.Cryptography.DES.IsSemiWeakKey(key))
                {
                    log.Warn(encrypt
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
                    log.Warn("Transform invocation error", ex);
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
