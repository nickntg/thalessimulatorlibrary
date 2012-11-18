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
    /// Thales A0 implementation.
    /// </summary>
    [ThalesHostCommand("A0", "A1", "Geneerates and encrypts key under ZMK for transmission")]
    public class GenerateKey_A0 : AHostCommand
    {
        private string _modeFlag;
        private string _keyType;
        private string _keyScheme;
        private string _zmk;
        private string _zmkScheme;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public GenerateKey_A0()
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

            _modeFlag = KeyValues.Item("Mode");
            _keyType = KeyValues.Item("Key Type");
            _keyScheme = KeyValues.Item("Key Scheme LMK");
            _zmkScheme = KeyValues.ItemOptional("Key Scheme ZMK");
            _zmk = KeyValues.ItemOptional("ZMK Scheme") + KeyValues.ItemOptional("ZMK");
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

            var zmkKs = KeyScheme.Unspecified;

            if (!string.IsNullOrEmpty(_zmkScheme))
            {
                if (!ValidateKeySchemeCode(_zmkScheme, mr, ref zmkKs))
                {
                    return mr;
                }
            }

            if (!ValidateAuthStateRequirement(KeyFunction.Generate, ktc.Pair, ktc.Variant, mr))
            {
                return mr;
            }

            var rndKey = string.Empty.RandomKey(ks);
            var thalesRndKey = new HexKeyThales(ktc, true, rndKey);

            Log.InfoFormat("Key generated (clear): {0}", rndKey);
            Log.InfoFormat("Key generated (LMK, ANSI): {0}", thalesRndKey.KeyAnsi);
            Log.InfoFormat("Key generated (LMK, Variant): {0}", thalesRndKey.KeyVariant);
            Log.InfoFormat("Check value: {0}", thalesRndKey.CheckValue);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(ks == KeyScheme.DoubleLengthKeyAnsi || ks == KeyScheme.TripleLengthKeyAnsi
                          ? thalesRndKey.KeyAnsi
                          : thalesRndKey.KeyVariant);

            if (!string.IsNullOrEmpty(_zmk) && _modeFlag == "1")
            {
                var zmk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _zmk);
                if (!zmk.ClearKey.IsParityOk(Parity.Odd))
                {
                    mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                    return mr;
                }

                var cryptUnderZmk = zmk.EncryptWithScheme(zmkKs.GetKeySchemeChar() + rndKey);

                Log.InfoFormat("ZMK (clear): {0}", zmk.ClearKey);
                Log.InfoFormat("Key under ZMK: {0}", zmkKs.GetKeySchemeChar() + cryptUnderZmk);

                mr.Append(zmkKs.GetKeySchemeChar() + cryptUnderZmk);
            }

            mr.Append(thalesRndKey.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
