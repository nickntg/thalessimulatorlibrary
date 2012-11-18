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

using ThalesSim.Core.Cryptography.PIN;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales BA implementation.
    /// </summary>
    [ThalesHostCommand("BA", "BB", "Encrypts a clear PIN")]
    [AuthorizedState]
    public class EncryptClearPIN_BA : AHostCommand
    {
        private string _clearPin;
        private string _acctNbr;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public EncryptClearPIN_BA()
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

            _clearPin = KeyValues.Item("PIN");
            _acctNbr = KeyValues.Item("Account Number");

            if (_clearPin.IndexOf("F", System.StringComparison.Ordinal) > 0)
            {
                var newPin = _clearPin.Replace("F", "");
                _clearPin = newPin.PadLeft(_clearPin.Length, '0');
            }
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            _clearPin = _clearPin.Substring(1);
            if (_clearPin.Length < 4 || _clearPin.Length > 12)
            {
                mr.Append(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG);
                return mr;
            }

            var cryptPin = Encrypt.EncryptPinForHostStorage(_clearPin);

            Log.InfoFormat("Clear PIN: {0}", _clearPin);
            Log.InfoFormat("Account Number: {0}", _acctNbr);
            Log.InfoFormat("Encrypted PIN: {0}", cryptPin);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(cryptPin);

            return mr;
        }
    }
}
