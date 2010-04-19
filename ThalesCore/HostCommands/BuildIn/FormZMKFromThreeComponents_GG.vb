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
    ''' Form a ZMK from three encrypted components.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the GG Racal command.
    ''' </remarks>
    <ThalesCommandCode("GG", "GH", "", "Form a ZMK from three encrypted components")> _
    Public Class FormZMKFromThreeComponents_GG
        Inherits AHostCommand

        Const COMPONENT_A As String = "COMPONENT_A"
        Const COMPONENT_B As String = "COMPONENT_B"
        Const COMPONENT_C As String = "COMPONENT_C"

        Private _keyA As String
        Private _keyB As String
        Private _keyC As String
        Private _del As String
        Private _keySchemeZMK As String
        Private _keySchemeLMK As String
        Private _keyCheckValue As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the GG message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection

            MFPC.AddMessageFieldParser(GenerateZMKKeyParser(COMPONENT_A, 90))
            MFPC.AddMessageFieldParser(GenerateZMKKeyParser(COMPONENT_B, 60))
            MFPC.AddMessageFieldParser(GenerateZMKKeyParser(COMPONENT_C, 30))

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
            _keyA = MFPC.GetMessageFieldByName(COMPONENT_A).FieldValue
            _keyB = MFPC.GetMessageFieldByName(COMPONENT_B).FieldValue
            _keyC = MFPC.GetMessageFieldByName(COMPONENT_C).FieldValue
            _del = MFPC.GetMessageFieldByName(DELIMITER).FieldValue
            _keySchemeZMK = MFPC.GetMessageFieldByName(KEY_SCHEME_ZMK).FieldValue
            _keySchemeLMK = MFPC.GetMessageFieldByName(KEY_SCHEME_LMK).FieldValue
            _keyCheckValue = MFPC.GetMessageFieldByName(KEY_CHECK_VALUE).FieldValue
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

            If Not IsInAuthorizedState() Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes._17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            Dim ks As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, ks, mr) = False Then Return mr
                If _keySchemeZMK <> "0" Then
                    mr.AddElement(ErrorCodes._26_INVALID_KEY_SCHEME)
                    Return mr
                End If
                If ks = KeySchemeTable.KeyScheme.TripleLengthKeyAnsi OrElse _
                   ks = KeySchemeTable.KeyScheme.TripleLengthKeyVariant Then
                    mr.AddElement(ErrorCodes._26_INVALID_KEY_SCHEME)
                    Return mr
                End If
            Else
                If MFPC.GetMessageFieldByName(COMPONENT_A).DeterminerName = COMPONENT_A + PLAIN_DOUBLE Then
                    ks = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                Else
                    ks = KeySchemeTable.KeyScheme.SingleDESKey
                End If
                _keyCheckValue = "0"
            End If

            Dim clearA As String, clearB As String, clearC As String, clearKey As String

            clearA = Utility.DecryptUnderLMK(_keyA, COMPONENT_A, MFPC.GetMessageFieldByName(COMPONENT_A).DeterminerName, LMKPairs.LMKPair.Pair04_05, "0")
            If Utility.IsParityOK(clearA, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            clearB = Utility.DecryptUnderLMK(_keyB, COMPONENT_B, MFPC.GetMessageFieldByName(COMPONENT_B).DeterminerName, LMKPairs.LMKPair.Pair04_05, "0")
            If Utility.IsParityOK(clearB, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            clearC = Utility.DecryptUnderLMK(_keyC, COMPONENT_C, MFPC.GetMessageFieldByName(COMPONENT_C).DeterminerName, LMKPairs.LMKPair.Pair04_05, "0")
            If Utility.IsParityOK(clearC, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            clearKey = Utility.XORHexStringsFull(Utility.XORHexStringsFull(clearA, clearB), clearC)

            Dim cryptKey As String = Utility.EncryptUnderLMK(clearKey, ks, LMKPairs.LMKPair.Pair04_05, "0")
            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            Log.Logger.MinorInfo("Component A (clear): " + Utility.RemoveKeyType(clearA))
            Log.Logger.MinorInfo("Component B (clear): " + Utility.RemoveKeyType(clearB))
            Log.Logger.MinorInfo("Component C (clear): " + Utility.RemoveKeyType(clearC))
            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Key (LMK): " + cryptKey)
            If _keyCheckValue = "0" Then
                Log.Logger.MinorInfo("Check value: " + checkValue)
            Else
                Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))
            End If

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptKey)
            If _keyCheckValue = "0" Then
                mr.AddElement(checkValue)
            Else
                mr.AddElement(checkValue.Substring(0, 6))
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
