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
    ''' Verifies and translates MAC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the ME Racal command.
    ''' </remarks>
    <ThalesCommandCode("ME", "MF", "", "Verifies and translates a MAC")> _
    Public Class VerifyAndTranslateMAC_ME
        Inherits AHostCommand

        Const SOURCE_TAK As String = "SOURCE_TAK"
        Const TARGET_TAK As String = "TARGET_TAK"
        Const M_A_C As String = "M_A_C"
        Const DATA As String = "DATA"

        Private _sourceTak As String
        Private _targetTak As String
        Private _mac As String
        Private _data() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the ME message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(SOURCE_TAK))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TARGET_TAK))
            MFPC.AddMessageFieldParser(New MessageFieldParser(M_A_C, 8))
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
            _sourceTak = MFPC.GetMessageFieldByName(SOURCE_TAK).FieldValue()
            _targetTak = MFPC.GetMessageFieldByName(TARGET_TAK).FieldValue()
            _mac = MFPC.GetMessageFieldByName(M_A_C).FieldValue()
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

            Dim sourceTAK As String = DecryptUnderLMK(_sourceTak, SOURCE_TAK, MFPC.GetMessageFieldByName(SOURCE_TAK).DeterminerName, LMKPairs.LMKPair.Pair16_17, "0")
            If Utility.IsParityOK(sourceTAK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim targetTAK As String = DecryptUnderLMK(_targetTak, TARGET_TAK, MFPC.GetMessageFieldByName(TARGET_TAK).DeterminerName, LMKPairs.LMKPair.Pair16_17, "0")
            If Utility.IsParityOK(targetTAK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim NewMAC As String = GenerateMAC(_data, sourceTAK, ZEROES).Substring(0, 8)
            Dim TransMAC As String = GenerateMAC(_data, targetTAK, ZEROES).Substring(0, 8)

            Log.Logger.MinorInfo("Source TAK (clear): " + sourceTAK)
            Log.Logger.MinorInfo("Target TAK (clear): " + targetTAK)
            Log.Logger.MinorInfo("Resulting MAC: " + NewMAC)
            Log.Logger.MinorInfo("Passed MAC: " + _mac)
            Log.Logger.MinorInfo("Translated MAC: " + TransMAC)

            If _mac = NewMAC Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
                mr.AddElement(TransMAC)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
            End If

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
