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
    ''' Translates a PIN from TPK to LMK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the JC Racal command.
    ''' </remarks>
    <ThalesCommandCode("JC", "JD", "", "Translates a PIN from TPK to LMK encryption")> _
    Public Class TranslatePINFromTPKToLMK_JC
        Inherits AHostCommand

        Const SOURCE_TPK As String = "SOURCE_TPK"
        Const PIN_BLOCK As String = "PIN_BLOCK"
        Const PB_FORMAT As String = "SOURCE_PB_FORMAT"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"

        Private _acct As String
        Private _sourceKey As String
        Private _pb As String
        Private _PBFormat As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the JC message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(SOURCE_TPK))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_BLOCK, 16))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PB_FORMAT, 2))
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
            _sourceKey = MFPC.GetMessageFieldByName(SOURCE_TPK).FieldValue()
            _pb = MFPC.GetMessageFieldByName(PIN_BLOCK).FieldValue()
            _PBFormat = MFPC.GetMessageFieldByName(PB_FORMAT).FieldValue()
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

            Dim clearTPK As String = DecryptUnderLMK(_sourceKey, SOURCE_TPK, MFPC.GetMessageFieldByName(SOURCE_TPK).DeterminerName, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearTPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim sourcePBFormat As PIN_Block_Format = ToPINBlockFormat(_PBFormat)
            If sourcePBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearTPK), _pb)
            Dim clearPIN As String = ToPIN(clearPB, _acct, sourcePBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim cryptPIN As String = EncryptPINForHostStorage(clearPIN)

            Log.Logger.MinorInfo("Clear TPK: " + clearTPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("New crypt PIN: " + cryptPIN)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptPIN)

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
