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

namespace ThalesSim.Core.Commands.Host
{
    public class HostCommand : Command
    {
        public string ResponseCode { get; set; }

        public string ResponseCodeAfterIo { get; set; }

        public override string ToString()
        {
            return
                string.Format(
                    "Command: {0}\r\nResponse: {1}\r\nResponse(I/O): {2}\r\nDescription: {3}\r\nType: {4}\r\nDeclared at: {5}\r\nAssembly: {6}",
                    Code, ResponseCode, ResponseCodeAfterIo, Description, Type, DeclaringType, Assembly.GetName());
        }
    }
}
