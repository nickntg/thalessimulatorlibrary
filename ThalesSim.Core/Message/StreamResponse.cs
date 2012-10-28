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

using ThalesSim.Core.Utility;

namespace ThalesSim.Core.Message
{
    /// <summary>
    /// Holds a response message.
    /// </summary>
    public class StreamResponse
    {
        /// <summary>
        /// Response message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Appends a string at the end of the message.
        /// </summary>
        /// <param name="str">String to append.</param>
        public void Append (string str)
        {
            Message += str;
        }

        /// <summary>
        /// Appends a string at the front of the message.
        /// </summary>
        /// <param name="str">String to append.</param>
        public void AppendFront (string str)
        {
            Message = str + Message;
        }

        /// <summary>
        /// Get bytes corresponding to the message.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return Message.GetBytes();
        }
    }
}
