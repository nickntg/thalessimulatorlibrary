﻿''
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

Imports ThalesSim.Core.Log
Imports ThalesSim.Core.Message

Namespace HostCommands.BuildIn

    ''' <summary>
    ''' Allows multiple commands to be sent as a bundle.
    ''' </summary>
    ''' <remarks></remarks>
    <ThalesCommandCode("NK", "NL", "", "Allows multiple commands to be sent as a bundle.")> _
    Public Class CommandChaining_NK
        Inherits AHostCommand

        Const HAS_HEADERS As String = "HAS_HEADERS"
        Const NUMBER_OF_COMMANDS As String = "NUMBER_OF_COMMANDS"
        Const COMMANDS As String = "COMMANDS"
        Const COMMAND As String = "COMMAND_"

        ''' <summary>
        ''' NK Has Headers field
        ''' </summary>
        ''' <remarks></remarks>
        Private _hasHeaders As String

        ''' <summary>
        ''' NK Number of commands field
        ''' </summary>
        ''' <remarks></remarks>
        Private _numberOfCommands As String
        Private _iNumberOfCommands As Integer
        Private _commandDefs(99 * 2) As MessageFieldParser
        Private _commands As List(Of AHostCommand)
        Private _responses As List(Of MessageResponse)

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the NK message parsing fields.
        ''' </remarks>
        Public Sub New()
            _commands = New List(Of AHostCommand)
            _responses = New List(Of MessageResponse)
            MFPC = New MessageFieldParserCollection
            MFPC.AddMessageFieldParser(New MessageFieldParser(HAS_HEADERS, 1))
            MFPC.AddMessageFieldParser(New MessageFieldParser(NUMBER_OF_COMMANDS, 2))

            'Charges one Field Parser for each internal command length and another for each command itself
            'All of size 4 (but the internal command length will be changed dinamicaly later)
            For iterator As Integer = 1 To 99 * 2
                _commandDefs(iterator - 1) = New MessageFieldParser(COMMAND + iterator.ToString(), 4)
            Next

        End Sub

        ''' <summary>
        ''' Parses the request message.
        ''' </summary>
        ''' <remarks>
        ''' This method parses the command message. The message header and message command
        ''' code are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Sub AcceptMessage(ByVal msg As Message.Message)

            Dim CE As CommandExplorer = New HostCommands.CommandExplorer

            MFPC.ParseMessage(msg)
            _hasHeaders = MFPC.GetMessageFieldByName(HAS_HEADERS).FieldValue()
            _numberOfCommands = MFPC.GetMessageFieldByName(NUMBER_OF_COMMANDS).FieldValue()

            Dim MFPCSubCommands As New ThalesSim.Core.Message.MessageFieldParserCollection
            MFPCSubCommands = GetMessageFieldParserCollection(_hasHeaders = "1")

            Try
                _iNumberOfCommands = Convert.ToInt32(_numberOfCommands)
                If _iNumberOfCommands < 1 OrElse _iNumberOfCommands > 99 Then
                    _iNumberOfCommands = -1
                Else
                    For i As Integer = 1 To _iNumberOfCommands * 2 Step 2
                        Try
                            'Parsing the command length
                            _commandDefs(i - 1).ParseField(msg)
                            Dim commandLength As Integer = Convert.ToInt32(_commandDefs(i - 1).FieldValue)
                            'Sets the command length dynamicaly
                            _commandDefs(i).Length = commandLength
                            _commandDefs(i).ParseField(msg)

                            Dim parsedCommand As AHostCommand = ParseMessage(CE, MFPCSubCommands, _commandDefs(i).FieldValue)
                            If parsedCommand IsNot Nothing Then
                                _commands.Add(parsedCommand)
                                Dim response As MessageResponse = GetMessageResponse(CE, MFPCSubCommands, parsedCommand)

                                If response IsNot Nothing Then
                                    _responses.Add(response)
                                Else
                                    _iNumberOfCommands = -2
                                    Exit Sub
                                End If


                            Else
                                _iNumberOfCommands = -2
                                Exit Sub
                            End If
                        Catch ex As Exceptions.XShortMessage
                            _iNumberOfCommands = -2
                            Exit Sub
                        Catch ex As Exceptions.XNoDeterminerMatched
                            _iNumberOfCommands = -2
                            Exit Sub
                        Catch ex As Exception
                            _iNumberOfCommands = -2
                            Exit Sub
                        End Try
                    Next
                End If
            Catch ex As Exception
                _iNumberOfCommands = -1
            End Try

        End Sub

        Private Function GetMessageResponse(ByVal CE As CommandExplorer, ByVal MFPC As ThalesSim.Core.Message.MessageFieldParserCollection, ByVal parsedCommand As AHostCommand) As MessageResponse

            Dim response As MessageResponse = parsedCommand.ConstructResponse()
            Dim commandCode As String = MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue
            Dim CC As ThalesSim.Core.HostCommands.CommandClass = CE.GetLoadedCommand(commandCode)
            response.AddElementFront(CC.ResponseCode)
            If _hasHeaders = "1" Then
                response.AddElementFront(MFPC.GetMessageFieldByName(HEADER).FieldValue)
            End If
            Return response
        End Function

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As MessageResponse

            Dim mr As New MessageResponse

            If _iNumberOfCommands = -1 Then
                mr.AddElement(ErrorCodes._03_INVALID_NUMBER_OF_COMPONENTS)
                Return mr
            ElseIf _iNumberOfCommands = -2 Then
                mr.AddElement(ErrorCodes._15_INVALID_INPUT_DATA)
                Return mr
            End If

            ' Includes NK Error Code
            mr.AddElement(ErrorCodes._00_NO_ERROR)

            'Includes Command Responses
            For index As Integer = 0 To _responses.Count - 1
                Dim subcommandResponse As String = _responses(index).MessageData
                'Includes subcommand response length
                Dim subcommandLength As String = String.Format("{0:0000}", subcommandResponse.Length)
                mr.AddElement(subcommandLength)
                'Includes subcommand response
                mr.AddElement(subcommandResponse)
            Next

            Return mr

        End Function

        ''' <summary>
        ''' Creates the response message after printer I/O is concluded.
        ''' </summary>
        ''' <remarks>
        ''' This method returns <b>Nothing</b> as no printer I/O is related with this command.
        ''' </remarks>
        Public Overrides Function ConstructResponseAfterOperationComplete() As MessageResponse
            Return Nothing
        End Function

        Private Const HEADER As String = "HEADER"
        Private Const COMMAND_CODE As String = "COMMAND_CODE"

        ''' <summary>
        ''' Get The SubCommand Parser
        ''' </summary>
        ''' <param name="hasHeaders"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetMessageFieldParserCollection(ByVal hasHeaders As Boolean) As ThalesSim.Core.Message.MessageFieldParserCollection
            Dim MFPC As New ThalesSim.Core.Message.MessageFieldParserCollection
            If hasHeaders Then
                MFPC.AddMessageFieldParser(New ThalesSim.Core.Message.MessageFieldParser(HEADER, 4))
            End If
            MFPC.AddMessageFieldParser(New ThalesSim.Core.Message.MessageFieldParser(COMMAND_CODE, 2))
            Return MFPC
        End Function

        ''' <summary>
        ''' Parses the NK contained message
        ''' </summary>
        ''' <param name="MFPC"></param>
        ''' <param name="subcommand"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseMessage(ByVal CE As HostCommands.CommandExplorer, ByVal MFPC As ThalesSim.Core.Message.MessageFieldParserCollection, ByRef subcommand As String) As AHostCommand

            Dim msg As New ThalesSim.Core.Message.Message(subcommand)
            Logger.MajorVerbose("Subcommand: " + msg.MessageData())

            Dim o As ThalesSim.Core.HostCommands.AHostCommand = Nothing

            Try
                Logger.MajorDebug("Parsing header and code of message " + msg.MessageData + "...")
                MFPC.ParseMessage(msg)

                Logger.MajorDebug("Searching for implementor of " + MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue + "...")
                Dim CC As ThalesSim.Core.HostCommands.CommandClass = CE.GetLoadedCommand(MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue)

                If CC Is Nothing Then
                    Logger.MajorError("No implementor for " + MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue + "." + vbCrLf)
                Else
                    Logger.MajorDebug("Found implementor " + CC.DeclaringType.FullName() + ", instantiating...")

                    o = CType(Activator.CreateInstance(CC.DeclaringType), HostCommands.AHostCommand)

                    Logger.MajorDebug("Calling AcceptMessage()...")
                    o.AcceptMessage(msg)
                End If
            Catch ex As Exception
                Logger.MajorError("Exception while parsing message or creating subcommand instance" + vbCrLf + ex.ToString())
                o = Nothing
            End Try

            Return o

        End Function

    End Class
End Namespace
