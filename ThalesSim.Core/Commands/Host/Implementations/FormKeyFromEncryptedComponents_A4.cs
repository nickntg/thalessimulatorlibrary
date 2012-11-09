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
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales A4 implementation.
    /// </summary>
    [ThalesHostCommand("A4", "A5", "Forms a key from encrypted components")]
    [AuthorizedState]
    public class FormKeyFromEncryptedComponents_A4 : AHostCommand
    {
        private string _nbrComponents;
        private int _iNbrComponents;
        private string _keyTypeCode;
        private string _lmkScheme;
        private string[] _comps ;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public FormKeyFromEncryptedComponents_A4()
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
            _keyTypeCode = KeyValues.Item("Key Type");
            _lmkScheme = KeyValues.Item("Key Scheme (LMK)");
            _iNbrComponents = Convert.ToInt32(_nbrComponents);
            _comps = new string[_iNbrComponents];
            for (var i = 1; i <= _iNbrComponents; i++)
            {
                _comps[i - 1] = KeyValues.ItemCombination("Key Component Scheme #" + i.ToString(),
                                                          "Key Component #" + i.ToString());
            }
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            KeyTypeCode ktc = null;
            if (!ValidateKeyTypeCode(_keyTypeCode, mr, ref ktc))
            {
                return mr;
            }

            var lmkKs = KeyScheme.Unspecified;
            if (!ValidateKeySchemeCode(_lmkScheme, mr, ref lmkKs))
            {
                return mr;
            }

            var clearComps = new HexKeyThales[_iNbrComponents];
            var clearKey = string.Empty;
            for (var i = 1; i <= _iNbrComponents; i++)
            {
                clearComps[i - 1] = new HexKeyThales(ktc, false, _comps[i - 1]);
                if (!clearComps[i - 1].ClearKey.IsParityOk(Parity.Odd))
                {
                    mr.Append(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR);
                    return mr;
                }

                clearKey = !string.IsNullOrEmpty(clearKey) ? clearKey.XorHex(clearComps[i - 1].ClearKey) : clearComps[i - 1].ClearKey;
            }

            clearKey = clearKey.MakeParity(Parity.Odd);
            var cryptKey = new HexKeyThales(ktc, true, clearKey);

            for (var i = 1; i <= _iNbrComponents; i++)
            {
                Log.InfoFormat("Component {0} (clear): {1}", i, clearComps[i - 1].ClearKey);
            }
            Log.InfoFormat("Key (clear): {0}", clearKey);
            Log.InfoFormat("Check value: {0}", cryptKey.CheckValue);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            if (lmkKs == KeyScheme.DoubleLengthKeyVariant || lmkKs == KeyScheme.TripleLengthKeyVariant)
            {
                mr.Append(cryptKey.KeyVariant);
            }
            else
            {
                mr.Append(cryptKey.KeyAnsi);
            }
            mr.Append(cryptKey.CheckValue.Substring(0, 6));

            return mr;
        }
    }
}
