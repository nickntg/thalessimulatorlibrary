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
    ''' Verifies a terminal PIN using the VISA algorithm.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DC Racal command.
    ''' </remarks>
    <ThalesCommandCode("DC", "DE", "", "Verifies a terminal PIN using the VISA algorithm")> _
    Public Class VerifyTerminalPINWithVISAAlgorithm_DC
        Inherits AHostCommand

        Const TPK As String = "TPK"
        Const PVK_PAIR As String = "PVK_PAIR"
        Const PIN_BLOCK As String = "PIN_BLOCK"
        Const PINBLOCKFORMAT As String = "PIN_BLOCK_FORMAT"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"
        Const PVKI As String = "PVKI"
        Const PVV As String = "PVV"

        Private _acct As String
        Private _pinBlock As String
        Private _pinBlockFormat As String
        Private _pvkPair As String
        Private _pvki As String
        Private _tpk As String
        Private _pvv As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DC message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TPK))
            MFPC.AddMessageFieldParser(GeneratePVKKeyParser(PVK_PAIR))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_BLOCK, 16))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PINBLOCKFORMAT, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, 12))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PVKI, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PVV, 4))
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
            _pinBlock = MFPC.GetMessageFieldByName(PIN_BLOCK).FieldValue()
            _pinBlockFormat = MFPC.GetMessageFieldByName(PINBLOCKFORMAT).FieldValue()
            _pvki = MFPC.GetMessageFieldByName(PVKI).FieldValue()
            _pvkPair = MFPC.GetMessageFieldByName(PVK_PAIR).FieldValue()
            _tpk = MFPC.GetMessageFieldByName(TPK).FieldValue()
            _pvv = MFPC.GetMessageFieldByName(PVV).FieldValue()
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

            Dim clearPVK As String = Utility.DecryptUnderLMK(_pvkPair, PVK_PAIR, MFPC.GetMessageFieldByName(PVK_PAIR).DeterminerName, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearTPK As String = Utility.DecryptUnderLMK(_tpk, TPK, MFPC.GetMessageFieldByName(TPK).DeterminerName, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearTPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearTPK), _pinBlock)
            Dim PBFormat As PIN_Block_Format = PIN.PINBlockFormat.ToPINBlockFormat(_pinBlockFormat)
            If PBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PBFormat)

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
            Log.Logger.MinorInfo("Clear TPK: " + clearTPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            If _pvv = PVV Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
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
