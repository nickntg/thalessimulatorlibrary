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
Imports ThalesSim.Core.Cryptography.MAC
Imports ThalesSim.Core

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Generates a MAC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the MS Thales command.
    ''' </remarks>
    <ThalesCommandCode("MS", "MT", "", "Generate a MAC (MAB) using Ansi X9.19 for large message.")> _
    Public Class GeneraceMACMABUsingAnsiX919ForLargeMessage_MS
        Inherits AHostCommand

        Protected Const MAC_SINGLEBLOCK As String = "0"
        Protected Const MAC_FIRSTBLOCK As String = "1"
        Protected Const MAC_MIDDLEBLOCK As String = "2"
        Protected Const MAC_LASTBLOCK As String = "3"

        Protected Const INPUT_BINARY As String = "0"
        Protected Const INPUT_HEX As String = "1"

        Protected Const KEY_TAK As String = "0"
        Protected Const KEY_ZAK As String = "1"

        Protected Const KEY_SINGLE As String = "0"
        Protected Const KEY_DOUBLE As String = "1"

        Protected blockNumber As String
        Protected keyType As String
        Protected keyLength As String
        Protected messageType As String
        Protected key As String
        Protected IV As String
        Protected msgLength As String
        Protected strmsg As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the MS message parsing fields.
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
                blockNumber = kvp.Item("Message Block")
                keyType = kvp.Item("Key Type")
                keyLength = kvp.Item("Key Length")
                messageType = kvp.Item("Message Type")
                key = kvp.ItemCombination("Key Scheme", "Key")
                IV = kvp.ItemOptional("IV")
                msgLength = kvp.Item("Message Length")
                strmsg = kvp.Item("Message")
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

            Dim cryptKey As New HexKey(key)

            If Not (((cryptKey.KeyLen = HexKey.KeyLength.SingleLength) AndAlso (keyLength = KEY_SINGLE)) OrElse _
                    ((cryptKey.KeyLen = HexKey.KeyLength.DoubleLength) AndAlso (keyLength = KEY_DOUBLE))) Then
                mr.AddElement(ErrorCodes.ER_04_INVALID_KEY_TYPE_CODE)
                Return mr
            End If

            Dim len As Integer = Convert.ToInt32(msgLength, 16)
            If messageType = INPUT_HEX Then
                len = len * 2
            End If

            If strmsg.Length < len Then
                mr.AddElement(ErrorCodes.ER_80_DATA_LENGTH_ERROR)
                Return mr
            End If

            Dim rawData As String = String.Empty
            If messageType = INPUT_HEX Then
                rawData = strmsg.Substring(0, len)
            Else
                Utility.ByteArrayToHexString(Utility.GetBytesFromString(strmsg.Substring(0, len)), rawData)
            End If

            Dim clearKey As String = String.Empty
            If keyType = KEY_TAK Then
                clearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair16_17, "0")
            Else
                clearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair26_27, "0")
            End If

            Dim block As MAC.ISOX919Blocks
            Select Case blockNumber
                Case MAC_SINGLEBLOCK
                    block = ISOX919Blocks.OnlyBlock
                    IV = ZEROES
                Case MAC_FIRSTBLOCK
                    block = ISOX919Blocks.FirstBlock
                    IV = ZEROES
                Case MAC_MIDDLEBLOCK
                    block = ISOX919Blocks.NextBlock
                Case Else
                    block = ISOX919Blocks.FinalBlock
            End Select

            Dim generatedMAC As String = ISOX919MAC.MacHexData(rawData, New HexKey(clearKey), IV, block)

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("IV: " + IV)
            Log.Logger.MinorInfo("Resulting MAC: " + generatedMAC)

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(generatedMAC)

            Return mr
        End Function

    End Class

End Namespace