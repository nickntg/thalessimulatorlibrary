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
using ThalesSim.Core.Cryptography.DES;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography
{
    /// <summary>
    /// This class represents a hexadecimal key used to encrypt/decrypt data.
    /// </summary>
    public class HexKey
    {
        /// <summary>
        /// Get/set the key scheme.
        /// </summary>
        public KeyScheme Scheme { get; private set; }

        /// <summary>
        /// Get/set the key length.
        /// </summary>
        public KeyLength Length { get; private set; }

        /// <summary>
        /// Get the represented by this instance.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Get the first key part.
        /// </summary>
        public string PartA
        {
            get { return Key.Substring(0, 16); }
        }

        /// <summary>
        /// Get the second key part.
        /// </summary>
        public string PartB
        {
            get { return Length != KeyLength.SingleLength ? Key.Substring(16, 16) : Key; }
            private set { Key = Length == KeyLength.DoubleLength ? PartA + value : PartA + value + PartC; }
        }

        /// <summary>
        /// Get the third key part.
        /// </summary>
        public string PartC
        {
            get { return Length == KeyLength.TripleLength ? Key.Substring(32, 16) : Key.Substring(0, 16); }
        }

        /// <summary>
        /// Creates a new instance of this class from a hexadecimal value.
        /// </summary>
        /// <param name="text"></param>
        public HexKey(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new InvalidCastException("Empty hex key not allowed");
            }

            Scheme = KeyScheme.Unspecified;
            text = text.ToUpper();

            if (text.StartsWithKeyScheme())
            {
                Scheme = text.Substring(0, 1).GetKeyScheme();
                text = text.Substring(1);
            }

            if (!text.IsHex())
            {
                throw new InvalidCastException(string.Format("Key {0} is not hex", text));
            }

            switch (text.Length)
            {
                case 16:
                    if (Scheme == KeyScheme.Unspecified)
                    {
                        Scheme = KeyScheme.SingleLengthKey;
                    }
                    Length = KeyLength.SingleLength;
                    break;
                case 32:
                    if (Scheme == KeyScheme.Unspecified)
                    {
                        Scheme = KeyScheme.DoubleLengthKeyAnsi;
                    }
                    Length = KeyLength.DoubleLength;
                    break;
                case 48:
                    if (Scheme == KeyScheme.Unspecified)
                    {
                        Scheme = KeyScheme.TripleLengthKeyAnsi;
                    }
                    Length = KeyLength.TripleLength;
                    break;
                default:
                    throw new InvalidCastException(string.Format("Invalid value {0} for key", text));
            }

            Key = text;
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>String representing this instance.</returns>
        public override string ToString()
        {
            return Scheme.GetKeySchemeChar().Replace("Z","") + Key;
        }

        /// <summary>
        /// Encrypt data under this key.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to encrypt.</param>
        /// <returns>Encrypted data.</returns>
        public string Encrypt (string data)
        {
            ValidateEncryptDecryptData(data);

            if (data.Length == 16)
            {
                return TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data);
            }

            if (data.Length == 32)
            {
                return TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(0, 16)) +
                       TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(16, 16));
            }

            return TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(0, 16)) +
                   TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(16, 16)) +
                   TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(32, 16));
        }

        /// <summary>
        /// Decrypt data using this key.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public string Decrypt(string data)
        {
            ValidateEncryptDecryptData(data);

            if (data.Length == 16)
            {
                return TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data);
            }

            if (data.Length == 32)
            {
                return TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(0, 16)) +
                       TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(16, 16));
            }

            return TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(0, 16)) +
                   TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(16, 16)) +
                   TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(32, 16));
        }

        /// <summary>
        /// Encrypt data under this key using CBC mode.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to encrypt.</param>
        /// <returns>Encrypted data.</returns>
        public string EncryptCbc (string data)
        {
            ValidateEncryptDecryptData(data);

            if (data.Length == 16)
            {
                return TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data);
            }

            if (data.Length == 32)
            {
                var resulta1 = TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(0, 16));
                return resulta1 + TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(16, 16).XorHex(resulta1));
            }

            var resulta2 = TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(0, 16));
            var resultb = TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(16, 16).XorHex(resulta2));
            return resulta2 + resultb + TripleDes.TripleDesEncrypt(PartA, PartB, PartC, data.Substring(32, 16).XorHex(resulta2));
        }

        /// <summary>
        /// Decrypt data using CBC mode.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public string DecryptCbc(string data)
        {
            ValidateEncryptDecryptData(data);

            if (data.Length == 16)
            {
                return TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data);
            }

            if (data.Length == 32)
            {
                var resulta1 = TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(0, 16));
                return resulta1 + TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(16, 16)).XorHex(data.Substring(0, 16));
            }

            var resulta2 = TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(0, 16));
            var resultb = TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(16, 16).XorHex(data.Substring(0, 16)));
            return resulta2 + resultb + TripleDes.TripleDesDecrypt(PartA, PartB, PartC, data.Substring(32, 16).XorHex(data.Substring(16, 16)));
        }

        /// <summary>
        /// Encrypt data under this key using the variant mode.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to encrypt.</param>
        /// <returns>Encrypted data.</returns>
        public string EncryptVariant (string data)
        {
            ValidateEncryptDecryptData(data);

            if (data.Length == 16)
            {
                throw new InvalidOperationException("Cannot use 8-byte data with EncryptVariant");
            }

            if (data.Length == 32)
            {
                var orgKeyPartB = PartB;
                PartB = PartB.XorHex(LMK.Variants.GetDoubleLengthVariant(1).PadRight(16, '0'));
                var result1 = Encrypt(data.Substring(0, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetDoubleLengthVariant(2).PadRight(16, '0'));
                var result2 = Encrypt(data.Substring(16, 16));
                PartB = orgKeyPartB;
                return result1 + result2;
            }
            else
            {
                var orgKeyPartB = PartB;
                PartB = PartB.XorHex(LMK.Variants.GetTripleLengthVariant(1).PadRight(16, '0'));
                var result1 = Encrypt(data.Substring(0, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetTripleLengthVariant(2).PadRight(16, '0'));
                var result2 = Encrypt(data.Substring(16, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetTripleLengthVariant(3).PadRight(16, '0'));
                var result3 = Encrypt(data.Substring(32, 16));
                PartB = orgKeyPartB;
                return result1 + result2 + result3;
            }
        }

        /// <summary>
        /// Decrypt data using the variant mode.
        /// </summary>
        /// <param name="data">16, 32 or 48 hex data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public string DecryptVariant (string data)
        {
            ValidateEncryptDecryptData(data);
            
            if (data.Length == 16)
            {
                throw new InvalidOperationException("Cannot use 8-byte data with DecryptVariant");
            }

            if (data.Length == 32)
            {
                var orgKeyPartB = PartB;
                PartB = PartB.XorHex(LMK.Variants.GetDoubleLengthVariant(1).PadRight(16, '0'));
                var result1 = Decrypt(data.Substring(0, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetDoubleLengthVariant(2).PadRight(16, '0'));
                var result2 = Decrypt(data.Substring(16, 16));
                PartB = orgKeyPartB;
                return result1 + result2;
            }
            else
            {
                var orgKeyPartB = PartB;
                PartB = PartB.XorHex(LMK.Variants.GetTripleLengthVariant(1).PadRight(16, '0'));
                var result1 = Decrypt(data.Substring(0, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetTripleLengthVariant(2).PadRight(16, '0'));
                var result2 = Decrypt(data.Substring(16, 16));
                PartB = orgKeyPartB.XorHex(LMK.Variants.GetTripleLengthVariant(3).PadRight(16, '0'));
                var result3 = Decrypt(data.Substring(32, 16));
                PartB = orgKeyPartB;
                return result1 + result2 + result3;
            }
        }

        private void ValidateEncryptDecryptData (string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new InvalidOperationException("Cannot use empty data with encypt/decrypt");
            }

            if (data.Length != 16 && data.Length != 32 && data.Length != 48)
            {
                throw new InvalidOperationException(string.Format("Data to encrypt/decrypt [{0}] has invalid length", data));
            }
        }
    }
}
