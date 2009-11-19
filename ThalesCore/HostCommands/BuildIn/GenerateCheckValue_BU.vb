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

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generate a key check value.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the BU Racal command.
    ''' </remarks>
    <ThalesCommandCode("BU", "BV", "", "Generate a key check value")> _
    Public Class GenerateCheckValue_BU
        Inherits AHostCommand

        Const KEY_TYPE_CODE As String = "KEY_TYPE_CODE"
        Const KEY As String = "KEY"
        Const KEY_LENGTH_FLAG As String = "KEY_LENGTH_FLAG"
        Const KEY_TYPE As String = "KEY_TYPE"

        Const DELIMITER2 As String = "DELIMITER2"
        Const DELIMITER_EXISTS2 As String = "DEL_EXISTS2"
        Const DELIMITER_NOT_EXISTS2 As String = "NODEL2"

        Private _key As String = ""
        Private _keyTypeCode As String = ""
        Private _keyLengthFlag As String
        Private _keyType As String
        Private _zmkScheme As String = ""
        Private _lmkScheme As String = ""
        Private _checkValueType As String = ""
        Private _delimiter As String = ""
        Private _delimiter2 As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the BU message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection

            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_TYPE_CODE, 2))
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_LENGTH_FLAG, 1))
            MFPC.AddMessageFieldParser(GenerateMultiKeyParser(KEY))

            Dim MFDC_Del As New MessageFieldDeterminerCollection
            MFDC_Del.AddFieldDeterminer(New MessageFieldDeterminer(DELIMITER_EXISTS2, 1, 1))
            MFDC_Del.AddFieldDeterminer(New MessageFieldDeterminer(DELIMITER_NOT_EXISTS2, "", 0))
            Dim P_Del As MessageFieldParser = New MessageFieldParser(DELIMITER2, MFDC_Del)
            MFPC.AddMessageFieldParser(P_Del)

            Dim P_KeyType As New MessageFieldParser(KEY_TYPE, 3)
            P_KeyType.DependentField = DELIMITER2
            P_KeyType.DependentValue = DELIMITER_VALUE
            MFPC.AddMessageFieldParser(P_KeyType)

            GenerateDelimiterParser()

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
            _keyTypeCode = MFPC.GetMessageFieldByName(KEY_TYPE_CODE).FieldValue
            _key = MFPC.GetMessageFieldByName(KEY).FieldValue
            _keyLengthFlag = MFPC.GetMessageFieldByName(KEY_LENGTH_FLAG).FieldValue
            _delimiter = MFPC.GetMessageFieldByName(DELIMITER2).FieldValue
            _keyType = MFPC.GetMessageFieldByName(KEY_TYPE).FieldValue
            _delimiter2 = MFPC.GetMessageFieldByName(DELIMITER).FieldValue
            _zmkScheme = MFPC.GetMessageFieldByName(KEY_SCHEME_ZMK).FieldName
            _lmkScheme = MFPC.GetMessageFieldByName(KEY_SCHEME_LMK).FieldValue
            _checkValueType = MFPC.GetMessageFieldByName(KEY_CHECK_VALUE).FieldValue
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

            Dim LMKKeyPair As LMKPairs.LMKPair, var As Integer

            If _keyTypeCode = "FF" Then
                If Me.ValidateKeyTypeCode(_keyType, LMKKeyPair, var.ToString, mr) = False Then Return mr
            Else
                Core.LMKPairs.LMKTypeCodeToLMKPair(_keyTypeCode, LMKKeyPair, var)
                If LMKKeyPair < LMKPairs.LMKPair.Pair00_01 Then
                    mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                    Return mr
                End If
            End If

            Dim hk As New HexKey(_key)
            If hk.KeyLen = HexKey.KeyLength.SingleLength AndAlso _keyLengthFlag <> "0" Then
                mr.AddElement(ErrorCodes._05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            ElseIf hk.KeyLen = HexKey.KeyLength.DoubleLength AndAlso _keyLengthFlag <> "1" Then
                mr.AddElement(ErrorCodes._05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            ElseIf hk.KeyLen = HexKey.KeyLength.TripleLength AndAlso _keyLengthFlag <> "2" Then
                mr.AddElement(ErrorCodes._05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            End If

            Dim clearKey As String = Utility.DecryptUnderLMK(_key, KEY, MFPC.GetMessageFieldByName(KEY).DeterminerName, LMKKeyPair, var.ToString())
            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Check value: " + checkValue)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            If _checkValueType = "1" Then
                mr.AddElement(checkValue.Substring(0, 6))
            Else
                mr.AddElement(checkValue)
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

