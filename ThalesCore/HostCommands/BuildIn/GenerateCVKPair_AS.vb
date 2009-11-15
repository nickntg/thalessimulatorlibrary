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
    ''' Generates a VISA CVK pair.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the AS Racal command.
    ''' </remarks>
    <ThalesCommandCode("AS", "AT", "", "Generates a VISA CVK pair.")> _
    Public Class GenerateCVKPair_AS
        Inherits AHostCommand

        Private _del As String
        Private _keySchemeZMK As String
        Private _keySchemeLMK As String
        Private _keyCheckValue As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the AS message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection

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

            Dim ks As KeySchemeTable.KeyScheme, zmkKs As KeySchemeTable.KeyScheme
            Dim useKs As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, ks, mr) = False Then Return mr
                If ValidateKeySchemeCode(_keySchemeZMK, zmkKs, mr) = False Then Return mr

                If ks <> KeySchemeTable.KeyScheme.DoubleLengthKeyVariant AndAlso ks <> KeySchemeTable.KeyScheme.TripleLengthKeyVariant AndAlso ks <> KeySchemeTable.KeyScheme.Unspecified Then
                    mr.AddElement(ErrorCodes._26_INVALID_KEY_SCHEME)
                    Return mr
                End If

                If ks = KeySchemeTable.KeyScheme.Unspecified Then
                    useKs = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                Else
                    useKs = ks
                End If

            Else
                ks = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                _keyCheckValue = "0"
                useKs = KeySchemeTable.KeyScheme.Unspecified
            End If

            Dim clearKeyA As String = Utility.CreateRandomKey(KeySchemeTable.KeyScheme.SingleDESKey)
            Dim clearKeyB As String = Utility.CreateRandomKey(KeySchemeTable.KeyScheme.SingleDESKey)
            Dim cryptKeyLMK As String = Utility.EncryptUnderLMK(clearKeyA + clearKeyB, useKs, LMKPairs.LMKPair.Pair14_15, "4")

            Log.Logger.MinorInfo("CVK (clear): " + clearKeyA + clearKeyB)
            Log.Logger.MinorInfo("CVK (LMK): " + cryptKeyLMK)

            mr.AddElement(ErrorCodes._00_NO_ERROR)

            If useKs = KeySchemeTable.KeyScheme.Unspecified Then
                mr.AddElement(Utility.RemoveKeyType(cryptKeyLMK))
            Else
                mr.AddElement(cryptKeyLMK)
            End If

            If _keyCheckValue = "0" Then
                Dim chkVal1 As String = TripleDES.TripleDESEncrypt(New HexKey(clearKeyA), ZEROES)
                Dim chkVal2 As String = TripleDES.TripleDESEncrypt(New HexKey(clearKeyB), ZEROES)
                Log.Logger.MinorInfo("CVK-A Check Value: " + chkVal1.Substring(0, 6))
                Log.Logger.MinorInfo("CVK-B Check Value: " + chkVal2.Substring(0, 6))
                mr.AddElement(chkVal1.Substring(0, 6))
                mr.AddElement(chkVal2.Substring(0, 6))
            Else
                Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(clearKeyA + clearKeyB), ZEROES)
                Log.Logger.MinorInfo("CVK Check Value: " + chkVal.Substring(0, 6))
                mr.AddElement(chkVal.Substring(0, 6))
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
