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

namespace ThalesSim.Core.Cryptography.MAC
{
    /// <summary>
    /// Implements ISO 9797 padding methods.
    /// </summary>
    public class Iso9797Pad
    {
        /// <summary>
        /// Pads a string to an 8-byte boundary.
        /// </summary>
        /// <param name="data">Data string to pad.</param>
        /// <param name="paddingMethod">Padding method to use.</param>
        /// <returns>Padded string.</returns>
        public static string PadHexString (string data, Iso9797PaddingMethodType paddingMethod)
        {
            if (paddingMethod == Iso9797PaddingMethodType.NoPadding)
            {
                return data;
            }

            var firstPad = "80";
            if (paddingMethod == Iso9797PaddingMethodType.PaddingMethod1)
            {
                firstPad = "00";
            }

            if ((data.Length / 2) % 8 == 0)
            {
                return data + firstPad + "00000000000000";
            }

            data = data + firstPad;
            while ((data.Length / 2) % 8 != 0)
            {
                data = data + "00";
            }
            return data;
        }
    }
}
