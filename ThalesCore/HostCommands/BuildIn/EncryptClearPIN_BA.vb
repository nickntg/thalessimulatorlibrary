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
    ''' Encrypts a clear PIN.
    ''' </summary>
    ''' <remarks>This implements the BA Thales command.</remarks>
    <ThalesCommandCode("BA", "BB", "", "Encrypts a clear PIN")> _
    Public Class EncryptClearPIN_BA
        Inherits AHostCommand

        Private _clearPIN As String
        Private _acctNbr As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the BA message parsing fields.
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
                _clearPIN = kvp.Item("PIN")
                _acctNbr = kvp.Item("Account Number")

                'As per http://thalessim.codeplex.com/Thread/View.aspx?ThreadId=239725
                'we want to accomodate clear PINs with an F.
                If _clearPIN.IndexOf("F") > 0 Then
                    Dim newPin As String = _clearPIN.Replace("F", "")
                    _clearPIN = newPin.PadLeft(_clearPIN.Length, "0"c)
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

            _clearPIN = _clearPIN.Substring(1)

            If _clearPIN.Length < 4 OrElse _clearPIN.Length > 12 Then
                mr.AddElement(ErrorCodes.ER_24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                Return mr
            End If

            Dim cryptPIN As String = EncryptPINForHostStorage(_clearPIN)

            Log.Logger.MinorInfo("Clear PIN: " + _clearPIN)
            Log.Logger.MinorInfo("Encrypted PIN: " + cryptPIN)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptPIN)

            Return mr

        End Function

    End Class

End Namespace

