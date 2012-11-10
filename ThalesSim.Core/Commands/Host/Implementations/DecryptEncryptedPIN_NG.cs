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
    /// Thales NG implementation. Dummy, no processing by design.
    /// </summary>
    [ThalesHostCommand("NG", "NH", "Decrypts an encrypted PIN")]
    [AuthorizedState]
    public class DecryptEncryptedPIN_NG : AHostCommand
    {
        private string _cryptPin;
        private string _acctNbr;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public DecryptEncryptedPIN_NG()
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

            _cryptPin = KeyValues.Item("PIN");
            _acctNbr = KeyValues.Item("Account Number");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var clearPin = Encrypt.DecryptPinUnderHostStorage(_cryptPin).PadRight(_cryptPin.Length, 'F');

            Log.InfoFormat("Encrypted PIN: {0}", _cryptPin);
            Log.InfoFormat("Account number: {0}", _acctNbr);
            Log.InfoFormat("Clear PIN: {0}", clearPin);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(clearPin);

            return mr;
        }
    }
}
