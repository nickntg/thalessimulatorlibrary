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
    ''' Verifies a terminal PIN using the VISA algorithm.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DC Racal command.
    ''' </remarks>
    <ThalesCommandCode("DC", "DD", "", "Verifies a terminal PIN using the VISA algorithm")> _
    Public Class VerifyTerminalPINWithVISAAlgorithm_DC
        Inherits AHostCommand

        Private _acct As String
        Private _pinBlock As String
        Private _pinBlockFormat As String
        Private _pvkPair As String
        Private _pvki As String
        Private _tpk As String
        Private _pvv As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DC message parsing fields.
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
                _tpk = kvp.ItemCombination("TPK Scheme", "TPK")
                _pvkPair = kvp.ItemCombination("PVK Scheme", "PVK")
                _pinBlock = kvp.Item("PIN Block")
                _pinBlockFormat = kvp.Item("PIN Block Format Code")
                _acct = kvp.Item("Account Number")
                _pvki = kvp.Item("PVKI")
                _pvv = kvp.Item("PVV")
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

            Dim cryptTPK As New HexKey(_tpk)
            Dim clearTPK As String = Utility.DecryptUnderLMK(cryptTPK.ToString, cryptTPK.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearTPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearTPK), _pinBlock)
            Dim PBFormat As PIN_Block_Format = PIN.PINBlockFormat.ToPINBlockFormat(_pinBlockFormat)
            If PBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPIN As String = PIN.PINBlockFormat.ToPIN(clearPB, _acct, PBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            If Integer.TryParse(_pvki, Nothing) = False Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim PVV As String = Me.GeneratePVV(_acct, _pvki, clearPIN, clearPVK)

            Log.Logger.MinorInfo("Clear PVKs: " + clearPVK)
            Log.Logger.MinorInfo("Clear TPK: " + clearTPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            If _pvv = PVV Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
            End If

            Return mr

        End Function

    End Class

End Namespace
