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

        Const MODE_FLAG As String = "MODE_FLAG"
        Const KEY_TYPE As String = "KEY_TYPE"
        Const KEY_SCHEME As String = "KEY_SCHEME"
        Const ZMK As String = "ZMK"

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
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(MODE_FLAG, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_TYPE, 3))
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_SCHEME, 1))

            Dim P_ZMK As MessageFieldParser = GenerateMultiKeyParser(ZMK)
            P_ZMK.DependentField = MODE_FLAG
            P_ZMK.DependentValue = "1"
            MFPC.AddMessageFieldParser(P_ZMK)

            Dim P_ZMK_Scheme As New MessageFieldParser(KEY_SCHEME_ZMK, 1)
            P_ZMK_Scheme.DependentField = MODE_FLAG
            P_ZMK_Scheme.DependentValue = "1"
            MFPC.AddMessageFieldParser(P_ZMK_Scheme)
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
            _modeFlag = MFPC.GetMessageFieldByName(MODE_FLAG).FieldValue
            _keyType = MFPC.GetMessageFieldByName(KEY_TYPE).FieldValue
            _keyScheme = MFPC.GetMessageFieldByName(KEY_SCHEME).FieldValue
            _zmk = MFPC.GetMessageFieldByName(ZMK).FieldValue
            _zmkScheme = MFPC.GetMessageFieldByName(KEY_SCHEME_ZMK).FieldValue
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
                clearZMK = Utility.DecryptEncryptedZMK(_zmk, ZMK, MFPC.GetMessageFieldByName(ZMK).DeterminerName)
                If Utility.IsParityOK(clearZMK, Utility.ParityCheck.OddParity) = False Then
                    mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                    Return mr
                End If

                cryptUnderZMK = Utility.EncryptUnderZMK(clearZMK, rndKey, zmk_ks)

                Log.Logger.MinorInfo("ZMK (clear): " + clearZMK)
                Log.Logger.MinorInfo("Key under ZMK: " + cryptUnderZMK)
            End If

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptRndKey)
            mr.AddElement(cryptUnderZMK)
            mr.AddElement(chkVal.Substring(0, 6))

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
