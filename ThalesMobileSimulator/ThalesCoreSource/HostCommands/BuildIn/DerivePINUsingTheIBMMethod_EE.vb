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
    ''' Derives a PIN using the IBM method.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the EE Thales command.
    ''' </remarks>
    <ThalesCommandCode("EE", "EF", "", "Derive a PIN using the IBM method.")> _
    Public Class DerivePINUsingTheIBMMethod_EE
        Inherits AHostCommand

        Private _pvkPair As String
        Private _offsetValue As String
        Private _checkLen As String
        Private _acct As String
        Private _decTable As String
        Private _pinValData As String

        Dim _cryptAcctNum As String
        Dim _decimalisedAcctNum As String
        Dim _naturalPin As String
        Dim _derivedPin As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the EE message parsing fields.
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
                _pvkPair = kvp.ItemCombination("PVK Scheme", "PVK")
                _offsetValue = kvp.Item("Offset")
                _checkLen = kvp.Item("Check Length")
                _acct = kvp.Item("Account Number")
                _decTable = kvp.Item("Decimalisation Table")
                _pinValData = kvp.Item("PIN Validation Data")
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
                mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                Return mr
            End If

            If Convert.ToInt32(_checkLen) < 4 Then
                mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim _expPinValData As String

            _expPinValData = _pinValData.Substring(0, _pinValData.IndexOf("N"))
            _expPinValData = _expPinValData + _acct.Substring(_acct.Length - 5, 5)
            _expPinValData = _expPinValData + _pinValData.Substring(_pinValData.IndexOf("N") + 1, (_pinValData.Length - (_pinValData.IndexOf("N") + 1)))
            'The PVK is a double-length key, so we need to do a 3DES-decrypt.
            _cryptAcctNum = TripleDES.TripleDESEncrypt(New HexKey(clearPVK), _expPinValData)
            _decimalisedAcctNum = Utility.Decimalise(_cryptAcctNum, _decTable)
            _naturalPin = _decimalisedAcctNum.Substring(0, Convert.ToInt32(_checkLen))
            _derivedPin = Utility.AddNoCarry(_naturalPin, _offsetValue.Substring(0, _offsetValue.IndexOf("F")))

            Dim cryptPIN As String = EncryptPINForHostStorage(_derivedPin)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptPIN)

            Return mr

        End Function

    End Class

End Namespace
