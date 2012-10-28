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

namespace ThalesSim.Core.Cryptography
{
    /// <summary>
    /// Enumeration of the key schemes.
    /// </summary>
    public enum KeyScheme
    {
        /// <summary>
        /// Single length key.
        /// </summary>
        SingleLengthKey = 0,

        /// <summary>
        /// Double length variant key.
        /// </summary>
        DoubleLengthKeyVariant = 1,

        /// <summary>
        /// Double length ANSI key.
        /// </summary>
        DoubleLengthKeyAnsi = 2,

        /// <summary>
        /// Triple length variant key.
        /// </summary>
        TripleLengthKeyVariant = 3,

        /// <summary>
        /// Triple length ANSI key.
        /// </summary>
        TripleLengthKeyAnsi = 4,

        /// <summary>
        /// Unspecified format.
        /// </summary>
        Unspecified = 5
    }
}
