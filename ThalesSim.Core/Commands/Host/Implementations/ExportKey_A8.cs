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
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales A8 implementation.
    /// </summary>
    [ThalesHostCommand("A8", "A9", "Exports a key under ZMK for transmission")]
    public class ExportKey_A8 : AHostCommand
    {
        private string _keyType;
        private string _zmk;
        private string _key;
        private string _zmkScheme;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public ExportKey_A8()
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
            _zmk = KeyValues.ItemCombination("ZMK Scheme", "ZMK");
            _key = KeyValues.ItemCombination("Key Scheme", "Key");
            _zmkScheme = KeyValues.Item("Key Scheme ZMK");
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

            var zmkKs = KeyScheme.Unspecified;
            if (!ValidateKeySchemeCode(_zmkScheme, mr, ref zmkKs))
            {
                return mr;
            }

            if (!ValidateAuthStateRequirement(KeyFunction.Export, ktc.Pair, ktc.Variant, mr))
            {
                return mr;
            }

            var zmk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _zmk);
            if (!zmk.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                return mr;
            }

            var key = new HexKeyThales(ktc, false, _key);
            if (!key.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR);
                return mr;
            }

            var cryptKey = zmk.EncryptWithScheme(zmkKs.GetKeySchemeChar() + key.ClearKey);

            Log.InfoFormat("ZMK (clear): {0}", zmk.ClearKey);
            Log.InfoFormat("Key (clear): {0}", key.ClearKey);
            Log.InfoFormat("Key (ZMK): {0}", cryptKey);
            Log.InfoFormat("Check value: {0}", key.CheckValue);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(zmkKs != KeyScheme.Unspecified && zmkKs != KeyScheme.SingleLengthKey
                          ? zmkKs.GetKeySchemeChar()
                          : string.Empty);
            mr.Append(cryptKey);
            mr.Append(key.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
