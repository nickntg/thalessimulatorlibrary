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

    Private Const HEADER As String = "HEADER"
    Private Const COMMAND_CODE As String = "COMMAND_CODE"

    Private port As Integer
    Private maxCons As Integer
    Private curCons As Integer = 0
    Private LMKFile As String
    Private VBsources As String
    Private CheckLMKParity As Boolean

    Private LT As Threading.Thread
    Private CE As ThalesSim.Core.HostCommands.CommandExplorer

    Private WC() As TCP.WorkerClient

    Private SL As TcpListener

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

    Private Sub StartTCP()

        Logger.MajorVerbose("Starting up the TCP listening thread...")
        LT = New Threading.Thread(AddressOf ListenerThread)
        LT.IsBackground = True
        Try
            LT.Start()
            Dim cntr As Integer = 0
            While LT.ThreadState <> Threading.ThreadState.Running AndAlso _
                  LT.ThreadState <> Threading.ThreadState.Aborted AndAlso _
                  LT.ThreadState <> Threading.ThreadState.Background AndAlso _
                  cntr < 20
                cntr += 1
                Threading.Thread.Sleep(50)
            End While

            If LT.ThreadState <> Threading.ThreadState.Background Then
                LT.Abort()
                LT = Nothing
                Throw New Exception("Timeout on starting the listener thread")
            End If

        Catch ex As Exception
            Logger.MajorError("Error while starting the TCP listening thread: " + ex.ToString())
            Throw ex
        End Try

        Logger.MajorInfo("Startup complete")

    End Sub

    Private Sub StartCrypto(ByVal XMLParameterFile As String)
        Logger.LogInterface = Me

        Try
            ThalesSim.Core.Resources.CleanUp()

            Logger.MajorDebug("Reading XML configuration...")

            Dim reader As New Xml.XmlTextReader(XMLParameterFile)

            reader.WhitespaceHandling = Xml.WhitespaceHandling.None
            reader.MoveToContent()
            reader.Read()

            Dim doc As New Xml.XmlDocument
            doc.Load(reader)

            port = CType(GetParameterValue(doc, "Port"), Integer)
            maxCons = CType(GetParameterValue(doc, "MaxConnections"), Integer)
            LMKFile = CType(GetParameterValue(doc, "LMKStorageFile"), String)
            VBsources = CType(GetParameterValue(doc, "VBSourceDirectory"), String)
            Log.Logger.CurrentLogLevel = CType(GetParameterValue(doc, "LogLevel"), Log.Logger.LogLevel)
            CheckLMKParity = CType(GetParameterValue(doc, "CheckLMKParity"), Boolean)

            CompileAndLoad(VBsources)

            Core.Resources.AddResource(Core.Resources.FIRMWARE_NUMBER, CType(GetParameterValue(doc, "FirmwareNumber"), String))
            Core.Resources.AddResource(Core.Resources.DSP_FIRMWARE_NUMBER, CType(GetParameterValue(doc, "DSPFirmwareNumber"), String))
            Core.Resources.AddResource(Core.Resources.MAX_CONS, maxCons)
            Core.Resources.AddResource(Core.Resources.AUTHORIZED_STATE, CType(GetParameterValue(doc, "StartInAuthorizedState"), Boolean))
            Core.Resources.AddResource(Core.Resources.CLEAR_PIN_LENGTH, CType(GetParameterValue(doc, "ClearPINLength"), Integer))

            If LMKFile = "" Then
                Logger.MajorInfo("No LMK storage file specified, creating new keys")
                ThalesSim.Core.Cryptography.LMK.LMKStorage.LMKStorageFile = "LMKSTORAGE.TXT"
                ThalesSim.Core.Cryptography.LMK.LMKStorage.GenerateTestLMKs()
            Else
                Logger.MajorDebug("Reading LMK storage")
                ThalesSim.Core.Cryptography.LMK.LMKStorage.ReadLMKs(LMKFile)
            End If

            Core.Resources.AddResource(Core.Resources.LMK_CHECK_VALUE, Cryptography.LMK.LMKStorage.GenerateLMKCheckValue())

            reader.Close()
            reader = Nothing
        Catch ex As Exception
            Throw New Exceptions.XInvalidConfiguration(ex.Message())
        End Try

        Logger.MajorDebug("Searching for host command implementors...")
        CE = New ThalesSim.Core.HostCommands.CommandExplorer
        Logger.MinorInfo("Loaded commands dump" + vbCrLf + CE.GetLoadedCommands())
    End Sub

    Private Sub CompileAndLoad(ByVal vbDir As String)

        If vbDir = "" Then Exit Sub

        Dim files() As String = IO.Directory.GetFiles(vbDir, "*.vb")
        For i As Integer = 0 To files.GetUpperBound(0)
            Dim asm As Reflection.Assembly = CompileCode(files(i))
        Next

    End Sub

    Private Function CompileCode(ByVal vbSourceFile As String) As Reflection.Assembly

        Dim vbSource As String = ""
        Dim fName As String = New IO.FileInfo(vbSourceFile).Name
        Log.Logger.MajorVerbose("Compiling " + fName + "...")
        Try
            Dim SR As New IO.StreamReader(vbSourceFile)
            While SR.Peek > -1
                vbSource += SR.ReadLine() + vbCrLf
            End While
            SR.Close()
            SR = Nothing
        Catch ex As Exception
            Log.Logger.MajorError("Exception raised while reading " + fName + vbCrLf + _
                                  ex.ToString())
            Return Nothing
        End Try

        Dim myProvider As Microsoft.VisualBasic.VBCodeProvider
        Dim myCompiler As System.CodeDom.Compiler.ICodeCompiler
        Dim compParams As System.CodeDom.Compiler.CompilerParameters = New System.CodeDom.Compiler.CompilerParameters
        Dim compResults As System.CodeDom.Compiler.CompilerResults

        compParams.GenerateExecutable = False
        compParams.GenerateInMemory = False
        compParams.IncludeDebugInformation = True
        compParams.OutputAssembly = ""
        compParams.TempFiles.KeepFiles = True

        'Add some common refs
        Dim refs() As String = {"System.dll", "Microsoft.VisualBasic.dll", "System.XML.dll", "System.Data.dll", "ThalesCore.dll", Reflection.Assembly.GetAssembly(GetType(ThalesMain)).Location}
        compParams.ReferencedAssemblies.AddRange(refs)

        Try
            myProvider = New Microsoft.VisualBasic.VBCodeProvider
            myCompiler = myProvider.CreateCompiler
            compResults = myCompiler.CompileAssemblyFromSource(compParams, vbSource)
        Catch ex As Exception
            'Oops
            Logger.MajorError("Exception raised during compilation of " + fName + vbCrLf + _
                              ex.ToString())
            Return Nothing
        End Try

        If compResults.Errors.Count > 0 Then
            Logger.MajorError("Compilation errors of " + fName)
            For Each Err As System.CodeDom.Compiler.CompilerError In compResults.Errors
                Logger.MajorError("Line: " + Err.Line.ToString + vbCrLf + _
                                  "Column: " + Err.Column.ToString + vbCrLf + _
                                  "Error: " + Err.ErrorText)
            Next
            Return Nothing
        Else
            Return System.Reflection.Assembly.LoadFrom(compResults.PathToAssembly)
        End If

    End Function

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

        End If

        Logger.MajorInfo("Shutdown complete")

    End Sub

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
            If Not SL Is Nothing Then
                SL.Stop()
                SL = Nothing
            End If
        End Try

    End Sub

    Private Sub WCDisconnected(ByVal sender As TCP.WorkerClient)

        Logger.MajorInfo("Client disconnected.")
        sender.TermClient()
        curCons -= 1

    End Sub

    Private Sub WCMessageArrived(ByVal sender As TCP.WorkerClient, ByRef b() As Byte, ByVal len As Integer)

        Dim MFPC As New ThalesSim.Core.Message.MessageFieldParserCollection
        MFPC.AddMessageFieldParser(New ThalesSim.Core.Message.MessageFieldParser(HEADER, 4))
        MFPC.AddMessageFieldParser(New ThalesSim.Core.Message.MessageFieldParser(COMMAND_CODE, 2))

        Dim msg As New ThalesSim.Core.Message.Message(b)

        Logger.MajorVerbose("Client: " + sender.ClientIP + vbCrLf + _
                            "Request: " + msg.MessageData())

        Try
            Log.Logger.MajorDebug("Parsing header and code of message " + msg.MessageData + "...")
            MFPC.ParseMessage(msg)

            Log.Logger.MajorDebug("Searching for implementor of " + MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue + "...")
            Dim CC As ThalesSim.Core.HostCommands.CommandClass = CE.GetLoadedCommand(MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue)

            If CC Is Nothing Then
                Log.Logger.MajorError("No implementor for " + MFPC.GetMessageFieldByName(COMMAND_CODE).FieldValue + "." + vbCrLf + _
                                      "Disconnecting client.")
                sender.TermClient()
            Else
                Log.Logger.MajorDebug("Found implementor " + CC.DeclaringType.FullName() + ", instantiating...")
                Dim o As ThalesSim.Core.HostCommands.AHostCommand
                'o = Activator.CreateInstance(CC.AssemblyName, CC.DeclaringType).Unwrap()
                o = CType(Activator.CreateInstance(CC.DeclaringType), HostCommands.AHostCommand)

                Dim retMsg As ThalesSim.Core.Message.MessageResponse
                Dim retMsgAfterIO As ThalesSim.Core.Message.MessageResponse

                Try
                    If CheckLMKParity = False OrElse Cryptography.LMK.LMKStorage.CheckLMKStorage() = True Then
                        Log.Logger.MajorDebug("Calling AcceptMessage()...")
                        o.AcceptMessage(msg)

                        DumpMinorRequest(MFPC)
                        Log.Logger.MinorVerbose(o.DumpFields())

                        Log.Logger.MajorDebug("Calling ConstructResponse()...")
                        retMsg = o.ConstructResponse()
                        Log.Logger.MajorDebug("Calling ConstructResponseAfterOperationComplete()...")
                        retMsgAfterIO = o.ConstructResponseAfterOperationComplete()

                        Log.Logger.MajorDebug("Attaching header/response code to response...")
                        retMsg.AddElementFront(CC.ResponseCode)
                        retMsg.AddElementFront(MFPC.GetMessageFieldByName(HEADER).FieldValue)

                        Log.Logger.MajorVerbose("Sending: " + retMsg.MessageData())
                        sender.send(retMsg.MessageData())

                        If Not (retMsgAfterIO Is Nothing) Then
                            Log.Logger.MajorDebug("Attaching header/response code to response after I/O...")
                            retMsgAfterIO.AddElementFront(CC.ResponseCodeAfterIO)
                            retMsgAfterIO.AddElementFront(MFPC.GetMessageFieldByName(HEADER).FieldValue)
                            Log.Logger.MajorVerbose("Sending: " + retMsgAfterIO.MessageData())
                            sender.send(retMsgAfterIO.MessageData())
                        End If
                    Else
                        Log.Logger.MajorError("LMK parity error")
                        retMsg = New Message.MessageResponse
                        retMsg.AddElementFront(Core.ErrorCodes._13_MASTER_KEY_PARITY_ERROR)
                        retMsg.AddElementFront(CC.ResponseCode)
                        retMsg.AddElementFront(MFPC.GetMessageFieldByName(HEADER).FieldValue)
                        sender.send(retMsg.MessageData())
                    End If

                Catch ex As Exception
                    Log.Logger.MajorError("Exception while processing message" + vbCrLf + ex.ToString())
                    Log.Logger.MajorError("Disconnecting client.")
                    sender.TermClient()
                End Try

                If o.PrinterData <> "" Then
                    RaiseEvent PrinterData(Me, o.PrinterData)
                End If

                Log.Logger.MajorDebug("Calling Terminate()...")
                o.Terminate()
                Log.Logger.MajorDebug("Implementor to Nothing")
                o = Nothing

            End If

        Catch ex As Exception
            Log.Logger.MajorError("Exception while parsing message or creating implementor instance" + vbCrLf + ex.ToString())
            Log.Logger.MajorError("Disconnecting client.")
            sender.TermClient()
        End Try

    End Sub

    Private Sub DumpMinorRequest(ByVal MFPC As ThalesSim.Core.Message.MessageFieldParserCollection)
        For i As Integer = 0 To MFPC.MessageFieldCount - 1
            Dim o As Message.MessageFieldParser = MFPC.GetMessageFieldByIndex(i)
            Logger.MinorVerbose("Field " + o.FieldName + ", value " + o.FieldValue)
        Next
    End Sub

    Private Sub GetMajor(ByVal s As String) Implements Log.ILogProcs.GetMajor
        RaiseEvent MajorLogEvent(Me, s)
    End Sub

    Private Sub GetMinor(ByVal s As String) Implements Log.ILogProcs.GetMinor
        RaiseEvent MinorLogEvent(Me, s)
    End Sub

End Class
