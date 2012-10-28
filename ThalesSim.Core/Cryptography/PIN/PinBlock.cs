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

namespace ThalesSim.Core.Cryptography.PIN
{
    /// <summary>
    /// This class is used to represent a PIN block.
    /// </summary>
    public class PinBlock
    {
        /// <summary>
        /// Get the clear PIN block.
        /// </summary>
        public string ClearPinBlock { get; private set; }

        /// <summary>
        /// Get the PIN block format.
        /// </summary>
        public PinBlockFormat Format { get; private set;}

        /// <summary>
        /// Get the account or padding string used
        /// to create the PIN block of this instance.
        /// </summary>
        public string AccountOrPadding { get; private set; }

        /// <summary>
        /// Get the PIN of this instance.
        /// </summary>
        public string Pin { get; private set; }

        /// <summary>
        /// Creates a new instance of this class from clear data.
        /// </summary>
        /// <param name="pin">PIN.</param>
        /// <param name="accountOrPadding">Account or padding string.</param>
        /// <param name="format">PIN block format.</param>
        public PinBlock (string pin, string accountOrPadding, PinBlockFormat format)
        {
            ClearPinBlock = pin.GetPinBlock(accountOrPadding, format);
            Pin = pin;
            AccountOrPadding = accountOrPadding;
            Format = format;
        }
        
        /// <summary>
        /// Creates a new instance of this class form encrypted data.
        /// </summary>
        /// <param name="encryptedPinBlock">Encrypted PIN block.</param>
        /// <param name="accountOrPadding">Account or padding string.</param>
        /// <param name="format">PIN block format.</param>
        /// <param name="clearKey">Clear encryption key.</param>
        public PinBlock (string encryptedPinBlock, string accountOrPadding, PinBlockFormat format, HexKey clearKey)
        {
            AccountOrPadding = accountOrPadding;
            Format = format;
            ClearPinBlock = clearKey.Decrypt(encryptedPinBlock);
            Pin = ClearPinBlock.GetPin(accountOrPadding, format);
        }

        /// <summary>
        /// Translte the PIN block under a different
        /// PIN block format.
        /// </summary>
        /// <param name="format">New PIN block format.</param>
        /// <returns>Translated PIN block.</returns>
        public string Translate (PinBlockFormat format)
        {
            var newBlock = new PinBlock(Pin, AccountOrPadding, format);
            return newBlock.ClearPinBlock;
        }

        /// <summary>
        /// Translate and encrypt PIN block.
        /// </summary>
        /// <param name="key">Clear encryption key.</param>
        /// <param name="format">New PIN block format.</param>
        /// <returns>Translated and encrypted PIN block.</returns>
        public string Translate (HexKey key, PinBlockFormat format)
        {
            var newBlock = new PinBlock(Pin, AccountOrPadding, format);
            return key.Encrypt(newBlock.ClearPinBlock);
        }
    }
}
