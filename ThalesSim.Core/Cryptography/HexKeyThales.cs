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
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography
{
    /// <summary>
    /// This class is used to represent a Thales key of any type.
    /// </summary>
    public class HexKeyThales
    {
        /// <summary>
        /// Get/set the key type code of this instance.
        /// </summary>
        public KeyTypeCode Code { get; private set; }

        /// <summary>
        /// Get/set the encrypted value of the key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Get/set the clear value of the key.
        /// </summary>
        public string ClearKey { get; private set; }

        /// <summary>
        /// Get/set a HexKey using the clear value of the key.
        /// </summary>
        public HexKey ClearHexKey { get { return new HexKey(ClearKey); } }

        /// <summary>
        /// Get the check value of the key represented by this instance.
        /// </summary>
        public string CheckValue { get; private set; }

        public string KeyAnsi { get; private set; }

        public string KeyVariant { get; private set; }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="keyCode">Key type code.</param>
        /// <param name="clearKey">True if created using a clear key.</param>
        /// <param name="key">Key value.</param>
        public HexKeyThales (KeyTypeCode keyCode, bool clearKey, string key) : this(keyCode.ToString(), clearKey, key) { }

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="keyTypeCode">Key type code.</param>
        /// <param name="clearKey">True if created using a clear key.</param>
        /// <param name="key">Key value.</param>
        public HexKeyThales (string keyTypeCode, bool clearKey, string key)
        {
            Code = new KeyTypeCode(keyTypeCode);

            if (!clearKey)
            {
                Key = key;

                DecryptKey();
            }
            else
            {
                ClearKey = key;

                EncryptKey();
            }

            CalculateEncryptedValues();
            CheckValue = ClearHexKey.Encrypt("0000000000000000");
        }

        /// <summary>
        /// Encrypt data using variant-encrypt when appropriate.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <returns>Encrypted data.</returns>
        public string EncryptWithScheme(string data)
        {
            return EncryptWithScheme(data, string.Empty);
        }

        /// <summary>
        /// Encrypt data using variant-encrypt when appropriate.
        /// </summary>
        /// <param name="data">Data to encrypt.</param>
        /// <param name="atallaVariant">Atalla variant</param>
        /// <returns>Encrypted data.</returns>
        public string EncryptWithScheme(string data, string atallaVariant)
        {
            var scheme = data.StartsWithKeyScheme() ? data.GetKeyScheme() : KeyScheme.Unspecified;
            switch (scheme)
            {
                case KeyScheme.DoubleLengthKeyVariant:
                case KeyScheme.TripleLengthKeyVariant:
                    return TransformAtalla(atallaVariant).EncryptVariant(data);
                default:
                    return TransformAtalla(atallaVariant).Encrypt(data);
            }
        }

        /// <summary>
        /// Decrypt data using variant-decrypt when appropriate.
        /// </summary>
        /// <param name="data">Data to decrypt.</param>
        /// <returns>Decrypted data.</returns>
        public string DecryptWithScheme(string data)
        {
            return DecryptWithScheme(data, string.Empty);
        }

        /// <summary>
        /// Decrypt data using variant-decrypt when appropriate.
        /// </summary>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="atallaVariant">Atalla variant.</param>
        /// <returns>Decrypted data.</returns>
        public string DecryptWithScheme (string data, string atallaVariant)
        {
            var scheme = data.StartsWithKeyScheme() ? data.GetKeyScheme() : KeyScheme.Unspecified;
            switch (scheme)
            {
                case KeyScheme.DoubleLengthKeyVariant:
                case KeyScheme.TripleLengthKeyVariant:
                    return TransformAtalla(atallaVariant).DecryptVariant(data.StripKeyScheme());
                default:
                    return TransformAtalla(atallaVariant).Decrypt(data.StripKeyScheme());
            }            
        }

        /// <summary>
        /// Returns a new HexKey transformed using the Attalla variant.
        /// </summary>
        /// <param name="atallaVariant">Atalla variant.</param>
        /// <returns>Transformed key.</returns>
        private HexKey TransformAtalla (string atallaVariant)
        {
            if (string.IsNullOrEmpty(atallaVariant))
            {
                return ClearHexKey;
            }

            var len = 0x8*Convert.ToInt32(atallaVariant);
            var varStr = len != 8 ? Convert.ToString(len, 16).PadRight(16, '0') : "08".PadRight(16, '0');
            var thisKey = ClearHexKey;
            var partA = thisKey.PartA.XorHex(varStr);
            if (thisKey.Length != KeyLength.SingleLength)
            {
                var partB = thisKey.PartB.XorHex(varStr);
                if (thisKey.Length != KeyLength.TripleLength)
                {
                    var partC = thisKey.PartC.XorHex(varStr);
                    return new HexKey(partA + partB + partC);
                }
                return new HexKey(partA + partB);
            }
            return new HexKey(partA);
        }

        /// <summary>
        /// Encrypt the clear key.
        /// </summary>
        private void EncryptKey()
        {
            Key = KeyOperation(false, ClearKey);
        }

        /// <summary>
        /// Decrypt the encrypted key.
        /// </summary>
        private void DecryptKey()
        {
            ClearKey = KeyOperation(true, Key).StripKeyScheme();
        }

        private string KeyOperation (bool decrypt, string key)
        {
            var scheme = KeyScheme.Unspecified;

            if (key.StartsWithKeyScheme())
            {
                scheme = key.GetKeyScheme();
            }

            string result;
            var lmk = new HexKey(LMK.LmkStorage.LmkVariant(Code.Pair, Code.Variant));

            switch (scheme)
            {
                case KeyScheme.Unspecified:
                case KeyScheme.SingleLengthKey:
                case KeyScheme.DoubleLengthKeyAnsi:
                case KeyScheme.TripleLengthKeyAnsi:
                    result = decrypt ? lmk.Decrypt(key.StripKeyScheme()) : lmk.Encrypt(key.StripKeyScheme());
                    break;
                default:
                    result = decrypt
                                 ? lmk.DecryptVariant(key.StripKeyScheme())
                                 : lmk.EncryptVariant(key.StripKeyScheme());
                    break;
            }

            return scheme != KeyScheme.Unspecified && scheme != KeyScheme.SingleLengthKey
                          ? scheme.GetKeySchemeChar() + result
                          : result;
        }

        private void CalculateEncryptedValues()
        {
            var scheme = KeyScheme.Unspecified;

            if (Key.StartsWithKeyScheme())
            {
                scheme = Key.GetKeyScheme();
            }
            else if (scheme == KeyScheme.Unspecified && Key.GetKeyLength() != KeyLength.SingleLength)
            {
                switch (Key.GetKeyLength())
                {
                    case KeyLength.DoubleLength:
                        scheme = KeyScheme.DoubleLengthKeyAnsi;
                        break;
                    default:
                        scheme = KeyScheme.TripleLengthKeyAnsi;
                        break;
                }

                Key = scheme.GetKeySchemeChar() + Key;
            }

            var lmk = new HexKey(LMK.LmkStorage.LmkVariant(Code.Pair, Code.Variant));

            switch (scheme)
            {
                case KeyScheme.Unspecified:
                case KeyScheme.SingleLengthKey:
                    KeyVariant = string.Empty;
                    KeyAnsi = Key;
                    break;
                case KeyScheme.DoubleLengthKeyAnsi:
                case KeyScheme.TripleLengthKeyAnsi:
                    KeyAnsi = Key;
                    KeyVariant = lmk.EncryptVariant(ClearKey);
                    switch (scheme)
                    {
                        case KeyScheme.DoubleLengthKeyAnsi:
                            KeyVariant = KeyScheme.DoubleLengthKeyVariant.GetKeySchemeChar() + KeyVariant;
                            break;
                        case KeyScheme.TripleLengthKeyAnsi:
                            KeyVariant = KeyScheme.TripleLengthKeyVariant.GetKeySchemeChar() + KeyVariant;
                            break;
                    }
                    break;
                default:
                    KeyVariant = Key;
                    KeyAnsi = lmk.Encrypt(ClearKey);
                    switch (scheme)
                    {
                        case KeyScheme.DoubleLengthKeyVariant:
                            KeyAnsi = KeyScheme.DoubleLengthKeyAnsi.GetKeySchemeChar() + ClearKey;
                            break;
                        case KeyScheme.TripleLengthKeyVariant:
                            KeyAnsi = KeyScheme.TripleLengthKeyVariant.GetKeySchemeChar() + ClearKey;
                            break;
                    }
                    break;
            }

        }
    }
}
