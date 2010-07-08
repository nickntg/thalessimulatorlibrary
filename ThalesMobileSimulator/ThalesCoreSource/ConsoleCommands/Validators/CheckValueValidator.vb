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
    ''' Validator that verifies a key check value.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class CheckValueValidator
        Implements IConsoleDataValidator

        Dim ignoreEmpty As Boolean

        ''' <summary>
        ''' Default class constructor.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            ignoreEmpty = False
        End Sub

        ''' <summary>
        ''' Constructor that allows to specify whether an empty check value is allowed.
        ''' </summary>
        ''' <param name="ignoreEmpty">Empty check value flag.</param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ignoreEmpty As Boolean)
            Me.ignoreEmpty = ignoreEmpty
        End Sub

        ''' <summary>
        ''' Determines whether a check value appears correct.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the check value appears to be incorrect, the validator 
        ''' throws a <see cref="Exceptions.XInvalidCheckValue"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            If ignoreEmpty AndAlso consoleMsg = "" Then Return

            If consoleMsg.Length <> 6 OrElse Utility.IsHexString(consoleMsg) = False Then
                Throw New Exceptions.XInvalidCheckValue("INVALID CHECK VALUE")
            End If
        End Sub
    End Class

End Namespace
