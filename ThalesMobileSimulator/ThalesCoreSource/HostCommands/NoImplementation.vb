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
Imports ThalesSim.Core.Message

Namespace HostCommands

    ''' <summary>
    ''' Host command class that performs nothing.
    ''' </summary>
    ''' <remarks>
    ''' This class is inherited by commands that do not perform any processing.
    ''' </remarks>
    Public MustInherit Class NoImplementation
        Inherits AHostCommand

        ''' <summary>
        ''' Related I/O flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to True to indicate that the implemented command performs printer I/O.
        ''' </remarks>
        Protected HasRelatedIO As Boolean = False

        ''' <summary>
        ''' Authorized state flag.
        ''' </summary>
        ''' <remarks>
        ''' Set this to True to indicate that the implemented command needs the authorized state.
        ''' </remarks>
        Protected NeedsAuthorizedState As Boolean = False

        ''' <summary>
        ''' Processing result.
        ''' </summary>
        ''' <remarks>
        ''' Set this to anything other than "OK" to indicate processing failure. This results
        ''' in the <see cref="HostCommands.NoImplementation.ConstructResponseAfterOperationComplete"/> method returning Nothing.
        ''' </remarks>
        Protected Result As String = "OK"

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the message parsing fields.
        ''' </remarks>
        Public Sub New()
            InitFields()
        End Sub

        ''' <summary>
        ''' Initialization method.
        ''' </summary>
        ''' <remarks>
        ''' This method must be overriden to provide specific implementation of the 
        ''' authorized mode and related I/O flags.
        ''' </remarks>
        Public MustOverride Sub InitFields()

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
            If NeedsAuthorizedState = True Then
                If IsInAuthorizedState() Then
                    mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                Else
                    Result = "Failed"
                    mr.AddElement(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                End If
            Else
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            End If
            Return mr
        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> if the previous processing failed.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            If HasRelatedIO = True Then
                If Result = "OK" Then
                    Dim mr As New MessageResponse
                    mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                    Return mr
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function
    End Class

End Namespace

