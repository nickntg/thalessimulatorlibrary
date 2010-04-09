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

        Const NUMBER_COMPONENTS As String = "NBR_COMPONENTS"
        Const COMPONENT As String = "COMPONENT_"

        Private _nbrComponents As String = ""
        Private _iNbrComponents As Integer
        Private _zmkScheme As String
        Private _lmkScheme As String = ""
        Private _keyCheckValue As String
        Private _defs(8) As MessageFieldParser
        Private _comps(8) As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the GY message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection

            MFPC.AddMessageFieldParser(New MessageFieldParser(NUMBER_COMPONENTS, 1))

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
            _nbrComponents = MFPC.GetMessageFieldByName(NUMBER_COMPONENTS).FieldValue()

            Try
                _iNbrComponents = Convert.ToInt32(_nbrComponents)

                For i As Integer = 1 To 8
                    If msg.CharsLeft() / _iNbrComponents > 20 Then
                        _defs(i - 1) = GenerateZMKKeyParser(COMPONENT + i.ToString(), 1)
                    Else
                        _defs(i - 1) = GenerateZMKKeyParser(COMPONENT + i.ToString(), 9999)
                    End If
                Next

                If _iNbrComponents < 2 OrElse _iNbrComponents > 9 Then
                    _iNbrComponents = -1
                    Exit Sub
                Else
                    For i As Integer = 1 To _iNbrComponents
                        Try
                            _defs(i - 1).ParseField(msg)
                            _comps(i - 1) = _defs(i - 1).FieldValue
                        Catch ex As Exceptions.XShortMessage
                            _iNbrComponents = -2
                            Exit Sub
                        Catch ex As Exceptions.XNoDeterminerMatched
                            _iNbrComponents = -2
                            Exit Sub
                        Catch ex As Exception
                            _iNbrComponents = -2
                            Exit Sub
                        End Try
                    Next
                End If
            Catch ex As Exception
                _iNbrComponents = -1
                Exit Sub
            End Try

            MFPC = New MessageFieldParserCollection

            GenerateDelimiterParser()

            MFPC.ParseMessage(msg)

            _zmkScheme = MFPC.GetMessageFieldByName(KEY_SCHEME_ZMK).FieldValue
            _lmkScheme = MFPC.GetMessageFieldByName(KEY_SCHEME_LMK).FieldValue
            _keyCheckValue = MFPC.GetMessageFieldByName(KEY_CHECK_VALUE).FieldValue

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

            If Convert.ToBoolean(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE)) = False Then
                Log.Logger.MajorInfo("Can't print clear key while not in the AUTHORIZED state")
                mr.AddElement(ErrorCodes._17_HSM_IS_NOT_IN_THE_AUTHORIZED_STATE)
                Return mr
            End If

            If _iNbrComponents = -1 Then
                mr.AddElement(ErrorCodes._03_INVALID_NUMBER_OF_COMPONENTS)
                Return mr
            ElseIf _iNbrComponents = -2 Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            Dim lmkKs As KeySchemeTable.KeyScheme

            If _lmkScheme <> "" AndAlso _zmkScheme <> "" Then
                If ValidateKeySchemeCode(_lmkScheme, lmkKs, mr) = False Then Return mr
                If _zmkScheme <> "0" Then
                    mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                    Return mr
                End If
            Else
                If _defs(0).DeterminerName = COMPONENT + "1" + PLAIN_DOUBLE Then
                    lmkKs = KeySchemeTable.KeyScheme.DoubleLengthKeyAnsi
                Else
                    lmkKs = KeySchemeTable.KeyScheme.SingleDESKey
                End If
            End If

            Dim clearKeys(8) As String, clearKey As String = ""

            For i As Integer = 1 To _iNbrComponents
                clearKeys(i - 1) = Utility.DecryptUnderLMK(_comps(i - 1), COMPONENT + i.ToString(), _defs(i - 1).DeterminerName, LMKPairs.LMKPair.Pair04_05, "0")
                If Utility.IsParityOK(clearKeys(i - 1), Utility.ParityCheck.OddParity) = False Then
                    If i = 1 Then
                        mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                    Else
                        mr.AddElement(ErrorCodes._11_DESTINATION_KEY_PARITY_ERROR)
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

            mr.AddElement(ErrorCodes._00_NO_ERROR)

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

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As Message.MessageResponse
            Return Nothing
        End Function

    End Class

End Namespace
