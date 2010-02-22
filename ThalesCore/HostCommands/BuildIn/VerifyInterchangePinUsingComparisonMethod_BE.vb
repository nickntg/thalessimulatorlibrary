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

Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core.Message
Imports ThalesSim.Core.PIN.PINBlockFormat

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Verifies a PIN using the comparison method.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesCommandCode("BE", "BF", "", "Verify a PIN received from interchange by comparing it with a value held on the Host database")> _
    Public Class VerifyInterchangePinUsingComparisonMethod_BE
        Inherits AHostCommand

        Const ZPK As String = "ZPK"
        Const PIN_BLOCK As String = "PIN_BLOCK"
        Const PIN_BLOCK_FORMAT As String = "PIN_BLOCK_FORMAT"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"
        Const PIN_HOST_STORAGE As String = "PIN_HOST_STORAGE"

        Private _zpk As String
        Private _pinBlock As String
        Private _pinBlockFormat As String
        Private _acct As String
        Private _pinDatabase As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the BE message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(ZPK))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_BLOCK, 16))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_BLOCK_FORMAT, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, 12))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_HOST_STORAGE, 5))
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
            _zpk = MFPC.GetMessageFieldByName(ZPK).FieldValue()
            _pinBlock = MFPC.GetMessageFieldByName(PIN_BLOCK).FieldValue()
            _pinBlockFormat = MFPC.GetMessageFieldByName(PIN_BLOCK_FORMAT).FieldValue()
            _acct = MFPC.GetMessageFieldByName(ACCT_NBR).FieldValue()
            _pinDatabase = MFPC.GetMessageFieldByName(PIN_HOST_STORAGE).FieldValue()
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

            ' Get's clear ZPK
            Dim clearZPK As String = Utility.DecryptUnderLMK(_zpk, ZPK, MFPC.GetMessageFieldByName(ZPK).DeterminerName, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            ' Get's clear Pinblock
            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearZPK), _pinBlock)
            Dim PBFormat As PIN_Block_Format = PIN.PINBlockFormat.ToPINBlockFormat(_pinBlockFormat)
            If PBFormat = PIN.PINBlockFormat.PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            ' Get's clear Pin
            Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Log.Logger.MinorInfo("Clear ZPK: " + clearZPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)

            'Decrypt's pin under host storage
            Dim clearDBPIN As String = DecryptPINUnderHostStorage(_pinDatabase)

            If clearDBPIN = clearPIN Then
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
