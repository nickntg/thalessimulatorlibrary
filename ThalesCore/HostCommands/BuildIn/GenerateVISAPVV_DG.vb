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
    ''' Generates a 4-digit VISA PVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DG Racal command.
    ''' </remarks>
    <ThalesCommandCode("DG", "DH", "", "Generates a 4-digit VISA PVV")> _
    Public Class GenerateVISAPVV_DG
        Inherits AHostCommand

        Const PVK_PAIR As String = "PVK_PAIR"
        Const PIN As String = "PIN"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"
        Const PVKI As String = "PVKI"

        Private _acct As String
        Private _pin As String
        Private _pvkPair As String
        Private _pvki As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DG message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GeneratePVKKeyParser(PVK_PAIR))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN, _
                           CType(Core.Resources.GetResource(Core.Resources.CLEAR_PIN_LENGTH), Integer) + 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, 12))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PVKI, 1))
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
            _acct = MFPC.GetMessageFieldByName(ACCT_NBR).FieldValue()
            _pin = MFPC.GetMessageFieldByName(PIN).FieldValue()
            _pvki = MFPC.GetMessageFieldByName(PVKI).FieldValue()
            _pvkPair = MFPC.GetMessageFieldByName(PVK_PAIR).FieldValue()
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

            Dim clearPVK As String = DecryptUnderLMK(_pvkPair, PVK_PAIR, MFPC.GetMessageFieldByName(PVK_PAIR).DeterminerName, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearPIN As String = DecryptPINUnderHostStorage(_pin)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            If IsNumeric(_pvki) = False Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim PVV As String = Me.GeneratePVV(_acct, _pvki, clearPIN, clearPVK)

            Log.Logger.MinorInfo("Clear PVKs: " + clearPVK)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(PVV)

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
