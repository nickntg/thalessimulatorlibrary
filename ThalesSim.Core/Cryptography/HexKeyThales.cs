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
    public class HexKeyThales
    {
        public KeyTypeCode Code { get; private set; }

        public string Key { get; private set; }

        public string ClearKey { get; private set; }

        public HexKey ClearHexKey { get { return new HexKey(ClearKey); } }

        public HexKeyThales (KeyTypeCode keyCode, int variant, bool clearKey, string key) : this(variant.ToString() + keyCode, clearKey, key) { }

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

        private void EncryptKey()
        {
            Key = KeyOperation(false, ClearKey);
        }

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
