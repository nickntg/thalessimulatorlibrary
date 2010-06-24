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
    ''' Verifies a Visa PVV and generates a new PVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CU Racal command.
    ''' </remarks>
    <ThalesCommandCode("CU", "CV", "", "Verifies a PVV and generates a new PVV")> _
    Public Class VerifyAndGenerateVISAPVV_CU
        Inherits AHostCommand

        Private _keyType As String
        Private _acct As String
        Private _pinBlock As String
        Private _pinBlockFormat As String
        Private _pvkPair As String
        Private _pvki As String
        Private _sourceKey As String
        Private _pvv As String
        Private _newPINBlock As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CU message parsing fields.
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
                _keyType = kvp.Item("Key Type")
                _sourceKey = kvp.ItemCombination("Key Scheme", "Key")
                _pvkPair = kvp.ItemCombination("PVK Scheme", "PVK")
                _pinBlock = kvp.Item("PIN Block")
                _pinBlockFormat = kvp.Item("PIN Block Format Code")
                _acct = kvp.Item("Account Number")
                _pvki = kvp.Item("PVKI")
                _pvv = kvp.Item("PVV")
                _newPINBlock = kvp.Item("New PIN Block")
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

            Dim cryptPVK As New HexKey(_pvkPair)
            Dim clearPVK As String = Utility.DecryptUnderLMK(cryptPVK.ToString, cryptPVK.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim cryptKey As New HexKey(_sourceKey), clearKey As String
            If _keyType = "001" Then
                clearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair06_07, "0")
            Else
                clearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            End If
            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim PBFormat As PIN_Block_Format = PIN.PINBlockFormat.ToPINBlockFormat(_pinBlockFormat)
            If PBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearKey), _pinBlock)
            Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            If Integer.TryParse(_pvki, 16) = False Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim PVV As String = GeneratePVV(_acct, _pvki, clearPIN, clearPVK)

            Log.Logger.MinorInfo("Clear PVKs: " + clearPVK)
            Log.Logger.MinorInfo("Clear Key: " + clearKey)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            If _pvv = PVV Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)

                Dim newClearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearKey), _newPINBlock)
                Dim newClearPIN As String = PIN.PINBlockFormat.ToPIN(newClearPB, _acct, PBFormat)
                Dim newPVV As String = Me.GeneratePVV(_acct, _pvki, newClearPIN, clearPVK)

                Log.Logger.MinorInfo("Clear New PIN Block: " + newClearPB)
                Log.Logger.MinorInfo("Clear New PIN: " + newClearPIN)
                Log.Logger.MinorInfo("New PVV: " + newPVV)

                mr.AddElement(newPVV)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
                Return mr
            End If

            Return mr

        End Function

    End Class

End Namespace