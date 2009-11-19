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
    ''' This validator verifies whether the simulator is in the authorized state.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class AuthorizedStateValidator
        Implements IConsoleDataValidator

        ''' <summary>
        ''' Determines whether the simulator is in the authorized state.
        ''' </summary>
        ''' <param name="consoleMsg">Console message to validate.</param>
        ''' <remarks>If the simulator is not in the authorized state, the validator 
        ''' throws a <see cref="Exceptions.XNeedsAuthorizedState"/> exception.</remarks>
        Public Sub ValidateConsoleMessage(ByVal consoleMsg As String) Implements IConsoleDataValidator.ValidateConsoleMessage
            If Not CType(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE), Boolean) Then
                Throw New Exceptions.XNeedsAuthorizedState("NOT AUTHORIZED")
            End If
        End Sub
    End Class

End Namespace
