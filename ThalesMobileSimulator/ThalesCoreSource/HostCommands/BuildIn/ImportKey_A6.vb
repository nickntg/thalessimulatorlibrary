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
    ''' Imports a key encrypted under a ZMK.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the A6 Racal command.
    ''' </remarks>
    <ThalesCommandCode("A6", "A7", "", "Imports a key encrypted under a ZMK")> _
    Public Class ImportKey_A6
        Inherits AHostCommand

        Private _keyType As String = ""
        Private _zmk As String = ""
        Private _key As String = ""
        Private _lmkScheme As String = ""
        Private _targetKeyScheme As KeySchemeTable.KeyScheme

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the A6 message parsing fields.
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
                _keyType = kvp.Item("Key Type")
                _zmk = kvp.ItemCombination("ZMK Scheme", "ZMK")
                _key = kvp.ItemCombination("Key Scheme", "Key")
                _lmkScheme = kvp.Item("Key Scheme LMK")
                If kvp.ItemOptional("Key Scheme") <> "" Then
                    _targetKeyScheme = KeySchemeTable.GetKeySchemeFromValue(kvp.ItemOptional("Key Scheme"))
                Else
                    _targetKeyScheme = KeySchemeTable.KeyScheme.Unspecified
                End If
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
            Dim _warnParity As Boolean = False

            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim lmk_ks As KeySchemeTable.KeyScheme

            If ValidateKeyTypeCode(_keyType, LMKKeyPair, var, mr) = False Then Return mr
            If ValidateKeySchemeCode(_lmkScheme, lmk_ks, mr) = False Then Return mr

            If ValidateFunctionRequirement(KeyTypeTable.KeyFunction.Export, LMKKeyPair, var, mr) = False Then Return mr

            Dim cryptZMK As New HexKey(_zmk)
            Dim clearZMK As String = Utility.DecryptUnderLMK(cryptZMK.ToString, cryptZMK.Scheme, LMKPairs.LMKPair.Pair04_05, var)
            If Utility.IsParityOK(clearZMK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            'Changed to use the DecryptUnderZMK method instead of directly performing 3DES decrypt.
            'See http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=217215.
            Dim clearKey As String = DecryptUnderZMK(clearZMK, Utility.RemoveKeyType(_key), _targetKeyScheme)
            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                _warnParity = True
            End If

            Dim cryptKey As String = Utility.EncryptUnderLMK(Utility.RemoveKeyType(clearKey), lmk_ks, LMKKeyPair, var)
            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            Log.Logger.MinorInfo("ZMK (clear): " + clearZMK)
            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Key (LMK): " + cryptKey)
            Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptKey)
            mr.AddElement(checkValue.Substring(0, 6))

            Return mr

        End Function

    End Class

End Namespace

