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
    public class StreamMessage
    {
        private byte[] _data;

        private string _sdata;

        public string MessageData { get { return _data.GetString(); } }

        public int Index { get; set; }

        public int CharsLeft { get { return _data.Length - Index; } }

        public StreamMessage (string data)
        {
            _data = data.GetBytes();
            _sdata = data;
        }

        public StreamMessage (byte[] data)
        {
            _data = data;
            _sdata = data.GetString();
        }

        public string Substring (int length)
        {
            return _sdata.Substring(Index, length);
        }

        public byte[] GetRemainingBytes()
        {
            return GetRemainingBytes(Index);
        }

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
