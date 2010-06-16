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
    ''' Verifies a VISA CVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CY Racal command.
    ''' </remarks>
    <ThalesCommandCode("CY", "CZ", "", "Verifies a VISA CVV")> _
    Public Class VerifyVISACVV_CY
        Inherits AHostCommand

        Private _cvv As String
        Private _acct As String
        Private _cvkPair As String
        Private _expiration As String
        Private _svc As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CY message parsing fields.
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
                _cvkPair = kvp.ItemCombination("CVK Scheme", "CVK")
                _cvv = kvp.Item("CVV")
                _acct = kvp.Item("Account Number")
                _expiration = kvp.Item("Expiration Date")
                _svc = kvp.Item("Service Code")
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

            Dim cryptCVK As New HexKey(_cvkPair)
            Dim clearCVK As String =  Utility.DecryptUnderLMK(cryptCVK.ToString,cryptcvk.Scheme,LMKPairs.LMKPair.Pair14_15,"4")
            If Utility.IsParityOK(clearCVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim NewCVV As String = GenerateCVV(clearCVK, _acct, _expiration, _svc)

            Log.Logger.MinorInfo("Clear CVK: " + clearCVK)
            Log.Logger.MinorInfo("Resulting CVV: " + NewCVV)
            Log.Logger.MinorInfo("Passed CVV: " + _cvv)

            If NewCVV = _cvv Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
            End If

            Return mr

        End Function

    End Class

End Namespace

