''
'' This program is free software; you can redistribute it and/or modify
'' it under the terms of the GNU General Public License as published by
'' the Free Software Foundation; either version 2 of the License, or
'' (at your option) any later version.
''
'' This program is distributed in the hope that it will be useful,
'' but WITHOUT ANY WARRANTY; without even the implied warranty of
'' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'' GNU General Public License for more details.
''
'' You should have received a copy of the GNU General Public License
'' along with this program; if not, write to the Free Software
'' Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
'' 

Namespace ConsoleCommands

    ''' <summary>
    ''' Interface that should be implemented by classes that validate a console message.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface IConsoleDataValidator

        ''' <summary>
        ''' This method validates a console message.
        ''' Implementors should throw an exception to indicate an error.
        ''' </summary>
        ''' <param name="consoleMsg"></param>
        ''' <remarks></remarks>
        Sub ValidateConsoleMessage(ByVal consoleMsg As String)

    End Interface

End Namespace