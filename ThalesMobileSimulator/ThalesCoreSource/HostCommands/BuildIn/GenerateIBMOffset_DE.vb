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
' Contributed by rjw - May 2010

Imports ThalesSim.Core.Message
Imports ThalesSim.Core.Cryptography
Imports ThalesSim.Core
Imports ThalesSim.Core.PIN.PINBlockFormat

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generates an IBM PIN Offset.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the DE Racal command.
    ''' </remarks>
    <ThalesCommandCode("DE", "DF", "", "Generates an IBM PIN Offset")> _
    Public Class GenerateIBMOffset_DE
        Inherits AHostCommand

        Private _pvk As String
        Private _maxPinLen As String
        Private _encPin As String
        Private _clearPin As String
        Private _checkLen As String
        Private _acct As String
        Private _decTable As String
        Private _pinValData As String
        Private _offsetValue As String
        Private _expPinValData As String
        Dim _cryptAcctNum As String
        Dim _decimalisedAcctNum As String
        Dim _naturalPin As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the DE message parsing fields.
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
                _pvk = kvp.ItemCombination("PVK Scheme", "PVK")
                _encPin = kvp.Item("PIN")
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

            Dim cryptPVK As New HexKey(_pvk)
            Dim clearPVK As String = Utility.DecryptUnderLMK(cryptPVK.ToString, cryptPVK.Scheme, LMKPairs.LMKPair.Pair14_15, "0")
            If Utility.IsParityOK(clearPVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim clearPin As String = DecryptPINUnderHostStorage(_encPin)

            If clearPin.Length < 4 OrElse clearPin.Length > 12 Then
                mr.AddElement(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            _expPinValData = _pinValData.Substring(0, _pinValData.IndexOf("N"))
            _expPinValData = _expPinValData + _acct.Substring(_acct.Length - 5, 5)
            _expPinValData = _expPinValData + _pinValData.Substring(_pinValData.IndexOf("N") + 1, (_pinValData.Length - (_pinValData.IndexOf("N") + 1)))

            _cryptAcctNum = DES.DESEncrypt(clearPVK, _expPinValData)
            _decimalisedAcctNum = Utility.Decimalise(_cryptAcctNum, _decTable)
            _naturalPin = _decimalisedAcctNum.Substring(0, Convert.ToInt32(_checkLen))
            _offsetValue = Utility.SubtractNoBorrow(clearPin, _decimalisedAcctNum.Substring(0, 4))

            Log.Logger.MinorInfo("Resulting Offset: " + _offsetValue)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(_offsetValue.PadRight(12, "F"c))

            Return mr

        End Function

    End Class

End Namespace
