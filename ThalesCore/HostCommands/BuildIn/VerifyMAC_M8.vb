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
    ''' Verifies a MAC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the M8 Thales command.
    ''' </remarks>
    <ThalesCommandCode("M8", "M9", "", "Verifies a MAC on a message using a TAK or ZAK.")> _
    Public Class VerifyMAC_M8
        Inherits GenerateMAC_M6

        Private _expectedMAC As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the M8 message parsing fields.
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
                _modeFlag = kvp.Item("Mode Flag")
                _inputModeFlag = kvp.Item("Input Format Flag")
                _MACAlgorithm = kvp.Item("MAC Algorithm")
                _PaddingMethod = kvp.Item("Padding Method")
                _keyType = kvp.Item("Key Type")
                _key = kvp.ItemCombination("Key Scheme", "Key")
                _IV = kvp.Item("IV")
                _msgLength = kvp.Item("Message Length")
                _msg = kvp.Item("Message")
                _expectedMAC = kvp.Item("MAC")
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

            Dim errorCode As String = "", generatedMAC As String = "", clearKey As String = ""
            ProcessMACGeneration(errorCode, clearKey, generatedMAC)

            If errorCode <> "" Then
                mr.AddElement(errorCode)
                Return mr
            End If

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("IV: " + _IV)
            Log.Logger.MinorInfo("Resulting MAC: " + generatedMAC)
            Log.Logger.MinorInfo("Expecting MAC: " + _expectedMAC)

            If generatedMAC.Substring(0, 8) = _expectedMAC Then
                mr.AddElement(ErrorCodes._00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes._01_VERIFICATION_FAILURE)
            End If

            If _modeFlag = "1" OrElse _modeFlag = "2" Then
                mr.AddElement(_IV)
            End If

            Return mr

        End Function

    End Class

End Namespace