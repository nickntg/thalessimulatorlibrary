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
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography
{
    public class KeyTypeCode
    {
        public LmkPair Pair { get; set; }

        public int Variant { get; set; }

        public KeyTypeCode (string keyTypeCode)
        {
            if (string.IsNullOrEmpty(keyTypeCode) || keyTypeCode.Length != 3 || !keyTypeCode.IsHex())
            {
                throw new InvalidCastException(string.Format("Invalid key type code {0}", keyTypeCode));
            }

            if (!char.IsDigit(keyTypeCode.ToCharArray()[0]))
            {
                throw new InvalidCastException(string.Format("Invalid variant number {0}", keyTypeCode.Substring(0,1)));
            }

            Variant = Convert.ToInt32(keyTypeCode.Substring(0, 1));

            Pair = keyTypeCode.Substring(1).GetLmkPair();
        }
    }
}
