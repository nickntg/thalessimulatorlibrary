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
    ''' Generates a 4-digit VISA PVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DG Racal command.
    ''' </remarks>
    <ThalesCommandCode("DG", "DH", "", "Generates a 4-digit VISA PVV")> _
    Public Class GenerateVISAPVV_DG
        Inherits AHostCommand

        Private _acct As String
        Private _pin As String
        Private _pvkPair As String
        Private _pvki As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DG message parsing fields.
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
                _acct = kvp.Item("Account Number")
                _pin = kvp.Item("PIN")
                _pvki = kvp.Item("PVKI")
                _pvkPair = kvp.ItemCombination("PVK Scheme", "PVK")
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
            Dim clearPVK As String = utility.DecryptUnderLMK(cryptPVK.ToString, cryptpvk.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearPIN As String = DecryptPINUnderHostStorage(_pin)

            If clearPIN.Length < 4 OrElse clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            If Utility.TryParseInt(_pvki) = False Then
                mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim PVV As String = Me.GeneratePVV(_acct, _pvki, clearPIN, clearPVK)

            Log.Logger.MinorInfo("Clear PVKs: " + clearPVK)
            Log.Logger.MinorInfo("Resulting PVV: " + PVV)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(PVV)

            Return mr

        End Function

    End Class

End Namespace
