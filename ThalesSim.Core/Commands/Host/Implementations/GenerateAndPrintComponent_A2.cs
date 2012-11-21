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
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales A2 implementation.
    /// </summary>
    [ThalesHostCommand("A2", "A3", "AZ", "Generates a random component and prints it in the clear")]
    [AuthorizedState]
    public class GenerateAndPrintComponent_A2 : AHostCommand
    {
        private string _keyType;
        private string _keyScheme;
        private string _result;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public GenerateAndPrintComponent_A2()
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

            _keyType = KeyValues.Item("Key Type");
            _keyScheme = KeyValues.Item("Key Scheme LMK");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            KeyTypeCode ktc = null;

            if (!ValidateKeyTypeCode(_keyType, mr, ref ktc))
            {
                return mr;
            }

            var ks = KeyScheme.Unspecified;
            if (!ValidateKeySchemeCode(_keyScheme, mr, ref ks))
            {
                return mr;
            }

            var rndKey = string.Empty.RandomKey(ks, Parity.Odd);
            var cryptKey = new HexKeyThales(ktc, true, rndKey);

            Log.InfoFormat("Key generated (clear): {0}", rndKey);
            Log.InfoFormat("Key generated (ANSI): {0}", cryptKey.KeyAnsi);
            Log.InfoFormat("Key generated (Variant): {0}", cryptKey.KeyVariant);

            AddPrinterData("CLEAR COMPONENT");
            AddPrinterData(rndKey);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(ks == KeyScheme.DoubleLengthKeyVariant || ks == KeyScheme.TripleLengthKeyVariant
                          ? cryptKey.KeyVariant
                          : cryptKey.KeyAnsi);

            _result = "OK";

            return mr;
        }

        /// <summary>
        /// Return response after I/O.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponseAfterIo()
        {
            if (!string.IsNullOrEmpty(_result))
            {
                var mr = new StreamResponse();
                mr.Append(ErrorCodes.ER_00_NO_ERROR);
                return mr;
            }

            return null;
        }
    }
}
