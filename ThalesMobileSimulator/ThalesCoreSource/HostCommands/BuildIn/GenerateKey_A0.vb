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
    ''' Generates and encrypts key under ZMK for transmission.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the A0 Racal command.
    ''' </remarks>
    <ThalesCommandCode("A0", "A1", "", "Generates and encrypts key under ZMK for transmission")> _
    Public Class GenerateKey_A0
        Inherits AHostCommand

        Private _modeFlag As String = ""
        Private _keyType As String = ""
        Private _keyScheme As String = ""
        Private _zmk As String = ""
        Private _zmkScheme As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the A0 message parsing fields.
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
                _modeFlag = kvp.Item("Mode")
                _keyType = kvp.Item("Key Type")
                _keyScheme = kvp.Item("Key Scheme LMK")
                _zmkScheme = kvp.ItemOptional("Key Scheme ZMK")
                _zmk = kvp.ItemOptional("ZMK Scheme") + kvp.ItemOptional("ZMK")
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
            Dim ks As KeySchemeTable.KeyScheme, zmk_ks As KeySchemeTable.KeyScheme

            If ValidateKeyTypeCode(_keyType, LMKKeyPair, var, mr) = False Then Return mr
            If ValidateKeySchemeCode(_keyScheme, ks, mr) = False Then Return mr

            If _zmkScheme <> "" Then
                If ValidateKeySchemeCode(_zmkScheme, zmk_ks, mr) = False Then Return mr
            End If

            If ValidateFunctionRequirement(KeyTypeTable.KeyFunction.Generate, LMKKeyPair, var, mr) = False Then Return mr

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKKeyPair, var)
            Dim chkVal As String = TripleDES.TripleDESEncrypt(New HexKey(rndKey), ZEROES)

            Log.Logger.MinorInfo("Key generated (clear): " + rndKey)
            Log.Logger.MinorInfo("Key generated (LMK): " + cryptRndKey)
            Log.Logger.MinorInfo("Check value: " + chkVal.Substring(0, 6))

            Dim clearZMK As String, cryptUnderZMK As String = ""

            If _zmk <> "" Then
                Dim cryptZMK As New HexKey(_zmk)
                clearZMK = Utility.DecryptZMKEncryptedUnderLMK(cryptZMK.ToString, cryptZMK.Scheme, 0)
                If Utility.IsParityOK(clearZMK, Utility.ParityCheck.OddParity) = False Then
                    mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                    Return mr
                End If

                cryptUnderZMK = Utility.EncryptUnderZMK(clearZMK, rndKey, zmk_ks)

                Log.Logger.MinorInfo("ZMK (clear): " + clearZMK)
                Log.Logger.MinorInfo("Key under ZMK: " + cryptUnderZMK)
            End If

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptRndKey)
            mr.AddElement(cryptUnderZMK)
            mr.AddElement(chkVal.Substring(0, 6))

            Return mr

        End Function

    End Class

End Namespace
