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

        ''' <summary>
        ''' NK Has Headers field
        ''' </summary>
        ''' <remarks></remarks>
        Private _hasHeaders As Boolean

        ''' <summary>
        ''' NK Number of commands field
        ''' </summary>
        ''' <remarks></remarks>
        Private _numberOfCommands As String
        Private _cmds As List(Of SubCommand)

        ''' <summary>
        ''' Constructor.
        ''' </summary>
        ''' <remarks>
        ''' The constructor sets up the NK message parsing fields.
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
            Try
                XML.MessageParser.Parse(msg, XMLMessageFields, kvp, XMLParseResult)
            Catch ex As Exception
                XMLParseResult = ErrorCodes._15_INVALID_INPUT_DATA
            End Try

            If XMLParseResult = ErrorCodes._00_NO_ERROR Then
                _hasHeaders = (kvp.Item("Header Flag") = "1")

                'Parse Number of commands field, sending error 52 if is not valid
                'Number of commands field must be between 1 and 99.
                Dim _iNumberOfCommands As Integer = 0
                Try
                    _numberOfCommands = kvp.Item("Number Of Commands")
                    _iNumberOfCommands = Convert.ToInt32(_numberOfCommands)

                    If _iNumberOfCommands < 1 OrElse _iNumberOfCommands > 99 Then
                        XMLParseResult = ErrorCodes._52_INVALID_NUMBER_OF_COMMANDS
                        Exit Sub
                    End If
                Catch ex As Exception
                    XMLParseResult = ErrorCodes._52_INVALID_NUMBER_OF_COMMANDS
                    Exit Sub
                End Try

                Dim CE As CommandExplorer = New HostCommands.CommandExplorer
                _cmds = New List(Of SubCommand)
                For i As Integer = 1 To _iNumberOfCommands
                    Try
                        Dim subCommand As New SubCommand(Convert.ToInt32(kvp.Item("SubCommand Length #" + i.ToString)), kvp.Item("SubCommand Data #" + i.ToString), _hasHeaders)
                        Dim hc As HostCommands.AHostCommand = ParseMessage(CE, subCommand)
                        GetMessageResponse(hc, subCommand)
                        _cmds.Add(subCommand)
                    Catch ex As Exception
                        XMLParseResult = ErrorCodes._15_INVALID_INPUT_DATA
                        Exit Sub
                    End Try
                Next

                'If has more bytes left on command or there are more commands...
                If (msg.CharsLeft > 0) OrElse (kvp.Count - 2 - _iNumberOfCommands * 2 <> 0) Then
                    XMLParseResult = ErrorCodes._15_INVALID_INPUT_DATA
                End If

            End If
        End Sub

        ''' <summary>
        ''' Call a host command implementation and get the results back.
        ''' </summary>
        ''' <param name="parsedCommand">Instance of host command.</param>
        ''' <param name="subCommand">Instance of sub command.</param>
        ''' <remarks></remarks>
        Private Sub GetMessageResponse(ByVal parsedCommand As AHostCommand, ByVal subCommand As SubCommand)
            Logger.MajorDebug(String.Format("Calling command [{0}] with data [{1}]", subCommand.CommandCode, subCommand.CommandParameters))
            Dim response As MessageResponse = parsedCommand.ConstructResponse()
            If response Is Nothing Then Throw New Exception("Error parsing message")
            Logger.MajorDebug(String.Format("Command [{0}] responded with data [{1}]", subCommand.CommandCode, response.MessageData))
            subCommand.Response = subCommand.MessageHeader + subCommand.ResponseCode + response.MessageData
        End Sub

        ''' <summary>
        ''' Creates the response message.
        ''' </summary>
        ''' <remarks>
        ''' This method creates the response message. The message header and message reply code
        ''' are <b>not</b> part of the message.
        ''' </remarks>
        Public Overrides Function ConstructResponse() As MessageResponse

            Dim mr As New MessageResponse

            ' Includes NK Error Code
            mr.AddElement(ErrorCodes._00_NO_ERROR)

            ' Includes NK Number of commands
            mr.AddElement(_numberOfCommands)

            For i As Integer = 0 To _cmds.Count - 1
                mr.AddElement(String.Format("{0:0000}", _cmds(i).Response.Length) + _cmds(i).Response)
            Next

            Return mr

        End Function

        ''' <summary>
        ''' Parses the NK contained message
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ParseMessage(ByVal CE As HostCommands.CommandExplorer, ByVal subCommand As SubCommand) As AHostCommand

            Logger.MajorVerbose("Subcommand: " + subCommand.CommandCode)

            Dim o As ThalesSim.Core.HostCommands.AHostCommand = Nothing

            Try
                Logger.MajorDebug("Searching for implementor of " + subCommand.CommandCode + "...")
                Dim CC As ThalesSim.Core.HostCommands.CommandClass = CE.GetLoadedCommand(subCommand.CommandCode)

                If CC Is Nothing Then
                    Logger.MajorError("No implementor for " + subCommand.CommandCode + "." + vbCrLf)
                Else
                    Logger.MajorDebug("Found implementor " + CC.DeclaringType.FullName() + ", instantiating...")
                    o = CType(Activator.CreateInstance(CC.DeclaringType), HostCommands.AHostCommand)

                    subCommand.ResponseCode = CC.ResponseCode

                    Logger.MajorDebug("Calling AcceptMessage()...")
                    o.AcceptMessage(New Message.Message(subCommand.CommandParameters))
                End If
            Catch ex As Exception
                Logger.MajorError("Exception while parsing message or creating subcommand instance" + vbCrLf + ex.ToString())
                o = Nothing
            End Try

            Return o

        End Function

    End Class

    ''' <summary>
    ''' Internal class that represents subcommand information.
    ''' </summary>
    ''' <remarks></remarks>
    Class SubCommand
        Public CommandLength As Integer
        Public MessageHeader As String = ""
        Public CommandCode As String = ""
        Public CommandParameters As String = ""
        Public ResponseCode As String = ""
        Public Response As String = ""

        Public Sub New(ByVal commandLength As Integer, ByVal messageHeader As String, ByVal commandCode As String, ByVal commandParameters As String)
            Me.CommandLength = commandLength
            Me.MessageHeader = messageHeader
            Me.CommandCode = commandCode
            Me.CommandParameters = commandParameters
        End Sub

        Public Sub New(ByVal commandLength As Integer, ByVal commandData As String, ByVal hasHeader As Boolean)
            Me.CommandLength = commandLength
            Dim idx As Integer = 0
            If hasHeader Then
                MessageHeader = commandData.Substring(idx, 4)
                idx += 4
            End If
            CommandCode = commandData.Substring(idx, 2)
            idx += 2
            CommandParameters = commandData.Substring(idx)
        End Sub
    End Class

End Namespace
