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
    ''' This validator verifies that a user enters S, D or T.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class DataLengthValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Determines whether the user enters S, D or T.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the enter key is not valid, the validator 
        ''' throws a <see cref="Exceptions.XInvalidData"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            'Slight modification to allow the use of lower-case characters.
            consoleMsg = consoleMsg.ToUpper

            If consoleMsg <> "S" AndAlso consoleMsg <> "D" AndAlso consoleMsg <> "T" Then
                Throw New Exceptions.XInvalidData("INVALID LENGTH")
            End If
        End Sub
    End Class

End Namespace
