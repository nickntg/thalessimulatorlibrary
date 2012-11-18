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
    /// Thales BA implementation.
    /// </summary>
    [ThalesHostCommand("GG", "GH", "Form a ZMK from three encrypted components")]
    [AuthorizedState]
    public class FormZMKFromThreeComponents_GG : AHostCommand
    {
        private string _keyA;
        private string _keyB;
        private string _keyC;
        private string _keySchemeLmk;
        private string _keyCheckValue;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public FormZMKFromThreeComponents_GG()
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

            _keyA = KeyValues.Item("ZMK Component #1");
            _keyB = KeyValues.Item("ZMK Component #2");
            _keyC = KeyValues.Item("ZMK Component #3");
            _keySchemeLmk = KeyValues.ItemOptional("Key Scheme LMK");
            _keyCheckValue = KeyValues.ItemOptional("Key Check Value Type");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var keyA = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _keyA);
            var keyB = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _keyB);
            var keyC = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _keyC);

            if (!keyA.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                return mr;
            }

            if (!keyB.ClearKey.IsParityOk(Parity.Odd) || !keyC.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR);
                return mr;
            }

            var ks = KeyScheme.Unspecified;

            if (!string.IsNullOrEmpty(_keySchemeLmk))
            {
                if (!ValidateKeySchemeCode(_keySchemeLmk, mr, ref ks))
                {
                    return mr;
                }

                if (ks == KeyScheme.TripleLengthKeyAnsi || ks == KeyScheme.TripleLengthKeyVariant)
                {
                    mr.Append(ErrorCodes.ER_26_INVALID_KEY_SCHEME);
                    return mr;
                }
            }
            else
            {
                ks = keyA.ClearHexKey.Scheme;
                _keyCheckValue = "0";
            }

            var clearKey = keyA.ClearKey.XorHex(keyB.ClearKey.XorHex(keyC.ClearKey));
            var cryptKey = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), true, clearKey);

            Log.InfoFormat("Component A (clear): {0}", keyA.ClearKey);
            Log.InfoFormat("Component B (clear): {0}", keyB.ClearKey);
            Log.InfoFormat("Component C (clear): {0}", keyC.ClearKey);
            Log.InfoFormat("Key (clear): {0}", clearKey);
            Log.InfoFormat("Key (LMK): {0}",
                           ks == KeyScheme.DoubleLengthKeyVariant ? cryptKey.KeyVariant: cryptKey.KeyAnsi);
            Log.InfoFormat("Check value: {0}",
                           _keyCheckValue == "0" ? cryptKey.CheckValue : cryptKey.CheckValue.Substring(0, 6));

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(ks == KeyScheme.DoubleLengthKeyVariant ? cryptKey.KeyVariant: cryptKey.KeyAnsi);
            mr.Append(_keyCheckValue == "0" ? cryptKey.CheckValue : cryptKey.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
