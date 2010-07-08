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

Imports ThalesSim.Core

Namespace ConsoleCommands

    ''' <summary>
    ''' Cancel the authorized state command.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesConsoleCommandCode("C", "Cancels the authorized state")> _
    Public Class CancelAuthorizedState_C
        Inherits AConsoleCommand

        ''' <summary>
        ''' No stack, since this is an immediate command.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub InitializeStack()
        End Sub

        ''' <summary>
        ''' Cancels the authorized state.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ProcessMessage() As String
            Log.Logger.MajorInfo("Canceling authorized state")
            Core.Resources.UpdateResource(Core.Resources.AUTHORIZED_STATE, False)
            Return "NOT AUTHORIZED"
        End Function

    End Class

End Namespace