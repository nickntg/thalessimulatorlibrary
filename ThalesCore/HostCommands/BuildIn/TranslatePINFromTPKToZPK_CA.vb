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
    ''' Translates a PIN block from TPK to ZPK encryption.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CA Racal command.
    ''' </remarks>
    <ThalesCommandCode("CA", "CB", "", "Translates a PIN block from TPK to ZPK encryption")> _
    Public Class TranslatePINFromTPKToZPK_CA
        Inherits AHostCommand

        Private _acct As String
        Private _sourceKey As String
        Private _targetKey As String
        Private _pb As String
        Private _sourcePBFormat As String
        Private _targetPBFormat As String
        Private _maxPinLen As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CA message parsing fields.
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
                _sourceKey = kvp.ItemCombination("Source TPK Scheme", "Source TPK")
                _targetKey = kvp.ItemCombination("Destination ZPK Scheme", "Destination ZPK")
                _maxPinLen = kvp.Item("PIN Length")
                _pb = kvp.Item("Source PIN Block")
                _sourcePBFormat = kvp.Item("Source PIN Block Format")
                _targetPBFormat = kvp.Item("Target PIN Block Format")
                _acct = kvp.Item("Account Number")
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

            Dim cryptTPK As New HexKey(_sourceKey)
            Dim cryptZPK As New HexKey(_targetKey)

            Dim clearTPK As String = Utility.DecryptUnderLMK(cryptTPK.ToString, cryptTPK.Scheme, LMKPairs.LMKPair.Pair14_15, "0") 'Utility.DecryptUnderLMK(_sourceKey, SOURCE_TPK, MFPC.GetMessageFieldByName(SOURCE_TPK).DeterminerName, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearTPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearZPK As String = Utility.DecryptUnderLMK(cryptZPK.ToString, cryptZPK.Scheme, LMKPairs.LMKPair.Pair06_07, "0") 'Utility.DecryptUnderLMK(_targetKey, TARGET_ZPK, MFPC.GetMessageFieldByName(TARGET_ZPK).DeterminerName, LMKPairs.LMKPair.Pair06_07, "0")
            If Utility.IsParityOK(clearZPK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim sourcePBFormat As PIN_Block_Format = ToPINBlockFormat(_sourcePBFormat)
            If sourcePBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim targetPBFormat As PIN_Block_Format = ToPINBlockFormat(_targetPBFormat)
            If targetPBFormat = PIN_Block_Format.InvalidPBCode Then
                mr.AddElement(ErrorCodes._23_INVALID_PIN_BLOCK_FORMAT_CODE)
                Return mr
            End If

            Dim clearPB As String = TripleDES.TripleDESDecrypt(New HexKey(clearTPK), _pb)
            Dim clearPIN As String = ToPIN(clearPB, _acct, sourcePBFormat)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim newPB As String = ToPINBlock(clearPIN, _acct, targetPBFormat)
            Dim cryptPB As String = TripleDES.TripleDESEncrypt(New HexKey(clearZPK), newPB)

            Log.Logger.MinorInfo("Clear TPK: " + clearTPK)
            Log.Logger.MinorInfo("Clear ZPK: " + clearZPK)
            Log.Logger.MinorInfo("Clear PIN Block: " + clearPB)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)
            Log.Logger.MinorInfo("New clear PIN Block: " + newPB)
            Log.Logger.MinorInfo("New crypt PIN Block: " + cryptPB)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(clearPIN.Length.ToString().PadLeft(2, "0"c))
            mr.AddElement(cryptPB)
            mr.AddElement(_targetPBFormat)

            Return mr

        End Function

    End Class

End Namespace
