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
    /// Thales GY implementation.
    /// </summary>
    [ThalesHostCommand("GY", "GZ", "Forms a ZMK from 2 to 9 components")]
    [AuthorizedState]
    public class FormZMKFromTwoToNineComponents_GY : AHostCommand
    {
        private string _nbrComponents;
        private int _iNbrComponents;
        private string _lmkScheme;
        private string _keyCheckValue;
        private readonly string[] _comps = new string[8];

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public FormZMKFromTwoToNineComponents_GY()
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
            _nbrComponents = KeyValues.Item("Number of Components");
            _iNbrComponents = Convert.ToInt32(_nbrComponents);
            for (var i = 1; i <= _iNbrComponents; i++)
            {
                _comps[i - 1] = KeyValues.ItemCombination("ZMK Component Scheme #" + i.ToString(),
                                                          "ZMK Component #" + i.ToString());
            }
            _lmkScheme = KeyValues.ItemOptional("Key Scheme LMK");
            _keyCheckValue = KeyValues.ItemOptional("Key Check Value Type");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var lmkKs = KeyScheme.Unspecified;
            if (!string.IsNullOrEmpty(_lmkScheme))
            {
                if (!ValidateKeySchemeCode(_lmkScheme, mr, ref lmkKs))
                {
                    return mr;
                }
            }
            else
            {
                lmkKs = _comps[0].Length >= 32 ? KeyScheme.DoubleLengthKeyAnsi : KeyScheme.SingleLengthKey;
            }

            if (string.IsNullOrEmpty(_keyCheckValue))
            {
                _keyCheckValue = "0";
            }

            var clearKeys = new HexKeyThales[8];
            var clearKey = string.Empty;
            for (var i = 1; i <= _iNbrComponents; i++)
            {
                if (!ConfigHelpers.InLegacyMode())
                {
                    if (_comps[i-1].Length % 16 != 0)
                    {
                        Log.Error("Legacy mode is off - key components must be in the form of 16H/32H only.");
                        mr.Append(ErrorCodes.ER_15_INVALID_INPUT_DATA);
                        return mr;
                    }
                }

                clearKeys[i-1] = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), false, _comps[i-1]);
                if (!clearKeys[i-1].ClearKey.IsParityOk(Parity.Odd))
                {
                    mr.Append(i == 1
                                  ? ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR
                                  : ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR);
                    return mr;
                }

                clearKey = !string.IsNullOrEmpty(clearKey) ? clearKey.XorHex(clearKeys[i - 1].ClearKey) : clearKeys[i - 1].ClearKey;
            }

            clearKey = clearKey.MakeParity(Parity.Odd);
            var cryptKey = new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair04_05), true, clearKey);

            for (var i = 1; i <= _iNbrComponents; i++)
            {
                Log.InfoFormat("Component #{0} (clear): {1}", i, clearKeys[i - 1].ClearKey);
            }

            Log.InfoFormat("Key (clear): {0}", clearKey);
            Log.InfoFormat("Key (LMK): {0}",
                           lmkKs == KeyScheme.DoubleLengthKeyVariant ? cryptKey.KeyVariant : cryptKey.KeyAnsi);
            Log.InfoFormat("Check value: {0}",
                           _keyCheckValue == "0" ? cryptKey.CheckValue : cryptKey.CheckValue.Substring(0, 6));

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            
            var returnedKey = lmkKs == KeyScheme.DoubleLengthKeyVariant
                                  ? cryptKey.KeyVariant
                                  : lmkKs == KeyScheme.Unspecified
                                        ? cryptKey.KeyAnsi.StripKeyScheme()
                                        : cryptKey.KeyAnsi;
            mr.Append(string.IsNullOrEmpty(_lmkScheme) ? returnedKey.StripKeyScheme() : returnedKey);

            mr.Append(_keyCheckValue == "0" ? cryptKey.CheckValue : cryptKey.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
