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
    public class PinBlock
    {
        public string ClearPinBlock { get; private set; }

        public PinBlockFormat Format { get; private set;}

        public string AccountOrPadding { get; private set; }

        public string Pin { get; private set; }

        public PinBlock (string pin, string accountOrPadding, PinBlockFormat format)
        {
            ClearPinBlock = pin.GetPinBlock(accountOrPadding, format);
            Pin = pin;
            AccountOrPadding = accountOrPadding;
            Format = format;
        }

        public PinBlock (string encryptedPinBlock, string accountOrPadding, PinBlockFormat format, HexKey clearKey)
        {
            AccountOrPadding = accountOrPadding;
            Format = format;
            ClearPinBlock = clearKey.Decrypt(encryptedPinBlock);
            Pin = ClearPinBlock.GetPin(accountOrPadding, format);
        }

        public string Translate (PinBlockFormat format)
        {
            var newBlock = new PinBlock(Pin, AccountOrPadding, format);
            return newBlock.ClearPinBlock;
        }

        public string Translate (HexKey key, PinBlockFormat format)
        {
            var newBlock = new PinBlock(Pin, AccountOrPadding, format);
            return key.Encrypt(newBlock.ClearPinBlock);
        }
    }
}
