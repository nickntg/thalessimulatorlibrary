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
    ''' Generates a AMEX CSC.
    ''' </summary>
    ''' <remarks>
    ''' This class implements the RY Racal command.
    ''' </remarks>
    <ThalesCommandCode("RY", "RZ", "", "Generates a AMEX CSC")> _
    Public Class GenerateAMEXCSC_RY
        Inherits AHostCommand

        Private _mode As String
        Private _flag As String
        Private _csckPair As String
        Private _acct As String
        Private _expiration As String
        Private _svc As String = ""
        Private _5csc As String = ""
        Private _4csc As String = ""
        Private _3csc As String = ""

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the CY message parsing fields.
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
                _mode = kvp.Item("Mode")
                _flag = kvp.Item("Flag")
                _csckPair = kvp.ItemCombination("CSCK Scheme", "CSCK")
                _acct = kvp.Item("Account Number")
                _expiration = kvp.Item("Expiration Date")
                If _flag <> "0" Then
                    _svc = kvp.Item("Service Code")
                End If
                If _mode = "4" Then
                    _5csc = kvp.Item("5CSC")
                    _4csc = kvp.Item("4CSC")
                    _3csc = kvp.Item("3CSC")
                End If
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

            Dim vr As String = "" 'verifivation results

            Dim cryptCSCK As New HexKey(_csckPair)
            Dim clearCSCK As String = Utility.DecryptUnderLMK(cryptCSCK.ToString, cryptCSCK.Scheme, LMKPairs.LMKPair.Pair14_15, "4")
            'If Utility.IsParityOK(clearCSCK, Utility.ParityCheck.OddParity) = False Then
            '    mr.AddElement(ErrorCodes.ER_10_SOURCE_KEY_PARITY_ERROR)
            '    Return mr
            'End If

            Dim CSC As String = GenerateCSC(clearCSCK, _acct, _expiration, _svc, Integer.Parse(_flag))

            Log.Logger.MinorInfo("Clear CSCK: " + clearCSCK)
            Log.Logger.MinorInfo("Passed CSC5: " + _5csc)
            Log.Logger.MinorInfo("Passed CSC4: " + _4csc)
            Log.Logger.MinorInfo("Passed CSC3: " + _3csc)
            Log.Logger.MinorInfo("Resulting CSC5: " + CSC.Substring(0, 5))
            Log.Logger.MinorInfo("Resulting CSC4: " + CSC.Substring(5, 4))
            Log.Logger.MinorInfo("Resulting CSC3: " + CSC.Substring(9, 3))

            If _mode = "3" Then
                mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                mr.AddElement(_mode)
                mr.AddElement(CSC)
            ElseIf _mode = "4" Then
                If _5csc = "FFFFF" Then
                    vr = "1"
                ElseIf _5csc = CSC.Substring(0, 5) Then
                    vr = "0"
                Else
                    vr = "2"
                End If

                If _4csc = "FFFF" Then
                    vr = "1"
                ElseIf _4csc = CSC.Substring(5, 4) Then
                    vr = "0"
                Else
                    vr = "2"
                End If

                If _3csc = "FFF" Then
                    vr = "1"
                ElseIf _3csc = CSC.Substring(9, 3) Then
                    vr = "0"
                Else
                    vr = "2"
                End If

                If InStr(vr, "2") = 0 Then
                    mr.AddElement(ErrorCodes.ER_00_NO_ERROR)
                Else
                    mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
                End If
                mr.AddElement(_mode)
                mr.AddElement(vr)

            Else
                mr.AddElement(ErrorCodes.ER_15_INVALID_INPUT_DATA)
            End If

            Return mr

        End Function

    End Class

End Namespace

