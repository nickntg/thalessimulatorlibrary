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
using ThalesSim.Core.Cryptography;
using ThalesSim.Core.Cryptography.LMK;
using ThalesSim.Core.Cryptography.PIN;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales EE command.
    /// </summary>
    [ThalesHostCommand("EE", "EF", "Derive a PIN using the IBM method")]
    public class DerivePinUsingTheIBMMethod_EE : AHostCommand
    {
        private string _pvkPair;
        private string _offsetValue;
        private string _checkLen;
        private string _acct;
        private string _decTable;
        private string _pinValData;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public DerivePinUsingTheIBMMethod_EE()
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

            _pvkPair = KeyValues.ItemCombination("PVK Scheme", "PVK");
            _offsetValue = KeyValues.Item("Offset");
            _checkLen = KeyValues.Item("Check Length");
            _acct = KeyValues.Item("Account Number");
            _decTable = KeyValues.Item("Decimalisation Table");
            _pinValData = KeyValues.Item("PIN Validation Data");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var pvk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair14_15), false, _pvkPair);
            if (!pvk.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR);
                return mr;
            }

            if (Convert.ToInt32(_checkLen) < 4)
            {
                mr.Append(ErrorCodes.ER_15_INVALID_INPUT_DATA);
                return mr;
            }

            var idx = _pinValData.IndexOf("N", StringComparison.Ordinal);
            var expPinValData = _pinValData.Substring(0, idx);
            expPinValData = expPinValData + _acct.Substring(_acct.Length - 5, 5);
            expPinValData = expPinValData + _pinValData.Substring(idx + 1, (_pinValData.Length - (idx + 1)));

            var cryptAcctNum = pvk.ClearHexKey.Encrypt(expPinValData);
            var decimalised = cryptAcctNum.Decimalise(_decTable);
            var naturalPin = decimalised.Substring(0, Convert.ToInt32(_checkLen));
            var derivedPin =
                naturalPin.AddWithoutCarry(_offsetValue.Substring(0, _offsetValue.IndexOf("F", StringComparison.Ordinal)));

            var cryptPin = Encrypt.EncryptPinForHostStorage(derivedPin);

            Log.InfoFormat("PVK (clear): {0}", pvk.ClearKey);
            Log.InfoFormat("Natural PIN: {0}", naturalPin);
            Log.InfoFormat("Derived PIN: {0}", derivedPin);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(cryptPin);

            return mr;
        }
    }
}
