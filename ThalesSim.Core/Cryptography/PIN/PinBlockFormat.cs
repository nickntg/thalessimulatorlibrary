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

namespace ThalesSim.Core.Cryptography.PIN
{
    /// <summary>
    /// Enumerations representing the PIN block formats.
    /// </summary>
    public enum PinBlockFormat
    {
        /// <summary>
        /// ANSI X9.8 format.
        /// </summary>
        AnsiX98 = 1,

        /// <summary>
        /// Docutel format.
        /// </summary>
        Docutel = 2,

        /// <summary>
        /// Diebold format.
        /// </summary>
        Diebold = 3,

        /// <summary>
        /// Plus format.
        /// </summary>
        Plus = 4,

        /// <summary>
        /// ISO 94564/1 format.
        /// </summary>
        Iso94564_1 = 5
    }
}
