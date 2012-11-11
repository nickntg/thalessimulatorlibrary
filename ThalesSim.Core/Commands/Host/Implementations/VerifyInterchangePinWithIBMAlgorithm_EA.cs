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
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales EA implementation
    /// </summary>
    [ThalesHostCommand("EA", "EB", "Verify interchange PIN using the IBM algorithm")]
    public class VerifyInterchangePinWithIBMAlgorithm_EA : AHostCommand
    {
        private string _acct;
        private string _pinBlock;
        private string _pbFormat;
        private string _pvkPair;
        private string _checkLen;
        private string _maxPinLen;
        private string _zpk;
        private string _decTable;
        private string _pinValData;
        private string _offsetValue;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public VerifyInterchangePinWithIBMAlgorithm_EA()
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

            _zpk = KeyValues.ItemCombination("ZPK Scheme", "ZPK");
            _pvkPair = KeyValues.ItemCombination("PVK Scheme", "PVK");
            _maxPinLen = KeyValues.Item("Maximum PIN Length");
            _pinBlock = KeyValues.Item("PIN Block");
            _pbFormat = KeyValues.Item("PIN Block Format Code");
            _checkLen = KeyValues.Item("Check Length");
            _acct = KeyValues.Item("Account Number");
            _decTable = KeyValues.Item("Decimalisation Table");
            _pinValData = KeyValues.Item("PIN Validation Data");
            _offsetValue = KeyValues.Item("Offset");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var zpk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair06_07), false, _zpk);
            if (!zpk.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                return mr;
            }

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

            if (!_pbFormat.IsPinBlockFormatSupported())
            {
                mr.Append(ErrorCodes.ER_23_INVALID_PIN_BLOCK_FORMAT_CODE);
                return mr;
            }

            var pbFormat = _pbFormat.GetPinBlockFormat();
            var clearPb = zpk.ClearHexKey.Decrypt(_pinBlock);
            var clearPin = clearPb.GetPin(_acct, pbFormat);

            if (clearPin.Length < 4 || clearPin.Length > 12)
            {
                mr.Append(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG);
                return mr;
            }

            var idx = _pinValData.IndexOf("N", StringComparison.Ordinal);
            var expPinValData = _pinValData.Substring(0, idx);
            expPinValData = expPinValData + _acct.Substring(_acct.Length - 5, 5);
            expPinValData = expPinValData + _pinValData.Substring(idx + 1, _pinValData.Length - idx - 1);
            var cryptAcctNum = pvk.ClearHexKey.Encrypt(expPinValData);
            var decimalisedAcctNum = cryptAcctNum.Decimalise(_decTable);
            var naturalPin = decimalisedAcctNum.Substring(0, Convert.ToInt32(_checkLen));
            var derivedPin =
                naturalPin.AddWithoutCarry(_offsetValue.Substring(0, _offsetValue.IndexOf("F", StringComparison.Ordinal)));

            Log.InfoFormat("PVK (clear): {0}", pvk.ClearKey);
            Log.InfoFormat("Expected Natural PIN: {0}", naturalPin);
            Log.InfoFormat("Expected Derived PIN: {0}", derivedPin);

            mr.Append(derivedPin != clearPin ? ErrorCodes.ER_01_VERIFICATION_FAILURE : ErrorCodes.ER_00_NO_ERROR);

            return mr;
        }
    }
}
