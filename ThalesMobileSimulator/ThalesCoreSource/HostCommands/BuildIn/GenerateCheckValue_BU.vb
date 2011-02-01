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

        Private _key As String = ""
        Private _keyTypeCode As String = ""
        Private _keyLengthFlag As String
        Private _keyType As String
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
            ReadXMLDefinitions()
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                _keyTypeCode = kvp.Item("Key Type Code")
                _keyLengthFlag = kvp.Item("Key Length Flag")
                _key = kvp.ItemCombination("Key Scheme", "Key")
                _keyType = kvp.ItemOptional("Key Type")
                _checkValueType = kvp.ItemOptional("Key Check Value Type")
            End If
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

            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""

            If _keyTypeCode = "FF" Then
                'Per http://thalessim.codeplex.com/workitem/8145, need to be more careful
                'how var is passed by reference.
                If Me.ValidateKeyTypeCode(_keyType, LMKKeyPair, var, mr) = False Then Return mr
            Else
                'http://thalessim.codeplex.com/workitem/8145
                Dim intVar As Integer
                Core.LMKPairs.LMKTypeCodeToLMKPair(_keyTypeCode, LMKKeyPair, intVar)
                var = intVar.ToString

                If LMKKeyPair < LMKPairs.LMKPair.Pair00_01 Then
                    mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
                    Return mr
                End If
            End If

            Dim hk As New HexKey(_key)
            If hk.KeyLen = HexKey.KeyLength.SingleLength AndAlso _keyLengthFlag <> "0" Then
                mr.AddElement(ErrorCodes.ER_05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            ElseIf hk.KeyLen = HexKey.KeyLength.DoubleLength AndAlso _keyLengthFlag <> "1" Then
                mr.AddElement(ErrorCodes.ER_05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            ElseIf hk.KeyLen = HexKey.KeyLength.TripleLength AndAlso _keyLengthFlag <> "2" Then
                mr.AddElement(ErrorCodes.ER_05_INVALID_KEY_LENGTH_FLAG)
                Return mr
            End If

            Dim cryptKey As New HexKey(_key)
            Dim clearKey As String = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKKeyPair, var)
            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Check value: " + checkValue)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            If _checkValueType = "1" Then
                mr.AddElement(checkValue.Substring(0, 6))
            Else
                mr.AddElement(checkValue)
            End If

            Return mr

        End Function

    End Class

End Namespace

