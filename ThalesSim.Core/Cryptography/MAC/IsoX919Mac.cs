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

using ThalesSim.Core.Cryptography.DES;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Cryptography.MAC
{
    /// <summary>
    /// This class implements the ISO X9.19 MAC algorithm with zero-based padding.
    /// </summary>
    public class IsoX919Mac
    {
        /// <summary>
        /// Calculates a MAC using the X9.19 algorithm.
        /// </summary>
        /// <param name="data">Hex data to MAC.</param>
        /// <param name="key">MACing key.</param>
        /// <param name="iv">Initial vector.</param>
        /// <param name="blockType">Message block to MAC.</param>
        /// <returns>MAC result.</returns>
        public static string MacHexData (string data, HexKey key, string iv, IsoX919BlockType blockType)
        {
            if (data.Length % 16 != 0)
            {
                data = Iso9797Pad.PadHexString(data, Iso9797PaddingMethodType.PaddingMethod1);
            }

            for (var i = 0; i <= (data.Length / 16) - 1; i++)
            {
                iv = iv.XorHex(data.Substring(i*16, 16));
                iv = TripleDes.DesEncrypt(key.PartA, iv);
            }

            var result = iv;

            if (blockType == IsoX919BlockType.FinalBlock || blockType == IsoX919BlockType.OnlyBlock)
            {
                result = TripleDes.DesDecrypt(key.PartB, iv);
                result = TripleDes.DesEncrypt(key.PartA, result);
            }

            return result;

        }
    }
}
