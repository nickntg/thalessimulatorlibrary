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

Namespace ConsoleCommands.Validators

    ''' <summary>
    ''' Validator that verifies a key type string.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class KeyTypeValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Determines whether a correct key type has been entered.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the console message is incorrect, the validator 
        ''' throws a <see cref="Exceptions.XInvalidKeyType"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            KeyTypeTable.ParseKeyTypeCode(consoleMsg, Nothing, Nothing)
        End Sub
    End Class

End Namespace
