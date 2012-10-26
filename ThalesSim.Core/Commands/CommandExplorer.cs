﻿/*
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
using System.Collections.Generic;
using System.Text;
using ThalesSim.Core.Commands.Console;
using ThalesSim.Core.Commands.Host;

namespace ThalesSim.Core.Commands
{
    public class CommandExplorer
    {
        private static readonly SortedList<CommandType, SortedList<string, Command>> Commands =
            new SortedList<CommandType, SortedList<string, Command>>();

        public static void Discover()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in assemblies)
            {
                foreach (var t in asm.GetTypes())
                {
                    foreach (var attr in t.GetCustomAttributes(false))
                    {
                        if (attr.GetType() == typeof(ThalesConsoleCommandAttribute))
                        {
                            var consoleAttr = (ThalesConsoleCommandAttribute) attr;
                            var consoleCommand = new Command
                                                     {
                                                         Code = consoleAttr.CommandCode,
                                                         DeclaringType = t,
                                                         Assembly = asm,
                                                         Description = consoleAttr.Description,
                                                         Type = CommandType.Console
                                                     };
                            if (!Commands.ContainsKey(CommandType.Console))
                            {
                                Commands.Add(CommandType.Console, new SortedList<string, Command>());
                            }

                            try
                            {
                                Commands[CommandType.Console].Add(consoleCommand.Code, consoleCommand);
                            }
                            catch (ArgumentException)
                            {
                                // Nothing, unit tests.
                            }
                        }

                        if (attr.GetType() == typeof(ThalesHostCommandAttribute))
                        {
                            var hostAttr = (ThalesHostCommandAttribute) attr;
                            var hostCommand = new HostCommand
                                                  {
                                                      Assembly = asm,
                                                      Code = hostAttr.CommandCode,
                                                      DeclaringType = t,
                                                      Description = hostAttr.Description,
                                                      ResponseCode = hostAttr.ResponseCode,
                                                      ResponseCodeAfterIo = hostAttr.ResponseCodeAfterIo,
                                                      Type = CommandType.Host
                                                  };

                            if (!Commands.ContainsKey(CommandType.Host))
                            {
                                Commands.Add(CommandType.Host, new SortedList<string, Command>());
                            }

                            try
                            {
                                Commands[CommandType.Host].Add(hostCommand.Code, hostCommand);
                            }
                            catch (ArgumentException)
                            {
                                // Nothing, unit tests.
                            }
                            
                        }
                    }
                }
            }
        }

        public static string GetLoadedCommandDescriptions (CommandType commandType)
        {
            var sb = new StringBuilder();
            foreach (var key in Commands[commandType].Keys)
            {
                sb.Append(Commands[commandType][key].ToString());
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static Command GetCommand (CommandType commandType, string code)
        {
            try
            {
                return Commands[commandType][code];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
    }
}
