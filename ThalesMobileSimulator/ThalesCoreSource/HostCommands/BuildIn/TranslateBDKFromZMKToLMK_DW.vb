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
    ''' Translates a BDK from ZMK to LMK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DW Thales command.
    ''' </remarks>
    <ThalesCommandCode("DW", "DX", "", "Translates a BDK from ZMK to LMK encryption")> _
    Public Class TranslateBDKFromZMKToLMK_DW
        Inherits AHostCommand

        Private _zmk As String
        Private _bdk As String
        Private _keySchemeLMK As String
        Private _keyCheckValueType As String
        Private _del As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DW message parsing fields.
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
                _zmk = kvp.ItemCombination("ZMK Scheme", "ZMK")
                _bdk = kvp.ItemCombination("BDK Scheme", "BDK")
                _del = kvp.ItemOptional("Delimiter")
                _keySchemeLMK = kvp.ItemOptional("Key Scheme LMK")
                _keyCheckValueType = kvp.ItemOptional("Key Check Value Type")
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

            Dim LMKks As KeySchemeTable.KeyScheme

            If _del = DELIMITER_VALUE Then
                If ValidateKeySchemeCode(_keySchemeLMK, LMKks, mr) = False Then Return mr
            Else
                LMKks = KeySchemeTable.KeyScheme.Unspecified
                _keyCheckValueType = "0"
            End If

            Dim cryptZMK As New HexKey(_zmk)
            Dim clearZMK As String = Utility.DecryptUnderLMK(cryptZMK.ToString, cryptZMK.Scheme, LMKPairs.LMKPair.Pair04_05, "0")
            If Utility.IsParityOK(clearZMK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim cryptBDK As New HexKey(_bdk)
            Dim clearBDK As String = DecryptUnderZMK(Utility.RemoveKeyType(clearZMK), cryptBDK.ToString, KeySchemeTable.KeyScheme.Unspecified)
            If Utility.IsParityOK(clearBDK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim BDKUnderLMK As String = Utility.EncryptUnderLMK(clearBDK, LMKks, LMKPairs.LMKPair.Pair28_29, "0")

            Dim checkValue As String = ""
            Select Case _keyCheckValueType
                Case "0", "2"
                    Dim chk1 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearBDK).Substring(0, 16)), ZEROES)
                    Dim chk2 As String = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearBDK).Substring(16)), ZEROES)
                    Log.Logger.MinorInfo("CVK-A check value: " + chk1)
                    Log.Logger.MinorInfo("CVK-B check value: " + chk2)
                    checkValue = chk1 + chk2
                Case "1"
                    checkValue = TripleDES.TripleDESEncrypt(New HexKey(Utility.RemoveKeyType(clearBDK)), ZEROES).Substring(0, 6)
                    Log.Logger.MinorInfo("CVK check value: " + checkValue)
            End Select

            Log.Logger.MinorInfo("Clear ZMK: " + clearZMK)
            Log.Logger.MinorInfo("Clear BDK: " + clearBDK)
            Log.Logger.MinorInfo("BDK Under LMK: " + BDKUnderLMK)
            Log.Logger.MinorInfo("Check Value: " + checkValue)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(BDKUnderLMK)
            mr.AddElement(checkValue)

            Return mr

        End Function

    End Class

End Namespace