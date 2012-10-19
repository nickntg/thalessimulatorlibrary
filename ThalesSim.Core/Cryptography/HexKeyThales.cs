﻿/*
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

        public HexKeyThales (string keyTypeCode, string key)
        {
            Code = new KeyTypeCode(keyTypeCode);

            Key = key;

            DecryptKey();
        }

        private void DecryptKey()
        {
            var scheme = KeyScheme.Unspecified;

            if (Key.StartsWithKeyScheme())
            {
                scheme = Key.GetKeyScheme();
            }

            string result;
            var lmk = new HexKey(LMK.LmkStorage.LmkVariant(Code.Pair, Code.Variant));

            switch (scheme)
            {
                case KeyScheme.Unspecified:
                case KeyScheme.SingleLengthKey:
                case KeyScheme.DoubleLengthKeyAnsi:
                case KeyScheme.TripleLengthKeyAnsi:
                    result = lmk.Decrypt(Key.StripKeyScheme());
                    break;
                default:
                    result = lmk.DecryptVariant(Key.StripKeyScheme());
                    break;
            }

            ClearKey = scheme != KeyScheme.Unspecified && scheme != KeyScheme.SingleLengthKey
                           ? scheme.GetKeySchemeChar() + result
                           : result;
        }
    }
}
