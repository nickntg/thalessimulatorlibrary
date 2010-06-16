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
    ''' Generates a random ZMK component and prints it in the clear.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the OC Racal command.
    ''' </remarks>
    <ThalesCommandCode("OC", "OD", "OZ", "Generates a random ZMK component and prints it in the clear")> _
    Public Class GenerateAndPrintZMKComponent_OC
        Inherits AHostCommand

        Private _keyType As String = ""
        Private _keyScheme As String = ""
        Private _result As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the OC message parsing fields.
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
                _keyType = "000"
                _keyScheme = kvp.ItemOptional("Key Scheme LMK")
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

            Dim LMKKeyPair As LMKPairs.LMKPair, var As String = ""
            Dim ks As KeySchemeTable.KeyScheme

            If Not IsInAuthorizedState() Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes._17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            Select Case CType(Resources.GetResource(Resources.DOUBLE_LENGTH_ZMKS), Boolean)
                Case False
                    If _keyScheme = "" Then
                        _keyScheme = "Z"
                    ElseIf _keyScheme <> "Z" Then
                        mr.AddElement(ErrorCodes._26_INVALID_KEY_SCHEME)
                        Return mr
                    End If
                Case True
                    If _keyScheme = "" Then
                        _keyScheme = "X"
                    ElseIf _keyScheme = "Z" Then
                        mr.AddElement(ErrorCodes._26_INVALID_KEY_SCHEME)
                        Return mr
                    End If
            End Select

            If ValidateKeyTypeCode(_keyType, LMKKeyPair, var, mr) = False Then Return mr
            If ValidateKeySchemeCode(_keyScheme, ks, mr) = False Then Return mr

            Dim rndKey As String = Utility.CreateRandomKey(ks)
            Dim cryptRndKey As String = Utility.EncryptUnderLMK(rndKey, ks, LMKKeyPair, var)

            Log.Logger.MinorInfo("ZMK component (clear): " + rndKey)
            Log.Logger.MinorInfo("ZMK component (LMK): " + cryptRndKey)

            AddPrinterData("ZMK COMPONENT")
            AddPrinterData(rndKey)

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
