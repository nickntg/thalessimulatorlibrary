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
    ''' Generates a VISA CVV.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the CW Racal command.
    ''' </remarks>
    <ThalesCommandCode("CW", "CX", "", "Generates a VISA CVV")> _
    Public Class GenerateVISACVV_CW
        Inherits AHostCommand

        Const CVK_PAIR As String = "CVK_PAIR"
        Const ACCT_NBR As String = "ACCOUNT_NUMBER"
        Const EXPIRATION As String = "EXPIRATION"
        Const SVC As String = "SVC"

        Private _acct As String
        Private _cvkPair As String
        Private _expiration As String
        Private _svc As String

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CW message parsing fields.
        ''' </remarks>
        Public Sub New()
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(GeneratePVKKeyParser(CVK_PAIR))
            MFPC.AddMessageFieldParser(New MessageFieldParser(ACCT_NBR, DELIMITER_VALUE))
            MFPC.AddMessageFieldParser(New MessageFieldParser(DELIMITER, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(EXPIRATION, 4))
            MFPC.AddMessageFieldParser(New MessageFieldParser(SVC, 3))
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
            _acct = MFPC.GetMessageFieldByName(ACCT_NBR).FieldValue()
            _cvkPair = MFPC.GetMessageFieldByName(CVK_PAIR).FieldValue()
            _expiration = MFPC.GetMessageFieldByName(EXPIRATION).FieldValue()
            _svc = MFPC.GetMessageFieldByName(SVC).FieldValue()
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

            Dim clearCVK As String = Utility.DecryptUnderLMK(_cvkPair, CVK_PAIR, MFPC.GetMessageFieldByName(CVK_PAIR).DeterminerName, LMKPairs.LMKPair.Pair14_15, "4")
            If Utility.IsParityOK(clearCVK, Utility.ParityCheck.OddParity) = False Then
                mr.AddElement(ErrorCodes._10_SOURCE_KEY_PARITY_ERROR)
                Return mr
            End If

            Dim CVV As String = GenerateCVV(clearCVK, _acct, _expiration, _svc)

            Log.Logger.MinorInfo("Clear CVK: " + clearCVK)
            Log.Logger.MinorInfo("Resulting CVV: " + CVV)

            mr.AddElement(ErrorCodes._00_NO_ERROR)
            mr.AddElement(CVV)

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
