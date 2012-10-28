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
using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Message
{
    /// <summary>
    /// Used to hold an incoming request message and contains
    /// some helper methods to assist in message parsing.
    /// </summary>
    public class StreamMessage
    {
        private byte[] _data;

        private string _sdata;

        /// <summary>
        /// Returns all of the message data.
        /// </summary>
        public string MessageData { get { return _data.GetString(); } }

        /// <summary>
        /// Get/set the message index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Get the number of characters left in the message.
        /// </summary>
        public int CharsLeft { get { return _data.Length - Index; } }

        /// <summary>
        /// Creates a new instance of this class
        /// from a message string.
        /// </summary>
        /// <param name="data">String with message.</param>
        public StreamMessage (string data)
        {
            _data = data.GetBytes();
            _sdata = data;
        }

        /// <summary>
        /// Creates a new instance of this class
        /// from a byte array message.
        /// </summary>
        /// <param name="data">Byte array with message.</param>
        public StreamMessage (byte[] data)
        {
            _data = data;
            _sdata = data.GetString();
        }

        /// <summary>
        /// Gets a set of the characters that
        /// follow in the message.
        /// </summary>
        /// <param name="length">Number of characters to get.</param>
        /// <returns>String with characters.</returns>
        public string Substring (int length)
        {
            return _sdata.Substring(Index, length);
        }

        /// <summary>
        /// Returns a byte array with the remaining
        /// unparsed bytes of the message.
        /// </summary>
        /// <returns>Byte array.</returns>
        public byte[] GetRemainingBytes()
        {
            return GetRemainingBytes(Index);
        }

        /// <summary>
        /// Gets a message trailer.
        /// </summary>
        /// <returns>String with message trailer
        /// including the trailer character.</returns>
        public string GetTrailer()
        {
            for (var i = _data.Length-1; i >= 0; i-=1)
            {
                if (_data[i] == 0x19)
                {
                    var trailer = GetRemainingBytes(i);

                    var dataNew = new byte[i];
                    Array.Copy(_data, 0, dataNew, 0, i);
                    _data = dataNew;
                    _sdata = _data.GetString();

                    return trailer.GetString();
                }
            }

            return string.Empty;
        }

        private byte[] GetRemainingBytes(int index)
        {
            var bb = new byte[_data.Length - index];
            Array.Copy(_data, index, bb, 0, _data.Length - index);
            return bb;
        }
    }
}
