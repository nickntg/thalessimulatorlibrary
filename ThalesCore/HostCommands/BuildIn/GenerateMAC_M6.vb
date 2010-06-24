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
    ''' Generates a MAC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the M6 Thales command.
    ''' </remarks>
    <ThalesCommandCode("M6", "M7", "", "Generate a MAC on a message using a TAK or ZAK.")> _
    Public Class GenerateMAC_M6
        Inherits AHostCommand

        Protected Const MAC_SINGLEBLOCK As String = "0"
        Protected Const MAC_FIRSTBLOCK As String = "1"
        Protected Const MAC_MIDDLEBLOCK As String = "2"
        Protected Const MAC_LASTBLOCK As String = "3"

        Protected Const INPUT_BINARY As String = "0"
        Protected Const INPUT_HEX As String = "1"
        Protected Const INPUT_TEXT As String = "2"

        Protected Const MAC_ALGORITHM_1 As String = "01"
        Protected Const MAC_ALGORITHM_3 As String = "03"

        Protected Const PAD_NOPADDING As String = "0"
        Protected Const PAD_METHOD_1 As String = "1"
        Protected Const PAD_METHOD_2 As String = "2"

        Protected Const KEY_TAK As String = "003"
        Protected Const KEY_ZAK As String = "008"

        Protected _modeFlag As String
        Protected _inputModeFlag As String
        Protected _MACAlgorithm As String
        Protected _PaddingMethod As String
        Protected _keyType As String
        Protected _key As String
        Protected _IV As String
        Protected _msgLength As String
        Protected _msg As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the M6 message parsing fields.
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

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            If _modeFlag = MAC_FIRSTBLOCK OrElse _modeFlag = MAC_MIDDLEBLOCK Then
                mr.AddElement(_IV)
                mr.AddElement("0000")
            Else
                mr.AddElement("0008")
                mr.AddElement(generatedMAC.Substring(0, 8))
            End If

            Return mr
        End Function

        ''' <summary>
        ''' This method performs sanity checks and generates the MAC.
        ''' </summary>
        ''' <param name="ErrorCode">If non-empty is returned, a logical error has occured.</param>
        ''' <param name="ClearKey">Returns the clear key.</param>
        ''' <param name="GeneratedMAC">Returns the complete MAC.</param>
        ''' <remarks></remarks>
        Protected Sub ProcessMACGeneration(ByRef ErrorCode As String, ByRef ClearKey As String, ByRef GeneratedMAC As String)
            Dim cryptKey As New HexKey(_key)

            If _keyType = KEY_TAK Then
                ClearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair16_17, "0")
            Else
                ClearKey = Utility.DecryptUnderLMK(cryptKey.ToString, cryptKey.Scheme, LMKPairs.LMKPair.Pair26_27, "0")
            End If

            If Utility.IsParityOK(clearKey, Utility.ParityCheck.OddParity) = False Then
                ErrorCode = ErrorCodes._10_SOURCE_KEY_PARITY_ERROR
                Exit Sub
            End If

            Dim hexData As String = ""
            If _inputModeFlag = INPUT_HEX Then
                'Message is hex string.
                hexData = _msg
            Else
                'Message is bytes or ASCII.
                Utility.ByteArrayToHexString(System.Text.ASCIIEncoding.Default.GetBytes(_msg), hexData)
            End If

            If (_modeFlag = MAC_FIRSTBLOCK OrElse _modeFlag = MAC_MIDDLEBLOCK) OrElse (_PaddingMethod = PAD_NOPADDING) Then
                If _msg.Length Mod 8 <> 0 Then
                    ErrorCode = "06"
                    Exit Sub
                End If
            End If

            If _modeFlag = MAC_FIRSTBLOCK OrElse _modeFlag = MAC_SINGLEBLOCK Then
                _IV = ZEROES
            End If

            Dim algo As MAC.ISO9797Algorithms
            If _MACAlgorithm = MAC_ALGORITHM_1 Then
                algo = MAC.ISO9797Algorithms.MACAlgorithm1
            Else
                algo = MAC.ISO9797Algorithms.MACAlgorithm3
            End If

            Dim pad As MAC.ISO9797PaddingMethods
            If _PaddingMethod = PAD_NOPADDING Then
                pad = MAC.ISO9797PaddingMethods.NoPadding
            ElseIf _PaddingMethod = PAD_METHOD_1 Then
                pad = MAC.ISO9797PaddingMethods.PaddingMethod1
            Else
                pad = MAC.ISO9797PaddingMethods.PaddingMethod2
            End If

            GeneratedMAC = MAC.ISO9797MAC.MACHexData(hexData, New HexKey(clearKey), _IV, algo, pad)
        End Sub

    End Class

End Namespace