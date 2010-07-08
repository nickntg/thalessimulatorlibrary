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

Imports System.Net.Sockets
Imports ThalesSim.Core
Imports ThalesSim.Core.Log

''' <summary>
''' Main Racal simulator driver.
''' </summary>
''' <remarks>
''' This class drives the Racal simulator processing. It reads configuration data,
''' starts up the TCP listener socket, accepts incoming connections and creates
''' <see cref="HostCommands.AHostCommand"/> objects to serve requests.
''' </remarks>
Public Class ThalesMain
    Implements ILogProcs

    Private port As Integer
    Private consolePort As Integer
    Private maxCons As Integer
    Private curCons As Integer = 0
    Private consoleCurCons As Integer = 0
    Private LMKFile As String
    Private VBsources As String
    Private CheckLMKParity As Boolean
    Private HostDefsDir As String
    Private DoubleLengthZMKs As Boolean

    'Listening thread for hosts
    Private LT As Threading.Thread

    'Listening thread for console
    Private CLT As Threading.Thread

    'Host commands explorer
    Private CE As HostCommands.CommandExplorer

    'Console commands explorer
    Private CCE As ConsoleCommands.ConsoleCommandExplorer

    'Host connections
    Private WC() As TCP.WorkerClient

    'Console connection - we allow only one at a time
    Private CWC As TCP.WorkerClient

    'Host TCP listener
    Private SL As TcpListener

    'Console TCP listener
    Private CSL As TcpListener

    Private curMsg As ConsoleCommands.AConsoleCommand = Nothing

    ''' <summary>
    ''' This event is raised when a Thales command is called.
    ''' </summary>
    ''' <param name="sender">This instance.</param>
    ''' <param name="commandCode">The Thales command code.</param>
    ''' <remarks></remarks>
    Public Event CommandCalled(ByVal sender As ThalesMain, ByVal commandCode As String)

    ''' <summary>
    ''' Major event.
    ''' </summary>
    ''' <remarks>
    ''' This event is raised to communicate major events. These are simulator-wide
    ''' events like data arrival, client connect/disconnect events, errors etc.
    ''' </remarks>
    Public Event MajorLogEvent(ByVal sender As ThalesMain, ByVal s As String)

    ''' <summary>
    ''' Minor event.
    ''' </summary>
    ''' <remarks>
    ''' This event is raised to communicate minor events. These are specific events like
    ''' host command processing actions.
    ''' </remarks>
    Public Event MinorLogEvent(ByVal sender As ThalesMain, ByVal s As String)

    ''' <summary>
    ''' Printer output.
    ''' </summary>
    ''' <remarks>
    ''' This event is raised to indicate that data has been printed by an executing
    ''' host command.
    ''' </remarks>
    Public Event PrinterData(ByVal sender As ThalesMain, ByVal s As String)

    ''' <summary>
    ''' Initialization method.
    ''' </summary>
    ''' <remarks>
    ''' This method initializes the object and starts up processing.
    ''' </remarks>
    Public Sub StartUp(ByVal XMLParameterFile As String)

        StartCrypto(XMLParameterFile)
        StartTCP()

    End Sub

    ''' <summary>
    ''' Initialization method for testing.
    ''' </summary>
    ''' <remarks>
    ''' This method initializes the object for testing.
    ''' </remarks>
    Public Sub StartUpWithoutTCP(ByVal XMLParameterFile As String)

        StartCrypto(XMLParameterFile)

    End Sub

    ''' <summary>
    ''' Return a human-readable string with the configuration.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SayConfiguration() As String
        Return "Host command port: " + port.ToString + vbCrLf + _
               "Console port: " + consolePort.ToString + vbCrLf + _
               "Maximum connections: " + maxCons.ToString + vbCrLf + _
               "Log level: " + Logger.CurrentLogLevel.ToString + vbCrLf + _
               "Check LMK parity: " + CheckLMKParity.ToString + vbCrLf + _
               "XML host command definitions: " + HostDefsDir + vbCrLf + _
               "Use double-length ZMKs: " + DoubleLengthZMKs.ToString
    End Function

    ''' <summary>
    ''' Startup TCP.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub StartTCP()

        StartThread(LT, AddressOf ListenerThread, "TCP listening")
        StartThread(CLT, AddressOf ConsoleListenerThread, "Console TCP listening")

        Logger.MajorInfo("Startup complete")

    End Sub

    ''' <summary>
    ''' Starts a new thread that hosts a tcp listener.
    ''' </summary>
    ''' <param name="t">Thread variable of thread to start.</param>
    ''' <param name="threadStart">Thread entry point</param>
    ''' <param name="threadMsg">Debug message.</param>
    ''' <remarks></remarks>
    Private Sub StartThread(ByRef t As Threading.Thread, ByVal threadStart As System.Threading.ThreadStart, ByVal threadMsg As String)

        Logger.MajorVerbose(String.Format("Starting up the {0} thread...", threadMsg))
        t = New Threading.Thread(threadStart)
        t.IsBackground = True
        Try
            t.Start()
            Dim cntr As Integer = 0
            Threading.Thread.Sleep(100)
        Catch ex As Exception
            Logger.MajorError(String.Format("Error while starting the {0} thread: " + ex.ToString(), threadMsg))
            Throw ex
        End Try

    End Sub

    ''' <summary>
    ''' Starts the crypto only.
    ''' </summary>
    ''' <param name="XMLParameterFile">Full or relative path to XML parameters file.</param>
    ''' <remarks></remarks>
    Private Sub StartCrypto(ByVal XMLParameterFile As String)
        Logger.LogInterface = Me

        ThalesSim.Core.Resources.CleanUp()

        If Not ReadXMLFile(XMLParameterFile) Then
            Logger.MajorError("Trying to load key/value file for Mono...")
            If Not TryToReadValuePairFile(XMLParameterFile) Then
                Logger.MajorDebug("Using default configuration...")
                SetDefaultConfiguration()
            End If
        End If

        'Parse the loaded host commands
        Logger.MajorDebug("Searching for host command implementors...")
        CE = New HostCommands.CommandExplorer
        Logger.MinorInfo("Loaded commands dump" + vbCrLf + CE.GetLoadedCommands())

        'Parse the loaded console commands
        Logger.MajorDebug("Searching for console command implementors...")
        CCE = New ConsoleCommands.ConsoleCommandExplorer
        Logger.MinorInfo("Loaded console commands dump " + vbCrLf + CCE.GetLoadedCommands())
    End Sub

    ''' <summary>
    ''' Attempts to read an XML file with the parameters and start the crypto.
    ''' </summary>
    ''' <param name="fileName">XML file name.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ReadXMLFile(ByVal fileName As String) As Boolean
        Try
            'Try to load from the configuration file.
            Logger.MajorDebug("Reading XML configuration...")

            Dim reader As New Xml.XmlTextReader(fileName)
            reader.WhitespaceHandling = Xml.WhitespaceHandling.None
            reader.MoveToContent()
            reader.Read()

            Dim doc As New Xml.XmlDocument
            doc.Load(reader)

            port = Convert.ToInt32(GetParameterValue(doc, "Port"))
            consolePort = Convert.ToInt32(GetParameterValue(doc, "ConsolePort"))
            maxCons = Convert.ToInt32(GetParameterValue(doc, "MaxConnections"))
            LMKFile = Convert.ToString(GetParameterValue(doc, "LMKStorageFile"))
            VBsources = Convert.ToString(GetParameterValue(doc, "VBSourceDirectory"))
            Logger.CurrentLogLevel = DirectCast([Enum].Parse(GetType(Logger.LogLevel), Convert.ToString(GetParameterValue(doc, "LogLevel")), True), Logger.LogLevel)
            CheckLMKParity = Convert.ToBoolean(GetParameterValue(doc, "CheckLMKParity"))
            HostDefsDir = Convert.ToString(GetParameterValue(doc, "XMLHostDefinitionsDirectory"))
            DoubleLengthZMKs = Convert.ToBoolean(GetParameterValue(doc, "DoubleLengthZMKs"))

            StartUpCore(Convert.ToString(GetParameterValue(doc, "FirmwareNumber")), _
                        Convert.ToString(GetParameterValue(doc, "DSPFirmwareNumber")), _
                        Convert.ToBoolean(GetParameterValue(doc, "StartInAuthorizedState")), _
                        Convert.ToInt32(GetParameterValue(doc, "ClearPINLength")))

            reader.Close()
            reader = Nothing
            Return True
        Catch ex As Exception
            Logger.MajorError("Error loading the configuration file")
            Logger.MajorError(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Attempts to read a key/value pair file with the parameters and start the crypto.
    ''' </summary>
    ''' <param name="fileName">File to read.</param>
    ''' <remarks>This is used in order to read the parameters under Mono where, for some
    ''' reason, we get an exception when trying to load the XML document from the reader.
    ''' We expect a file with the following format:
    ''' - A starting ; denotes a comment and is ignored.
    ''' - Other lines must have a Key=Value format and we expect the folliwng keys:
    '''   * Port
    '''   * ConsolePort
    '''   * MaxConnections
    '''   * LMKStorageFile
    '''   * VBSourceDirectory
    '''   * XMLHostDefinitionsDirectory
    '''   * LogLevel
    '''   * CheckLMKParity
    '''   * FirmwareNumber
    '''   * DSPFirmwareNumber
    '''   * StartInAuthorizedState
    '''   * ClearPINLength
    ''' </remarks>
    Private Function TryToReadValuePairFile(ByVal fileName As String) As Boolean
        Try
            Dim list As New SortedList(Of String, String)
            Using SR As IO.StreamReader = New IO.StreamReader(fileName, System.Text.Encoding.Default)
                While SR.Peek > -1
                    Dim s As String = SR.ReadLine
                    If Not (String.IsNullOrEmpty(s) OrElse s.StartsWith(";"c)) Then
                        Dim sSplit() As String = s.Split("="c)
                        list.Add(sSplit(0).ToUpper, sSplit(1))
                    End If
                End While
            End Using

            port = Convert.ToInt32(list("PORT"))
            consolePort = Convert.ToInt32(list("CONSOLEPORT"))
            maxCons = Convert.ToInt32(list("MAXCONNECTIONS"))
            LMKFile = list("LMKSTORAGEFILE")
            VBsources = list("VBSOURCEDIRECTORY")
            Logger.CurrentLogLevel = DirectCast([Enum].Parse(GetType(Logger.LogLevel), Convert.ToString(list("LOGLEVEL")), True), Logger.LogLevel)
            CheckLMKParity = Convert.ToBoolean(list("CHECKLMKPARITY"))
            HostDefsDir = list("XMLHOSTDEFINITIONSDIRECTORY")
            DoubleLengthZMKs = Convert.ToBoolean(list("DOUBLELENGTHZMKS"))

            If HostDefsDir = "" Then HostDefsDir = Utility.GetExecutingDirectory
            If VBsources = "" Then VBsources = Utility.GetExecutingDirectory

            StartUpCore(list("FIRMWARENUMBER"), _
                        list("DSPFIRMWARENUMBER"), _
                        Convert.ToBoolean(list("STARTINAUTHORIZEDSTATE")), _
                        Convert.ToInt32(list("CLEARPINLENGTH")))

            Return True
        Catch ex As Exception
            Logger.MajorError("Error loading key/value file")
            Logger.MajorError(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Starts the crypto with default parameters.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SetDefaultConfiguration()
        Logger.MajorDebug("Using default configuration...")
        port = 9998
        consolePort = 9997
        maxCons = 5
        LMKFile = ""
        VBsources = Utility.GetExecutingDirectory
        Logger.CurrentLogLevel = Logger.LogLevel.Debug
        CheckLMKParity = True
        HostDefsDir = Utility.GetExecutingDirectory
        DoubleLengthZMKs = True

        StartUpCore("0007-E000", _
                    "0001", _
                    True, _
                    4)
    End Sub

    Private Sub StartUpCore(ByVal firmwareNumber As String, _
                            ByVal dspFirmwareNumber As String, ByVal startInAuthorizedState As Boolean, _
                            ByVal clearPINLength As Integer)
        ''This has been removed for the mobile version.
        ''CompileAndLoad(VBSources)

        Resources.AddResource(Resources.CONSOLE_PORT, consolePort)
        Resources.AddResource(Resources.WELL_KNOWN_PORT, port)
        Resources.AddResource(Resources.FIRMWARE_NUMBER, firmwareNumber)
        Resources.AddResource(Resources.DSP_FIRMWARE_NUMBER, dspFirmwareNumber)
        Resources.AddResource(Resources.MAX_CONS, maxCons)
        Resources.AddResource(Resources.AUTHORIZED_STATE, startInAuthorizedState)
        Resources.AddResource(Resources.CLEAR_PIN_LENGTH, clearPINLength)
        Resources.AddResource(Resources.DOUBLE_LENGTH_ZMKS, DoubleLengthZMKs)

        'Make sure it ends with a directory separator, both for Windows and Linux.
        HostDefsDir = Utility.AppendDirectorySeparator(HostDefsDir)

        Resources.AddResource(Resources.HOST_COMMANDS_XML_DEFS, HostDefsDir)

        If LMKFile = "" Then
            Logger.MajorInfo("No LMK storage file specified, creating new keys")
            ThalesSim.Core.Cryptography.LMK.LMKStorage.LMKStorageFile = "LMKSTORAGE.TXT"
            ThalesSim.Core.Cryptography.LMK.LMKStorage.GenerateTestLMKs()
        Else
            Logger.MajorDebug("Reading LMK storage")
            ThalesSim.Core.Cryptography.LMK.LMKStorage.ReadLMKs(LMKFile)
        End If

        Resources.AddResource(Core.Resources.LMK_CHECK_VALUE, Cryptography.LMK.LMKStorage.GenerateLMKCheckValue())
    End Sub

    Private Function GetParameterValue(ByVal doc As Xml.XmlDocument, ByVal element As String) As String
        Return doc.DocumentElement(element).Attributes("value").Value
    End Function

    ''' <summary>
    ''' Stops processing.
    ''' </summary>
    ''' <remarks>
    ''' This method stops processing.
    ''' </remarks>
    Public Sub ShutDown()

        If Not LT Is Nothing Then

            Try
                SL.Stop()
                SL = Nothing
            Catch ex As Exception
            End Try

            Logger.MajorVerbose("Stopping the listening thread...")
            Try
                LT.Abort()
                LT = Nothing
            Catch ex As Exception
            End Try

            Logger.MajorVerbose("Disconnecting connected clients...")

            For i As Integer = 0 To WC.GetUpperBound(0)
                Try
                    If Not WC(i) Is Nothing AndAlso WC(i).IsConnected = True Then WC(i).TermClient()
                    WC(i) = Nothing
                Catch ex As Exception
                End Try
            Next

            Try
                CSL.Stop()
                CSL = Nothing
            Catch ex As Exception
            End Try

            Logger.MajorVerbose("Stopping the console listening thread...")
            Try
                CLT.Abort()
                CLT = Nothing
            Catch ex As Exception
            End Try

            Try
                If Not CWC Is Nothing AndAlso CWC.IsConnected Then CWC.TermClient()
                CWC = Nothing
            Catch ex As Exception
            End Try

        End If

        Logger.MajorInfo("Shutdown complete")

    End Sub

    'Thread for the console listening socket.
    Private Sub ConsoleListenerThread()
        Try
            CSL = New TcpListener(New System.Net.IPEndPoint(0, consolePort))
            CSL.Start()

            While True
                CWC = New TCP.WorkerClient(CSL.AcceptTcpClient())
                CWC.InitOps()

                AddHandler CWC.Disconnected, AddressOf CWCDisconnected
                AddHandler CWC.MessageArrived, AddressOf CWCMessageArrived

                Logger.MajorInfo("Console client from " + CWC.ClientIP + " is connected")

                'If we have one connection, don't accept others.
                consoleCurCons = 1
                While consoleCurCons = 1
                    Threading.Thread.Sleep(50)
                End While
            End While
        Catch ex As Exception
            Logger.MajorInfo("Exception on console listening thread (" + ex.Message + ")")
            If Not CSL Is Nothing Then
                CSL.Stop()
                CSL = Nothing
            End If
        End Try
    End Sub

    'Thread for the host listening socket.
    Private Sub ListenerThread()

        ReDim WC(-1)

        Try

            SL = New TcpListener(New System.Net.IPEndPoint(0, port))
            SL.Start()

            While True
                Dim wClient As New TCP.WorkerClient(SL.AcceptTcpClient())
                wClient.InitOps()

                AddHandler wClient.Disconnected, AddressOf WCDisconnected
                AddHandler wClient.MessageArrived, AddressOf WCMessageArrived

                Logger.MajorInfo("Client from " + wClient.ClientIP + " is connected")

                curCons += 1

                Dim slotedIt As Boolean = False

                For i As Integer = 0 To WC.GetUpperBound(0)
                    If WC(i) Is Nothing OrElse WC(i).IsConnected = False Then
                        WC(i) = wClient
                        slotedIt = True
                        Exit For
                    End If
                Next

                If slotedIt = False Then
                    ReDim Preserve WC(WC.GetLength(0))
                    WC(WC.GetUpperBound(0)) = wClient
                End If

                While curCons >= maxCons
                    Threading.Thread.Sleep(50)
                End While

            End While

        Catch ex As Exception
            Logger.MajorInfo("Exception on listening thread (" + ex.Message + ")")
            If Not SL Is Nothing Then
                SL.Stop()
                SL = Nothing
            End If
        End Try

    End Sub

    'Console client disconnect event
    Private Sub CWCDisconnected(ByVal sender As TCP.WorkerClient)

        Logger.MajorInfo("Console client disconnected.")
        sender.TermClient()

        'Indicate that the console is off
        consoleCurCons -= 1

        curMsg = Nothing

    End Sub

    'Console client data event
    Private Sub CWCMessageArrived(ByVal sender As TCP.WorkerClient, ByRef b() As Byte, ByVal len As Integer)

        '' Data for console commands do not all arrive at once. For most console commands, the console
        '' prompts the user to enter information during a series of steps, then the command is executed
        '' when all information has been gathered.
        ''
        '' This event handler is coded in order to reflect that. During the first message arrival, an
        '' appropriate implementor of a console command is searched. If one is found, an object is
        '' created and kept in the curMsg variable. This is used to accept keyed data from the console
        '' and prompt the user for the next part of information to be entered. Once all information has
        '' been entered, curMsg performs the processing and returns the result.
        ''

        'We're using a Message only to get a string back. No other relation to processing.
        Dim msg As New ThalesSim.Core.Message.Message(b)

        Try
            'Do we have a current command?
            If curMsg Is Nothing Then
                'No, find the appropriate one.
                Logger.MajorVerbose("Client: " + sender.ClientIP + vbCrLf + _
                                    "Request: " + msg.MessageData())
                Logger.MajorDebug("Searching for implementor of " + msg.MessageData + "...")
                Dim CC As ConsoleCommands.ConsoleCommandClass = CCE.GetLoadedCommand(msg.MessageData)
                If CC Is Nothing Then
                    Logger.MajorError("No implementor for " + msg.MessageData + ".")
                    sender.send("Command not found" + vbCrLf)
                    Exit Sub
                End If

                'Instantiate and let it initialize its command stack.
                curMsg = CType(Activator.CreateInstance(CC.CommandType), ConsoleCommands.AConsoleCommand)
                curMsg.InitializeStack()
            Else
                'We already have a command so we'll pass the data from the console to it.
                Dim returnMsg As String = Nothing

                'This catches exceptions of the last process.
                Try
                    returnMsg = curMsg.AcceptMessage(msg.MessageData)
                Catch ex As Exception
                    returnMsg = ex.Message
                End Try

                'If it returns some string and it signaled a finish, we're done with the command.
                If returnMsg IsNot Nothing AndAlso curMsg.CommandFinished Then
                    sender.send(returnMsg + vbCrLf)
                    curMsg = Nothing
                Else
                    'Else, let the command send the next prompt to the console.
                    sender.send(curMsg.GetClientMessage())
                End If
                Exit Sub
            End If

            'This is reached when a command has just been instantiated.
            'There are some commands that require no input. If this is one
            'of them, just run the ProcessMessage method and return the result.
            If curMsg.IsNoinputCommand Then
                Try
                    sender.send(curMsg.ProcessMessage + vbCrLf)
                Catch ex As Exception
                    sender.send(ex.Message)
                End Try
                curMsg = Nothing
            Else
                'Else, let the command send the first prompt to the console.
                sender.send(curMsg.GetClientMessage())
            End If

        Catch ex As Exception
            Logger.MajorError("Exception while parsing message or creating implementor instance" + vbCrLf + ex.ToString())
            Logger.MajorError("Disconnecting client.")
            sender.TermClient()
            curMsg = Nothing
        End Try

    End Sub

    'Host disconnect event
    Private Sub WCDisconnected(ByVal sender As TCP.WorkerClient)

        Logger.MajorInfo("Client disconnected.")
        sender.TermClient()
        curCons -= 1

    End Sub

    'Host date event
    Private Sub WCMessageArrived(ByVal sender As TCP.WorkerClient, ByRef b() As Byte, ByVal len As Integer)

        Dim msg As New ThalesSim.Core.Message.Message(b)

        Logger.MajorVerbose("Client: " + sender.ClientIP + vbCrLf + _
                            "Request: " + msg.MessageData())

        Try
            Logger.MajorDebug("Parsing header and code of message " + msg.MessageData + "...")

            Dim messageHeader As String = msg.GetSubstring(4)
            msg.AdvanceIndex(4)
            Dim commandCode As String = msg.GetSubstring(2)
            msg.AdvanceIndex(2)

            RaiseEvent CommandCalled(Me, commandCode)

            Logger.MajorDebug("Searching for implementor of " + commandCode + "...")
            Dim CC As ThalesSim.Core.HostCommands.CommandClass = CE.GetLoadedCommand(commandCode)

            If CC Is Nothing Then
                Logger.MajorError("No implementor for " + commandCode + "." + vbCrLf + _
                                  "Disconnecting client.")
                sender.TermClient()
            Else
                Logger.MajorDebug("Found implementor " + CC.DeclaringType.FullName() + ", instantiating...")
                Dim o As ThalesSim.Core.HostCommands.AHostCommand
                o = CType(Activator.CreateInstance(CC.DeclaringType), HostCommands.AHostCommand)

                Dim retMsg As ThalesSim.Core.Message.MessageResponse
                Dim retMsgAfterIO As ThalesSim.Core.Message.MessageResponse = Nothing

                Try
                    If CheckLMKParity = False OrElse Cryptography.LMK.LMKStorage.CheckLMKStorage() = True Then
                        Logger.MinorInfo("=== [" + commandCode + "], starts " + Utility.getTimeMMHHSSmmmm + " =======")

                        Logger.MajorDebug("Calling AcceptMessage()...")
                        o.AcceptMessage(msg)

                        Logger.MinorVerbose(o.DumpFields())

                        If o.XMLParseResult <> ErrorCodes.ER_00_NO_ERROR Then
                            Logger.MajorDebug("Error condition encountered during message parsing.")
                            Logger.MajorDebug(String.Format("Error code {0} will be returned without calling ConstructResponse().", o.XMLParseResult))
                            retMsg = New Core.Message.MessageResponse
                            retMsg.AddElement(o.XMLParseResult)
                        Else
                            Logger.MajorDebug("Calling ConstructResponse()...")
                            retMsg = o.ConstructResponse()
                            Logger.MajorDebug("Calling ConstructResponseAfterOperationComplete()...")
                            retMsgAfterIO = o.ConstructResponseAfterOperationComplete()
                        End If

                        Logger.MajorDebug("Attaching header/response code to response...")
                        retMsg.AddElementFront(CC.ResponseCode)
                        retMsg.AddElementFront(messageHeader)

                        Logger.MajorVerbose("Sending: " + retMsg.MessageData())
                        sender.send(retMsg.MessageData())

                        If retMsgAfterIO IsNot Nothing Then
                            Logger.MajorDebug("Attaching header/response code to response after I/O...")
                            retMsgAfterIO.AddElementFront(CC.ResponseCodeAfterIO)
                            retMsgAfterIO.AddElementFront(messageHeader)
                            Logger.MajorVerbose("Sending: " + retMsgAfterIO.MessageData())
                            sender.send(retMsgAfterIO.MessageData())
                        End If

                        Logger.MinorInfo("=== [" + commandCode + "],   ends " + Utility.getTimeMMHHSSmmmm + " =======" + vbCrLf)
                    Else
                        Logger.MajorError("LMK parity error")
                        retMsg = New Message.MessageResponse
                        retMsg.AddElementFront(Core.ErrorCodes.ER_13_MASTER_KEY_PARITY_ERROR)
                        retMsg.AddElementFront(CC.ResponseCode)
                        retMsg.AddElementFront(messageHeader)
                        sender.send(retMsg.MessageData())
                    End If

                Catch ex As Exception
                    Logger.MajorError("Exception while processing message" + vbCrLf + ex.ToString())
                    Logger.MajorError("Disconnecting client.")
                    sender.TermClient()
                End Try

                If o.PrinterData <> "" Then
                    RaiseEvent PrinterData(Me, o.PrinterData)
                End If

                Logger.MajorDebug("Calling Terminate()...")
                o.Terminate()
                Logger.MajorDebug("Implementor to Nothing")
                o = Nothing

            End If

        Catch ex As Exception
            Logger.MajorError("Exception while parsing message or creating implementor instance" + vbCrLf + ex.ToString())
            Logger.MajorError("Disconnecting client.")
            sender.TermClient()
        End Try

    End Sub

    Private Sub GetMajor(ByVal s As String) Implements Log.ILogProcs.GetMajor
        RaiseEvent MajorLogEvent(Me, s)
    End Sub

    Private Sub GetMinor(ByVal s As String) Implements Log.ILogProcs.GetMinor
        RaiseEvent MinorLogEvent(Me, s)
    End Sub

End Class
