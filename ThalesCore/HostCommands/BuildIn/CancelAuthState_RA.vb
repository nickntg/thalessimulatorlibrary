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

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Cancels the authorized state.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the RA Racal command.
    ''' </remarks>
    <ThalesCommandCode("RA", "RB", "", "Cancel the authorized state")> _
    Public Class CancelAuthState_RA
        Inherits AHostCommand

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the RA message parsing fields (so it does nothing).
        ''' </remarks>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As Message.MessageResponse
            Dim mr As New MessageResponse
            If CType(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE), Boolean) = True Then
                Log.Logger.MajorInfo("Exiting from AUTHORIZED state")
            Else
                Log.Logger.MajorInfo("Already out of the AUTHORIZED state")
            End If
            Core.Resources.UpdateResource(Core.Resources.AUTHORIZED_STATE, False)
            mr.AddElement(ErrorCodes._00_NO_ERROR)
            Return mr
        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            Return Nothing
        End Function
    End Class

End Namespace
