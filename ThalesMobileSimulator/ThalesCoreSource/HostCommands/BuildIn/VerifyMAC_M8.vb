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

        Private expectedMAC As String

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
            If XMLParseResult = ErrorCodes.ER_00_NO_ERROR Then
                modeFlag = kvp.Item("Mode Flag")
                inputModeFlag = kvp.Item("Input Format Flag")
                MACAlgorithm = kvp.Item("MAC Algorithm")
                PaddingMethod = kvp.Item("Padding Method")
                keyType = kvp.Item("Key Type")
                key = kvp.ItemCombination("Key Scheme", "Key")
                IV = kvp.ItemOptional("IV")
                msgLength = kvp.Item("Message Length")
                strmsg = kvp.Item("Message")
                expectedMAC = kvp.Item("MAC")
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
            Log.Logger.MinorInfo("IV: " + IV)
            Log.Logger.MinorInfo("Resulting MAC: " + generatedMAC)
            Log.Logger.MinorInfo("Expecting MAC: " + expectedMAC)

            If generatedMAC.Substring(0, 8) = expectedMAC Then
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            Else
                mr.AddElement(ErrorCodes.ER_01_VERIFICATION_FAILURE)
            End If

            If modeFlag = "1" OrElse modeFlag = "2" Then
                mr.AddElement(IV)
            End If

            Return mr

        End Function

    End Class

End Namespace