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
    ''' Translates a PIN block from ZPK to ZPK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CC Racal command.
    ''' </remarks>
    <ThalesCommandCode("CC", "CD", "", "Translates a PIN block from ZPK to ZPK encryption")> _
    Public Class TranslatePINFromZPKToZPK_CC
        Inherits AHostCommand

        Const SOURCE_ZPK As String = "SOURCE_ZPK"
        Const TARGET_ZPK As String = "TARGET_ZPK"
        Const MAX_PIN_LEN As String = "MAX_PIN_LEN"
        Const PIN_BLOCK As String = "PIN_BLOCK"
        Const SOURCE_PB_FORMAT As String = "SOURCE_PB_FORMAT"
        Const TARGET_PB_FORMAT As String = "TARGET_PB_FORMAT"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"

        Private _acct As String
        Private _sourceKey As String
        Private _targetKey As String
        Private _pb As String
        Private _sourcePBFormat As String
        Private _targetPBFormat As String
        Private _maxPinLen As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CC message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(SOURCE_ZPK))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(TARGET_ZPK))
            MFPC.AddMessageFieldParser(New MessageFieldParser(MAX_PIN_LEN, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_BLOCK, 16))
            MFPC.AddMessageFieldParser(New MessageFieldParser(SOURCE_PB_FORMAT, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(TARGET_PB_FORMAT, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, 12))
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
            _sourceKey = MFPC.GetMessageFieldByName(SOURCE_ZPK).FieldValue()
            _targetKey = MFPC.GetMessageFieldByName(TARGET_ZPK).FieldValue()
            _maxPinLen = MFPC.GetMessageFieldByName(MAX_PIN_LEN).FieldValue()
            _pb = MFPC.GetMessageFieldByName(PIN_BLOCK).FieldValue()
            _sourcePBFormat = MFPC.GetMessageFieldByName(SOURCE_PB_FORMAT).FieldValue()
            _targetPBFormat = MFPC.GetMessageFieldByName(TARGET_PB_FORMAT).FieldValue()
            _acct = MFPC.GetMessageFieldByName(ACCT_NBR).FieldValue()
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

            If _maxPinLen <> "12" Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim clearZPK1 As String = Utility.DecryptUnderLMK(_sourceKey, SOURCE_ZPK, MFPC.GetMessageFieldByName(SOURCE_ZPK).DeterminerName, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK1, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearZPK2 As String = Utility.DecryptUnderLMK(_targetKey, TARGET_ZPK, MFPC.GetMessageFieldByName(TARGET_ZPK).DeterminerName, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK2, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim sourcePBFormat As PIN_Block_Format = ToPINBlockFormat(_sourcePBFormat)
            If sourcePBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim targetPBFormat As PIN_Block_Format = ToPINBlockFormat(_targetPBFormat)
            If targetPBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearZPK1), _pb)
            Dim clearPIN As String = ToPIN(clearPB, _acct, sourcePBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim newPB As String = ToPINBlock(clearPIN, _acct, targetPBFormat)
            Dim cryptPB As String = TripleDES.TripleDESEncrypt(New HexKey(clearZPK2), newPB)

            Log.Logger.MinorInfo("Clear source ZPK: " + clearZPK1)
            Log.Logger.MinorInfo("Clear target ZPK: " + clearZPK2)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("New clear PIN Block: " + newPB)
            Log.Logger.MinorInfo("New crypt PIN Block: " + cryptPB)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(clearPIN.Length.ToString().PadLeft(2, "0"c))
            mr.AddElement(cryptPB)
            mr.AddElement(_targetPBFormat)

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

