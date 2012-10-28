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
using System.Reflection;

namespace ThalesSim.Core.Commands
{
    /// <summary>
    /// This class represents the basic information of a host or console command.
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Get/set the command code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Get/set the command description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Get/set the command type.
        /// </summary>
        public CommandType Type { get; set; }

        /// <summary>
        /// Get/set the type that implements the command.
        /// </summary>
        public Type DeclaringType { get; set; }

        /// <summary>
        /// Get/set the assembly containing the command type.
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// Returns a string representing this instance.
        /// </summary>
        /// <returns>String representation of this instance.</returns>
        public override string ToString()
        {
            return string.Format("Command: {0}\r\nDescription: {1}\r\nType: {2}\r\nDeclared at: {3}\r\nAssembly: {4}",
                                 Code, Description, Type, DeclaringType, Assembly.GetName());
        }
    }
}
