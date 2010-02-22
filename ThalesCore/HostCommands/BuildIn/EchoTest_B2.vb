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

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Echoes a command back to the caller.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesCommandCode("B2", "B3", "", "Echo received data back to the user")> _
    Public Class EchoTest_B2
        Inherits AHostCommand

        Const DATA_LENGTH As String = "DATA_LENGTH"

        Private _dataLength As String
        Private _data As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the B2 message parsing fields.
        ''' </remarks>
        Public Sub New()

            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(DATA_LENGTH, 4))

        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)

            Dim CE As CommandExplorer = New HostCommands.CommandExplorer

            MFPC.ParseMessage(msg)

            ' Get's the message hex length
            _dataLength = MFPC.GetMessageFieldByName(DATA_LENGTH).FieldValue()
            Dim iDataLength As Integer = CInt("&H" + _dataLength)

            Try
                _data = msg.GetSubstring(iDataLength)
            Catch ex As Exception
                'Currently assuming that an error means that a short message was send.
                Throw New Exceptions.XShortMessage("Short message")
            End Try

        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As MessageResponse

            Dim mr As New MessageResponse

            ' Includes B2 Error Code
            mr.AddElement(ErrorCodes._00_NO_ERROR)
            ' Includes the message echoed back
            mr.AddElement(_data)

            Return mr

        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As MessageResponse
            Return Nothing
        End Function

    End Class
End Namespace
