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
    ''' Generates a MAC for a large message.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the MQ Racal command.
    ''' </remarks>
    <ThalesCommandCode("MQ", "MR", "", "Generates a MAC for a large message")> _
    Public Class GenerateMACForLargeMessage_MQ
        Inherits AHostCommand

        Const BLOCK_NBR As String = "BLOCK_NBR"
        Const ZAKKEY As String = "ZAK"
        Const IV_1 As String = "IV_1"
        Const IV_2 As String = "IV_2"
        Const DATA As String = "DATA"
        Const MSG_LEN As String = "MSG_LEN"

        Private _blockNbr As String
        Private _zak As String
        Private _iv As String
        Private _msgLen As String
        Private _data() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the MQ message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(BLOCK_NBR, 1))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(ZAKKEY))
            Dim P_IV1 As New MessageFieldParser(IV_1, 16)
            P_IV1.DependentField = BLOCK_NBR
            P_IV1.DependentValue = "2"
            MFPC.AddMessageFieldParser(P_IV1)
            Dim P_IV2 As New MessageFieldParser(IV_2, 16)
            P_IV2.DependentField = BLOCK_NBR
            P_IV2.DependentValue = "3"
            MFPC.AddMessageFieldParser(P_IV2)
            MFPC.AddMessageFieldParser(New MessageFieldParser(MSG_LEN, 3))
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
            _blockNbr = MFPC.GetMessageFieldByName(BLOCK_NBR).FieldValue()
            _zak = MFPC.GetMessageFieldByName(ZAKKEY).FieldValue()
            If _blockNbr = "2" Then
                _iv = MFPC.GetMessageFieldByName(IV_1).FieldValue()
            ElseIf _blockNbr = "3" Then
                _iv = MFPC.GetMessageFieldByName(IV_2).FieldValue()
            Else
                _iv = ZEROES
            End If
            _msgLen = MFPC.GetMessageFieldByName(MSG_LEN).FieldValue()
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

            Dim zak As String = Utility.DecryptUnderLMK(_zak, ZAKKEY, MFPC.GetMessageFieldByName(ZAKKEY).DeterminerName, LMKPairs.LMKPair.Pair26_27, "0")
            If Utility.IsParityOK(zak, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim msgLen As Integer = CInt("&H" + _msgLen)
            If msgLen <> _data.GetLength(0) Then
                mr.AddElement(ErrorCodes._80_DATA_LENGTH_ERROR)
                Return mr
            End If

            Dim MAC As String = GenerateMAC(_data, zak, _iv)

            Log.Logger.MinorInfo("ZAK (clear): " + zak)
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

