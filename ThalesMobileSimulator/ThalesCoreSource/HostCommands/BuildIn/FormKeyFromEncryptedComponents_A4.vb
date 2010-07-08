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
    ''' Forms a key from encrypted components.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the A4 Racal command.
    ''' </remarks>
    <ThalesCommandCode("A4", "A5", "", "Forms a key from encrypted components")> _
    Public Class FormKeyFromEncryptedComponents_A4
        Inherits AHostCommand

        Private _nbrComponents As String = ""
        Private _iNbrComponents As Integer
        Private _keyTypeCode As String = ""
        Private _lmkScheme As String = ""
        Private _comps(8) As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the A4 message parsing fields.
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
                _keyTypeCode = kvp.Item("Key Type")
                _lmkScheme = kvp.Item("Key Scheme (LMK)")
                _iNbrComponents = Convert.ToInt32(_nbrComponents)
                For i As Integer = 1 To _iNbrComponents
                    _comps(i - 1) = kvp.Item("Key Component #" + i.ToString)
                Next
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

            Dim LMKKeyPair As LMKPairs.LMKPair, var As Integer
            Dim ks As KeySchemeTable.KeyScheme

            If ValidateKeyTypeCode(_keyTypeCode, LMKKeyPair, var.ToString, mr) = False Then Return mr
            If ValidateKeySchemeCode(_lmkScheme, ks, mr) = False Then Return mr

            Dim clearKeys(8) As String, clearKey As String = ""
            Dim sourceKs As KeySchemeTable.KeyScheme = New HexKey(_comps(0)).Scheme

            For i As Integer = 1 To _iNbrComponents
                clearKeys(i - 1) = Utility.DecryptUnderLMK(_comps(i - 1), sourceKs, LMKKeyPair, var.ToString)
                If Utility.IsParityOK(clearKeys(i - 1), Utility.ParityCheck.OddParity) = False Then
                    mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
                    Return mr
                End If
                If clearKey <> "" Then
                    clearKey = Utility.XORHexStringsFull(clearKey, clearKeys(i - 1))
                Else
                    clearKey = clearKeys(i - 1)
                End If
            Next

            Dim cryptKey As String = Utility.EncryptUnderLMK(clearKey, ks, LMKKeyPair, var.ToString)
            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(clearKey), ZEROES)

            For i As Integer = 1 To _iNbrComponents
                Log.Logger.MinorInfo("Component " + i.ToString() + " (clear): " + Utility.RemoveKeyType(clearKeys(i - 1)))
            Next
            Log.Logger.MinorInfo("Key (clear): " + clearKey)
            Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))

            mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
            mr.AddElement(cryptKey)
            mr.AddElement(checkValue.Substring(0, 6))

            Return mr

        End Function

    End Class

End Namespace
