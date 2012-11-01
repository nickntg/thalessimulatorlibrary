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
    [ThalesHostCommand("A6", "A7", "Imports a key encrypted under a ZMK")]
    public class ImportKey_A6 : AHostCommand
    {
        private string _keyType;
        private string _zmk;
        private string _key;
        private string _lmkScheme;
        private string _atallaVariant;

        public ImportKey_A6()
        {
            ReadXmlDefinitions();
        }

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
            _lmkScheme = KeyValues.Item("Key Scheme LMK");
            _atallaVariant = KeyValues.ItemOptional("Atalla Variant");
        }

        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            KeyTypeCode ktc = null;
            if (!ValidateKeyTypeCode(_keyType, mr, ref ktc))
            {
                return mr;
            }

            var lmk = KeyScheme.Unspecified;
            if (!ValidateKeySchemeCode(_lmkScheme, mr, ref lmk))
            {
                return mr;
            }

            if (!ValidateAuthStateRequirement(KeyFunction.Import, ktc.Pair, ktc.Variant, mr))
            {
                return mr;
            }

            var zmk = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _zmk);
            if (!zmk.ClearKey.IsParityOk(Parity.Odd))
            {
                mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                return mr;
            }

            var clearKey = zmk.DecryptWithScheme(_key, _atallaVariant);
            var warnParity = !clearKey.IsParityOk(Parity.Odd);

            var newKey = new HexKeyThales(ktc, true, clearKey);
            var checkValue = newKey.CheckValue;

            Log.InfoFormat("ZMK (clear): {0}", zmk.ClearKey);
            Log.InfoFormat("Key (clear): {0}", clearKey);
            Log.InfoFormat("Key (LMK): {0}", newKey.Key);
            Log.InfoFormat("Check value: {0}", checkValue);
            if (warnParity)
            {
                Log.WarnFormat("Key {0} does not have odd parity (would be {1})", clearKey, clearKey.MakeParity(Parity.Odd));
            }

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(lmk == KeyScheme.DoubleLengthKeyVariant || lmk == KeyScheme.TripleLengthKeyVariant
                          ? newKey.KeyVariant
                          : newKey.KeyAnsi);
            mr.Append(newKey.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
