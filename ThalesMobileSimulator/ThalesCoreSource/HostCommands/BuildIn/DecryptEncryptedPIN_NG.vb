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
    ''' Decrypts an encrypted PIN.
    ''' </summary>
    ''' <remarks>This implements the NG Thales command.</remarks>
    <ThalesCommandCode("NG", "NH", "", "Decrypts an encrypted PIN.")> _
    Public Class DecryptEncryptedPIN_NG
        Inherits AHostCommand

        Private _cryptPIN As String
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
                _cryptPIN = kvp.Item("PIN")
                _acctNbr = kvp.Item("Account Number")
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

            Dim clearPIN As String = DecryptPINUnderHostStorage(_cryptPIN)
            clearPIN = clearPIN.PadRight(_cryptPIN.Length, "F"c)

            Log.Logger.MinorInfo("Encrypted PIN: " + _cryptPIN)
            Log.Logger.MinorInfo("Clear PIN: " + clearPIN)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(clearPIN)

            Return mr

        End Function

    End Class

End Namespace

