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
Imports ThalesSim.Core.PIN.PINBlockFormat

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generates a MAC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the MA Racal command.
    ''' </remarks>
    <ThalesCommandCode("MA", "MB", "", "Generates a MAC")> _
    Public Class GenerateMAC_MA
        Inherits AHostCommand

        Const TAK As String = "TAK"
        Const DATA As String = "DATA"

        Private _tak As String
        Private _data() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the MA message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TAK))
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            MFPC.ParseMessage(msg)
            _tak = MFPC.GetMessageFieldByName(TAK).FieldValue()
            _data = msg.GetRemainingBytes()
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

            Dim clearTAK As String = Utility.DecryptUnderLMK(_tak, TAK, MFPC.GetMessageFieldByName(TAK).DeterminerName, LMKPairs.LMKPair.Pair16_17, "0")
            If Utility.IsParityOK(clearTAK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim MAC As String = GenerateMAC(_data, clearTAK, ZEROES).Substring(0, 8)

            Log.Logger.MinorInfo("Clear TAK: " + clearTAK)
            Log.Logger.MinorInfo("Resulting MAC: " + MAC)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(MAC)

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
