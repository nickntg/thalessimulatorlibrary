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
    ''' Generates a random key and prints it in the clear as split components.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the NE Racal command.
    ''' </remarks>
    <ThalesCommandCode("NE", "NF", "NZ", "Generates a random key and prints it in the clear as split components")> _
    Public Class GenerateAndPrintSplitComponents_NE
        Inherits AHostCommand

        Const KEY_TYPE As String = "KEY_TYPE"
        Const KEY_SCHEME As String = "KEY_SCHEME"

        Private _keyType As String = ""
        Private _keyScheme As String = ""
        Private _result As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the NE message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_TYPE, 3))
            MFPC.AddMessageFieldParser(New MessageFieldParser(KEY_SCHEME, 1))
        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)
            MFPC.ParseMessage(msg)
            _keyType = MFPC.GetMessageFieldByName(KEY_TYPE).FieldValue
            _keyScheme = MFPC.GetMessageFieldByName(KEY_SCHEME).FieldValue
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

            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme

            If CType(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE), Boolean) = False Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes._17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            If ValidateKeyTypeCode(_keyType, LMKKeyPair, var, mr) = False Then Return mr
            If ValidateKeySchemeCode(_keyScheme, ks, mr) = False Then Return mr

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKKeyPair, var)
            Dim checkValue As String = TripleDES.TripleDESEncrypt(New HexKey(rndKey), ZEROES)

            Log.Logger.MinorInfo("Key generated (clear): " + rndKey)
            Log.Logger.MinorInfo("Key generated (LMK): " + cryptRndKey)
            Log.Logger.MinorInfo("Check value: " + checkValue.Substring(0, 6))
            Select Case ks
                Case KeySchemeTable.KeyScheme.SingleDESKey
                    Log.Logger.MinorInfo("Component A: " + rndKey.Substring(0, 8))
                    Log.Logger.MinorInfo("Component B: " + rndKey.Substring(8))
                    AddPrinterData("COMPONENT A: " + rndKey.Substring(0, 8))
                    AddPrinterData("COMPONENT B: " + rndKey.Substring(8))
                Case KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi, KeySchemeTable.KeyScheme.DoubleLengthKeyVariant
                    Log.Logger.MinorInfo("Component A: " + rndKey.Substring(0, 16))
                    Log.Logger.MinorInfo("Component B: " + rndKey.Substring(16))
                    AddPrinterData("COMPONENT A: " + rndKey.Substring(0, 16))
                    AddPrinterData("COMPONENT B: " + rndKey.Substring(16))
                Case KeySchemeTable.KeyScheme.TripleLengthKeyAnsi, KeySchemeTable.KeyScheme.TripleLengthKeyVariant
                    Log.Logger.MinorInfo("Component A: " + rndKey.Substring(0, 16))
                    Log.Logger.MinorInfo("Component B: " + rndKey.Substring(16, 16))
                    Log.Logger.MinorInfo("Component C: " + rndKey.Substring(32))
                    AddPrinterData("COMPONENT A: " + rndKey.Substring(0, 8))
                    AddPrinterData("COMPONENT B: " + rndKey.Substring(16, 16))
                    AddPrinterData("COMPONENT C: " + rndKey.Substring(32))
            End Select

            AddPrinterData("CHECK VALUE: " + checkValue.Substring(0, 6))

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(cryptRndKey)

            _result = "OK"

            Return mr

        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> if the HSM is not in the authorized state.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            If _result = "OK" Then
                Dim mr As New MessageResponse
                mr.AddElement(ErrorCodes._00_NO_ERROR)
                Return mr
            Else
                Return Nothing
            End If
        End Function

    End Class

End Namespace
