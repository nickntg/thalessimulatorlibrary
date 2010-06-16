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
    ''' Generates a ZEK or ZAK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the FI Racal command.
    ''' </remarks>
    <ThalesCommandCode("FI", "FJ", "", "Generates a ZEK or ZAK")> _
    Public Class GenerateZEKorZAK_FI
        Inherits AHostCommand

        Private _cmdFlag As String
        Private _sourceZmk As String
        Private _del As String
        Private _keySchemeZMK As String
        Private _keySchemeLMK As String
        Private _keyCheckValue As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the FI message parsing fields.
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
            If XMLParseResult = ErrorCodes._00_NO_ERROR Then
                _cmdFlag = kvp.Item("Flag")
                _sourceZmk = kvp.ItemCombination("ZMK Scheme", "ZMK")
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeZMK = kvp.ItemOptional("Key Scheme ZMK")
                _keySchemeLMK = kvp.ItemOptional("Key Scheme LMK")
                _keyCheckValue = kvp.ItemOptional("Key Check Value Type")
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

            Dim ks As KeySchemeTable.KeyScheme, zmkKs As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, ks, mr) = False Then Return mr
                If ValidateKeySchemeCode(_keySchemeZMK, zmkKs, mr) = False Then Return mr
            Else
                ks = KeySchemeTable.KeyScheme.SingleDESKey
                zmkKs = KeySchemeTable.KeyScheme.SingleDESKey
                _keyCheckValue = "0"
            End If

            Dim clearSource As String

            Dim cryptSource As New HexKey(_sourceZmk)
            clearSource = Utility.DecryptUnderLMK(cryptSource.ToString, cryptSource.Scheme, LMKPairs.LMKPair.Pair04_05, "0")
            If Utility.IsParityOK(clearSource, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearKey As String = Utility.MakeParity(Utility.CreateRandomKey(ks), Utility.ParityCheck.OddParity)
            Dim cryptKeyZMK As String = Utility.EncryptUnderZMK(clearSource, clearKey, zmkKs)
            Dim cryptKeyLMK As String
            If _cmdFlag = "0" Then
                cryptKeyLMK = Utility.EncryptUnderLMK(clearKey, ks, LMKPairs.LMKPair.Pair30_31, "0")
            Else
                cryptKeyLMK = Utility.EncryptUnderLMK(clearKey, ks, LMKPairs.LMKPair.Pair26_27, "0")
            End If

            Dim chkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            If _keyCheckValue = "1" Then
                chkValue = chkValue.Substring(0, 6)
            End If

            Log.Logger.MinorInfo("ZMK (clear): " + clearSource)
            Log.Logger.MinorInfo("ZEK/ZAK (clear): " + clearKey)
            Log.Logger.MinorInfo("ZEK/ZAK (ZMK): " + cryptKeyZMK)
            Log.Logger.MinorInfo("ZEK/ZAK (LMK): " + cryptKeyLMK)
            Log.Logger.MinorInfo("Check Value: " + chkValue)

            mr.AddElement(ErrorCodes._00_NO_ERROR)

            mr.AddElement(cryptKeyZMK)
            mr.AddElement(cryptKeyLMK)
            mr.AddElement(chkValue)

            Return mr

        End Function

    End Class

End Namespace
