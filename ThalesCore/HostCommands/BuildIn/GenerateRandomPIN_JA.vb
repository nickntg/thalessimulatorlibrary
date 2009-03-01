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
    ''' Generates a random PIN of 4 to 12 digits.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the JA Racal command.
    ''' </remarks>
    <ThalesCommandCode("JA", "JB", "", "Generates a random PIN of 4 to 12 digits")> _
    Public Class GenerateRandomPIN_JA
        Inherits AHostCommand

        Const ACCOUNT_NBR As String = "ACCOUNT_NUMBER"
        Const PIN_LEN As String = "PIN_LENGTH"
        Const PIN_LEN_EXISTS As String = "PIN_EXISTS"
        Const PIN_LEN_NO_EXISTS As String = "PIN_DOESNT_EXIST"

        Private _acct As String
        Private _pinLen As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the JA message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCOUNT_NBR, 12))
            Dim MFDC As New MessageFieldDeterminerCollection
            MFDC.AddFieldDeterminer(New MessageFieldDeterminer(PIN_LEN_EXISTS, 1, 2))
            MFDC.AddFieldDeterminer(New MessageFieldDeterminer(PIN_LEN_NO_EXISTS, "", 0))
            MFPC.AddMessageFieldParser(New MessageFieldParser(PIN_LEN, MFDC))
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
            _acct = MFPC.GetMessageFieldByName(ACCOUNT_NBR).FieldValue
            _pinLen = MFPC.GetMessageFieldByName(PIN_LEN).FieldValue()
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

            If _pinLen = "" Then
                _pinLen = Convert.ToString(CType(Core.Resources.GetResource(Core.Resources.CLEAR_PIN_LENGTH), Integer))
            Else
                If IsNumeric(_pinLen) = False Then
                    mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                    Return mr
                End If

                If CType(Core.Resources.GetResource(Core.Resources.CLEAR_PIN_LENGTH), Integer) < Convert.ToInt32(_pinLen) OrElse _
                   Convert.ToInt32(_pinLen) < 4 Then
                    mr.AddElement(ErrorCodes._24_PIN_IS_FEWER_THAN_4_OR_MORE_THAN_12_DIGITS_LONG)
                    Return mr
                End If
            End If

            Dim PIN As String = GetRandomPIN(Convert.ToInt32(_pinLen))
            Dim PINCrypt As String = EncryptPINForHostStorage(PIN)

            Log.Logger.MinorInfo("Clear PIN: " + PIN)
            Log.Logger.MinorInfo("Crypt PIN: " + PINCrypt)
            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(PINCrypt)

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
