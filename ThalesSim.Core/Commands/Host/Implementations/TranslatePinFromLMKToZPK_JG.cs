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

using ThalesSim.Core.Cryptography;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Cryptography.PIN;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales JG implementation.
    /// </summary>
    [ThalesHostCommand("JG", "JH", "Translates a PIN from LMK to ZPK encryption")]
    public class TranslatePinFromLMKToZPK_JG : AHostCommand
    {
        private string _acct;
        private string _targetKey;
        private string _pbFormat;
        private string _pin;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public TranslatePinFromLMKToZPK_JG()
        {
            ReadXmlDefinitions();
        }

        /// <summary>
        /// Accept message from client.
        /// </summary>
        /// <param name="message">Request message.</param>
        public override void AcceptMessage(StreamMessage message)
        {
            base.AcceptMessage(message);
            if (XmlParseResult != ErrorCodes.ER_00_NO_ERROR)
            {
                return;
            }

            _targetKey = KeyValues.ItemCombination("ZPK Scheme", "ZPK");
            _pbFormat = KeyValues.Item("PIN Block Format Code");
            _acct = KeyValues.Item("Account Number");
            _pin = KeyValues.Item("PIN");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var zpk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair06_07), false, _targetKey);
            if (!zpk.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                return mr;
            }

            if (!_pbFormat.IsPinBlockFormatSupported())
            {
                mr.Append(ErrorCodes.ER_23_INVALID_PIN_BLOCK_FORMAT_CODE);
                return mr;
            }

            var pbformat = _pbFormat.GetPinBlockFormat();
            var clearPin = Encrypt.DecryptPinUnderHostStorage(_pin);

            if (clearPin.Length < 4 || clearPin.Length > 12)
            {
                mr.Append(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG);
                return mr;
            }

            var clearPb = clearPin.GetPinBlock(_acct, pbformat);
            var cryptPb = zpk.ClearHexKey.Encrypt(clearPb);

            Log.InfoFormat("Clear ZPK: {0}", zpk.ClearKey);
            Log.InfoFormat("Clear PIN: {0}", clearPin);
            Log.InfoFormat("Clear PIN Block: {0}", clearPb);
            Log.InfoFormat("Encrypted PIN Block: {0}", cryptPb);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(cryptPb);

            return mr;
        }
    }
}
