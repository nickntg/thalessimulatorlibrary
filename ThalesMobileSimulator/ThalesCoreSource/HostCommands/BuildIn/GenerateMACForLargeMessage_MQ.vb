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
    ''' Generates a MAC for a large message.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the MQ Racal command.
    ''' </remarks>
    <ThalesCommandCode("MQ", "MR", "", "Generates a MAC for a large message")> _
    Public Class GenerateMACForLargeMessage_MQ
        Inherits AHostCommand

        Private _blockNbr As String
        Private _zak As String
        Private _iv As String
        Private _msgLen As String
        Private _data() As Byte

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the MQ message parsing fields.
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
                _blockNbr = kvp.Item("Message Block Number")
                _zak = kvp.ItemCombination("ZAK Scheme", "ZAK")
                _iv = kvp.ItemOptional("IV")
                If _iv = "" Then _iv = ZEROES
                _msgLen = kvp.Item("Message Length")
                _data = System.Text.ASCIIEncoding.Default.GetBytes(kvp.Item("Message Block"))
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

            Dim cryptZAK As New HexKey(_zak)
            Dim zak As String = Utility.DecryptUnderLMK(cryptZAK.ToString, cryptZAK.Scheme, LMKPairs.LMKPair.Pair26_27, "0")
            If Utility.IsParityOK(zak, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim msgLen As Integer = Convert.ToInt32(_msgLen, 16)
            If msgLen <> _data.GetLength(0) Then
                mr.AddElement(ErrorCodes.ER_80_DATA_LENGTH_ERROR)
                Return mr
            End If

            Dim MAC As String = GenerateMAC(_data, zak, _iv)

            Log.Logger.MinorInfo("ZAK (clear): " + zak)
            Log.Logger.MinorInfo("Resulting MAC: " + MAC)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(MAC)

            Return mr

        End Function

    End Class

End Namespace

