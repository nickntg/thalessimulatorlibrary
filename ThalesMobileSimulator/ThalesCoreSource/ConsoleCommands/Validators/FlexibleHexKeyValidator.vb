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
    ''' This validator makes certain a user can enter a proper hex key.
    ''' 
    '''     - Clear hex key (16, 32 or 48 hex characters).
    '''     - Half key (8 hex characters)
    '''     - Variant/ANSI double or triple length keys.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FlexibleHexKeyValidator
        Implements IConsoleDataValidator

        Dim ignoreEmptyKey As Boolean

        ''' <summary>
        ''' Default class constructor.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            ignoreEmptyKey = False
        End Sub

        ''' <summary>
        ''' Constructor that allows to specify whether a hexadecimal key can be empty.
        ''' </summary>
        ''' <param name="ignoreEmptyKey">Empty key flag.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ignoreEmptyKey As Boolean)
            Me.ignoreEmptyKey = ignoreEmptyKey
        End Sub

        ''' <summary>
        ''' Determines whether a hexadecimal key is constructed properly.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the enter key is not a proper hexadecimal key, the validator 
        ''' throws a <see cref="Exceptions.XInvalidKey"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            'Slight modification to allow the use of lower-case hex characters.
            consoleMsg = consoleMsg.ToUpper

            'Check for empty key
            If ignoreEmptyKey AndAlso consoleMsg = "" Then Return

            'The HexKey class does what we want, except accepting an 8-hex key.
            If Utility.IsHexString(consoleMsg) = False OrElse (consoleMsg.Length <> 8) Then
                Try
                    Dim key As New Core.Cryptography.HexKey(consoleMsg)
                Catch ex As Exception
                    Throw New Exceptions.XInvalidKey("INVALID KEY")
                End Try
            End If
        End Sub
    End Class

End Namespace
