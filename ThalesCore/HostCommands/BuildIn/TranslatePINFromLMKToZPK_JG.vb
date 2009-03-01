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
    ''' Translates a PIN from LMK to ZPK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the JG Racal command.
    ''' </remarks>
    <ThalesCommandCode("JG", "JH", "", "Translates a PIN from LMK to ZPK encryption")> _
    Public Class TranslatePINFromLMKToZPK_JG
        Inherits AHostCommand

        Const DESTINATION_ZPK As String = "DESTINATION_ZPK"
        Const PB_FORMAT As String = "PB_FORMAT"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"
        Const PIN As String = "PIN"

        Private _acct As String
        Private _targetKey As String
        Private _PBFormat As String
        Private _pin As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the JG message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(DESTINATION_ZPK))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PB_FORMAT, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, 12))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN, _
                                       CType(Core.Resources.GetResource(Core.Resources.CLEAR_PIN_LENGTH), Integer) + 1))
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
            _targetKey = MFPC.GetMessageFieldByName(DESTINATION_ZPK).FieldValue()
            _PBFormat = MFPC.GetMessageFieldByName(PB_FORMAT).FieldValue()
            _acct = MFPC.GetMessageFieldByName(ACCT_NBR).FieldValue()
            _pin = MFPC.GetMessageFieldByName(PIN).FieldValue()
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

            Dim clearZPK As String = DecryptUnderLMK(_targetKey, DESTINATION_ZPK, MFPC.GetMessageFieldByName(DESTINATION_ZPK).DeterminerName, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim PBFormat As PIN_Block_Format = ToPINBlockFormat(_PBFormat)
            If PBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPIN As String = DecryptPINUnderHostStorage(_pin)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim clearPB = Core.PIN.PINBlockFormat.ToPINBlock(clearPIN, _acct, PBFormat)
            Dim cryptPB As String = TripleDES.TripleDESEncrypt(New HexKey(clearZPK), clearPB)

            Log.Logger.MinorInfo("Clear ZPK: " + clearZPK)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Crypt PIN Block: " + cryptPB)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptPB)

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
