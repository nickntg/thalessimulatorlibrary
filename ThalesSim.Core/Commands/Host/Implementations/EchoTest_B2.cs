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
using ThalesSim.Core.Message;
using ThalesSim.Core.Resources;

namespace ThalesSim.Core.Commands.Host.Implementations
{
    [ThalesHostCommand("B2", "B3", "Echo received data back to the user")]
    public class EchoTest_B2 : AHostCommand
    {
        private int _dataLength;
        private string _data;

        /// <summary>
        /// Read XML definitions on instantiation.
        /// </summary>
        public EchoTest_B2()
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

            _dataLength = Convert.ToInt32(KeyValues.Item("Length"), 16);

            try
            {
                if (message.CharsLeft < _dataLength)
                {
                    XmlParseResult = ErrorCodes.ER_80_DATA_LENGTH_ERROR;
                }
                else if (message.CharsLeft > _dataLength)
                {
                    XmlParseResult = ErrorCodes.ER_15_INVALID_INPUT_DATA;
                }
                else
                {
                    _data = message.Substring(_dataLength);
                }
            }
            catch (Exception)
            {
                XmlParseResult = ErrorCodes.ER_80_DATA_LENGTH_ERROR;
            }
        }

        /// <summary>
        /// Process request.
        /// </summary>
        /// <returns>Response message.</returns>
        public override StreamResponse ConstructResponse()
        {
            Log.InfoFormat("Echo back {0}", _data);

            var mr = new StreamResponse();
            mr.Append(ErrorCodes.ER_00_NO_ERROR);
            mr.Append(_data);
            return mr;
        }
    }
}
