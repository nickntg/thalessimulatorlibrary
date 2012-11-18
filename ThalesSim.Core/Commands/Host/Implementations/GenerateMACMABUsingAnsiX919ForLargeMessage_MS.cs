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
using ThalesSim.Core.Cryptography.MAC;
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    /// <summary>
    /// Thales MS implementation.
    /// </summary>
    [ThalesHostCommand("MS", "MT", "Generate a MAC (MAB) using Ansi X9.19 for large message")]
    public class GenerateMACMABUsingAnsiX919ForLargeMessage_MS : AHostCommand
    {
        private const string MacSingleBlock = "0";
        private const string MacFirstBlock = "1";
        private const string MacMiddleBlock = "2";
        private const string MacLastBlock = "3";

        private const string InputBinary = "0";
        private const string InputHex = "1";

        private const string KeyTak = "0";
        private const string KeyZak = "1";

        private const string KeySingle = "0";
        private const string KeyDouble = "1";

        private const string Zeroes = "0000000000000000";

        private string _blockNumber;
        private string _keyType;
        private string _keyLength;
        private string _messageType;
        private string _key;
        private string _iv;
        private string _msgLength;
        private string _strMsg;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public GenerateMACMABUsingAnsiX919ForLargeMessage_MS()
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

            _blockNumber = KeyValues.Item("Message Block");
            _keyType = KeyValues.Item("Key Type");
            _keyLength = KeyValues.Item("Key Length");
            _messageType = KeyValues.Item("Message Type");
            _key = KeyValues.ItemCombination("Key Scheme", "Key");
            _iv = KeyValues.ItemOptional("IV");
            _msgLength = KeyValues.Item("Message Length");
            _strMsg = KeyValues.Item("Message");
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            var mr = new StreamResponse();

            var cryptKey = new HexKey(_key);
            if ((cryptKey.Length == KeyLength.SingleLength && _keyLength != KeySingle) ||
                (cryptKey.Length == KeyLength.DoubleLength && _keyLength != KeyDouble))
            {
                mr.Append(ErrorCodes.ER_04_INVALID_KEY_TYPE_CODE);
                return mr;
            }

            var len = Convert.ToInt32(_msgLength, 16);
            if (_messageType == InputHex)
            {
                len = len*2;
            }

            if (_strMsg.Length < len)
            {
                mr.Append(ErrorCodes.ER_80_DATA_LENGTH_ERROR);
                return mr;
            }

            var rawData = _messageType == InputHex
                              ? _strMsg.Substring(0, len)
                              : _strMsg.Substring(0, len).GetBytes().GetHexString();

            var clearKey = _keyType == KeyTak
                               ? new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair16_17), false, _key)
                               : new HexKeyThales(new KeyTypeCode(0, LmkPair.Pair26_27), false, _key);

            var block = IsoX919BlockType.FinalBlock;
            switch (_blockNumber)
            {
                case MacSingleBlock:
                    block = IsoX919BlockType.OnlyBlock;
                    _iv = Zeroes;
                    break;
                case MacFirstBlock:
                    block = IsoX919BlockType.FirstBlock;
                    _iv = Zeroes;
                    break;
                case MacMiddleBlock:
                    block = IsoX919BlockType.NextBlock;
                    break;
            }

            var mac = IsoX919Mac.MacHexData(rawData, clearKey.ClearHexKey, _iv, block);

            Log.InfoFormat("Key (clear): {0}", clearKey.ClearKey);
            Log.InfoFormat("IV: {0}", _iv);
            Log.InfoFormat("Resulting MAC: {0}", mac);

            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(mac);

            return mr;
        }
    }
}
