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
    ''' This validator verifies that the user enters a correct component type.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ComponentTypeValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Determines whether a correct component type is enter (X,H,E).
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the component type is incorrect, the validator 
        ''' throws a <see cref="Exceptions.XInvalidComponentType"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            If consoleMsg = "S" Then
                Throw New Exceptions.XInvalidComponentType("INPUT FROM SMART CARDS NOT SUPPORTED")
            ElseIf consoleMsg <> "X" AndAlso consoleMsg <> "H" AndAlso consoleMsg <> "E" Then
                Throw New Exceptions.XInvalidComponentType("INVALID COMPONENT TYPE")
            End If
        End Sub
    End Class

End Namespace
