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
    ''' This validator makes certain a string is 16, 32 or 48 hex characters long.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class HexKeyValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Determines whether a key is properly formatted (16, 32 or 48 hex characters long)
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the enter key is not a proper hexadecimal key, the validator 
        ''' throws a <see cref="Exceptions.XInvalidKey"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            'Slight modification to allow the use of lower-case hex characters.
            consoleMsg = consoleMsg.ToUpper
            If Utility.IsHexString(consoleMsg) = False OrElse (consoleMsg.Length <> 16 AndAlso consoleMsg.Length <> 32 AndAlso consoleMsg.Length <> 48) Then
                Throw New Exceptions.XInvalidKey("INVALID")
            End If
        End Sub
    End Class

End Namespace
