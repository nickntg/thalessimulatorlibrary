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

namespace ThalesSim.Core.Cryptography.LMK
{
    /// <summary>
    /// Enumeration of the LMK pairs.
    /// </summary>
    public enum LmkPair
    {
        /// <summary>
        /// LMK pair 00-01.
        /// </summary>
        /// <remarks>
        /// Contains the two smart card ""keys"" (Passwords if the HSM is configured for Password mode) required for setting the HSM into the Authorized state.
        /// </remarks>
        Pair00_01 = 0,
        /// <summary>
        /// LMK pair 02-03.
        /// </summary>
        /// <remarks>
        /// Encrypts the PINs for host storage.
        /// </remarks>
        Pair02_03 = 1,
        /// <summary>
        /// LMK pair 04-05.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Master Keys and double-length ZMKs. Encrypts Zone Master Key components under a Variant.
        /// </remarks>
        Pair04_05 = 2,
        /// <summary>
        /// LMK pair 06-07.
        /// </summary>
        /// <remarks>
        /// Encrypts the Zone PIN keys for interchange transactions.
        /// </remarks>
        Pair06_07 = 3,
        /// <summary>
        /// LMK pair 08-09.
        /// </summary>
        /// <remarks>
        /// Used for random number generation.
        /// </remarks>
        Pair08_09 = 4,
        /// <summary>
        /// LMK pair 10-11.
        /// </summary>
        /// <remarks>
        /// Used for encrypting keys in HSM buffer areas.
        /// </remarks>
        Pair10_11 = 5,
        /// <summary>
        /// LMK pair 12-13.
        /// </summary>
        /// <remarks>
        /// The initial set of Secret Values created by the user; used for generating all other Master Key pairs.
        /// </remarks>
        Pair12_13 = 6,
        /// <summary>
        /// LMK pair 14-15.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Master Keys, Terminal PIN Keys and PIN Verification Keys. Encrypts Card Verification Keys under a Variant.
        /// </remarks>
        Pair14_15 = 7,
        /// <summary>
        /// LMK pair 16-17.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Authentication Keys.
        /// </remarks>
        Pair16_17 = 8,
        /// <summary>
        /// LMK pair 18-19
        /// </summary>
        /// <remarks>
        /// Encrypts reference numbers for solicitation mailers.
        /// </remarks>
        Pair18_19 = 9,
        /// <summary>
        /// LMK pair 20-21.
        /// </summary>
        /// <remarks>
        /// Encrypts 'not on us' PIN Verification Keys and Card Verification Keys under a Variant.
        /// </remarks>
        Pair20_21 = 10,
        /// <summary>
        /// LMK pair 22-23.
        /// </summary>
        /// <remarks>
        /// Encrypts Watchword Keys.
        /// </remarks>
        Pair22_23 = 11,
        /// <summary>
        /// LMK pair 24-25.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Transport Keys.
        /// </remarks>
        Pair24_25 = 12,
        /// <summary>
        /// LMK pair 26-27.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Authentication Keys.
        /// </remarks>
        Pair26_27 = 13,
        /// <summary>
        /// LMK pair 28-29.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Derivation Keys.
        /// </remarks>
        Pair28_29 = 14,
        /// <summary>
        /// LMK pair 30-31.
        /// </summary>
        /// <remarks>
        /// Encrypts Zone Encryption Keys.
        /// </remarks>
        Pair30_31 = 15,
        /// <summary>
        /// LMK pair 32-33.
        /// </summary>
        /// <remarks>
        /// Encrypts Terminal Encryption Keys.
        /// </remarks>
        Pair32_33 = 16,
        /// <summary>
        /// LMK pair 34-35.
        /// </summary>
        /// <remarks>
        /// Encrypts RSA keys.
        /// </remarks>
        Pair34_35 = 17,
        /// <summary>
        /// LMK pair 36-37.
        /// </summary>
        /// <remarks>
        /// Encrypts RSA MAC keys.
        /// </remarks>
        Pair36_37 = 18,
        /// <summary>
        /// LMK pair 38-39.
        /// </summary>
        /// <remarks>
        /// LMK pair 38-39.
        /// </remarks>
        Pair38_39 = 19
    }
}
