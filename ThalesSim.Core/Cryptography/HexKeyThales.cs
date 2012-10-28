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
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="keyCode">Key type code.</param>
        /// <param name="variant">Variant.</param>
        /// <param name="clearKey">True if created using a clear key.</param>
        /// <param name="key">Key value.</param>
        public HexKeyThales (KeyTypeCode keyCode, int variant, bool clearKey, string key) : this(variant.ToString() + keyCode, clearKey, key) { }

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
            ClearKey = KeyOperation(true, Key);
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
    }
}
