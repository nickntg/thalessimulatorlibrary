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
    ''' Forms a ZMK from 2 to 9 components.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the GY Racal command.
    ''' </remarks>
    <ThalesCommandCode("GY", "GZ", "", "Forms a ZMK from 2 to 9 components")> _
    Public Class FormZMKFromTwoToNineComponents_GY
        Inherits AHostCommand

        Private _nbrComponents As String = ""
        Private _iNbrComponents As Integer
        Private _lmkScheme As String = ""
        Private _keyCheckValue As String
        Private _comps(8) As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the GY message parsing fields.
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
                _nbrComponents = kvp.Item("Number of Components")
                _iNbrComponents = Convert.ToInt32(_nbrComponents)
                For i As Integer = 1 To _iNbrComponents
                    _comps(i - 1) = kvp.ItemCombination("ZMK Component Scheme #" + i.ToString, "ZMK Component #" + i.ToString)
                Next
                _lmkScheme = kvp.ItemOptional("Key Scheme LMK")
                _keyCheckValue = kvp.ItemOptional("Key Check Value Type")
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

            If Not IsInAuthorizedState() Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes.ER_17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            Dim lmkKs As KeySchemeTable.KeyScheme

            If _lmkScheme <> "" Then
                If ValidateKeySchemeCode(_lmkScheme, lmkKs, mr) = False Then Return mr
            Else
                If _comps(0).Length >= 32 Then
                    lmkKs = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                Else
                    lmkKs = KeySchemeTable.KeyScheme.SingleDESKey
                End If
            End If

            Dim clearKeys(8) As String, clearKey As String = ""

            For i As Integer = 1 To _iNbrComponents
                Dim component As New HexKey(_comps(i - 1))

                If Not IsInLegacyMode() Then
                    'If we're in legacy mode, the user can pass the key without its flag (ANSI) or the key with its flag.
                    'If we're NOT in legacy mode, we only want to allow the user to pass ANSI keys without a flag.
                    If _comps(i - 1).Length Mod 16 <> 0 Then
                        Log.Logger.MinorError("LegacyMode is off - key components must be in the form of 16H/32H only.")
                        mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
                        Return mr
                    End If
                End If

                clearKeys(i - 1) = Utility.DecryptUnderLMK(component.ToString, component.Scheme, LMKPairs.LMKPair.Pair04_05, "0")
                If Utility.IsParityOK(clearKeys(i - 1), Utility.ParityCheck.OddParity) = False Then
                    If i = 1 Then
                        mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                    Else
                        mr.AddElement(ErrorCodes.ER_11_DESTINATION_KEY_PARITY_ERROR)
                    End If
                    Return mr
                End If
                If clearKey <> "" Then
                    clearKey = Utility.XORHexStringsFull(clearKey, clearKeys(i - 1))
                Else
                    clearKey = clearKeys(i - 1)
                End If
            Next

            Dim cryptKey As String = Utility.EncryptUnderLMK(clearKey, lmkKs, LMKPairs.LMKPair.Pair04_05, "0")
            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            For i As Integer = 1 To _iNbrComponents
                Log.Logger.MinorInfo("Component " + i.ToString() + " (clear): " + Utility.RemoveKeyType(clearKeys(i - 1)))
            Next

            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Key (crypt): " + cryptKey)
            If _keyCheckValue = "1" Then
                Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))
            Else
                Log.Logger.MinorInfo("Check value: " + checkValue)
            End If

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)

            If _lmkScheme <> "" Then
                mr.AddElement(cryptKey)
            Else
                mr.AddElement(Utility.RemoveKeyType(cryptKey))
            End If

            If _keyCheckValue = "1" Then
                mr.AddElement(checkValue.Substring(0, 6))
            Else
                mr.AddElement(checkValue)
            End If

            Return mr

        End Function

    End Class

End Namespace
