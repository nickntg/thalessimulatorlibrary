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
    ''' This validator verifies that the user enters a correct number of components.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class NumberOfComponentsValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Verifies that the user enters a correct number of components.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the number of components is not between 2 and 9, the validator 
        ''' throws a <see cref="Exceptions.XInvalidNumberOfComponents"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            Try
                Dim nbr As Integer = Convert.ToInt32(consoleMsg)
                If nbr < 2 OrElse nbr > 9 Then
                    Throw New Exceptions.XInvalidNumberOfComponents("INVALID NUMBER OF COMPONENTS")
                End If
            Catch ex As Exception
                Throw New Exceptions.XInvalidNumberOfComponents("INVALID NUMBER OF COMPONENTS")
            End Try
        End Sub
    End Class

End Namespace
