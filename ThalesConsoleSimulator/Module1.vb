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

Imports ThalesSim

Module Module1

    Dim WithEvents sim As Core.ThalesMain
    Dim bShowMajor As Boolean = True, bShowMinor As Boolean = True, bShowPrinter As Boolean = True


    Sub Main()
        StartSimulator()

        'Hide cursor, show or title and do not break on Ctrl-C.
        Console.CursorVisible = False
        Console.Title = "Thales Simulator"
        Console.TreatControlCAsInput = True

        While True
            'Sleep and wait for a key.
            Threading.Thread.Sleep(5)
            If Console.KeyAvailable Then
                Dim key As SimKey = GetKey()
                Select Case key
                    Case SimKey.F2
                        bShowMajor = Not bShowMajor
                        WriteStatusLine("Major logging enabled: " + bShowMajor.ToString)
                    Case SimKey.F3
                        bShowMinor = Not bShowMinor
                        WriteStatusLine("Minor logging enabled: " + bShowMinor.ToString)
                    Case SimKey.F4
                        bShowPrinter = Not bShowPrinter
                        WriteStatusLine("Printer logging enabled: " + bShowPrinter.ToString)
                    Case SimKey.F5
                        Select Case Core.Log.Logger.CurrentLogLevel
                            Case Core.Log.Logger.LogLevel.NoLogging
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Errror
                            Case Core.Log.Logger.LogLevel.Errror
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Warning
                            Case Core.Log.Logger.LogLevel.Warning
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Info
                            Case Core.Log.Logger.LogLevel.Info
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Verbose
                            Case Core.Log.Logger.LogLevel.Verbose
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Debug
                            Case Core.Log.Logger.LogLevel.Debug
                                Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.NoLogging
                        End Select
                        WriteStatusLine("Log level set to: " + Core.Log.Logger.CurrentLogLevel.ToString)
                    Case SimKey.F6
                        Core.Resources.UpdateResource(Core.Resources.AUTHORIZED_STATE, Not Convert.ToBoolean(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE)))
                        ShowStatus()
                    Case SimKey.CtrlC
                        sim.ShutDown()
                        sim = Nothing
                        Exit Sub
                End Select
            End If
        End While

    End Sub

    'Show a line indicating an action and also refresh our status.
    Private Sub WriteStatusLine(ByVal s As String)
        Console.WriteLine(s)
        ShowStatus()
    End Sub

    'Get a key and translate it to our known options.
    Private Function GetKey() As SimKey
        Dim cki As ConsoleKeyInfo = Console.ReadKey(True)
        Select Case cki.Key
            Case ConsoleKey.F2
                Return SimKey.F2
            Case ConsoleKey.F3
                Return SimKey.F3
            Case ConsoleKey.F4
                Return SimKey.F4
            Case ConsoleKey.F5
                Return SimKey.F5
            Case ConsoleKey.F6
                Return SimKey.F6
            Case ConsoleKey.C
                If cki.Modifiers And ConsoleModifiers.Control Then
                    Return SimKey.CtrlC
                End If
        End Select
        Return SimKey.Other
    End Function

    'Start the simulator up.
    Private Sub StartSimulator()
        Dim path As String = New IO.FileInfo(Reflection.Assembly.GetExecutingAssembly.Location).DirectoryName
        If path.IndexOf("/") > -1 Then
            path = Core.Utility.AppendDirectorySeparator(path) + "ThalesMonoParameters.exe"
        Else
            path = Core.Utility.AppendDirectorySeparator(path) + "ThalesParameters.xml"
        End If

        Core.Log.Logger.CurrentLogLevel = Core.Log.Logger.LogLevel.Debug
        sim = New Core.ThalesMain()
        sim.StartUp(path)
    End Sub

    'Show our status.
    Private Sub ShowStatus()
        Const TOP_LINE As String = "<F2>/<F3>/<F4>Toggle major/minor/printer logging   <CTRL-C>Quit"
        Const SECOND_LINE As String = "<F5>Toggle log detail  <F6>Toggle authorized mode"

        'Save position.
        Dim x As Integer = Console.CursorLeft, y As Integer = Console.CursorTop

        'Go to top of visible console window.
        If y > Console.WindowHeight Then
            Console.SetCursorPosition(0, y - Console.WindowHeight + 1)
        Else
            Console.SetCursorPosition(0, 0)
        End If

        'High contrast.
        Console.BackgroundColor = ConsoleColor.White
        Console.ForegroundColor = ConsoleColor.Black
        Console.Write(TOP_LINE.PadRight(Console.WindowWidth, " "c))
        Console.Write(SECOND_LINE)

        Try
            Dim authMode As Boolean = Convert.ToBoolean(Core.Resources.GetResource(Core.Resources.AUTHORIZED_STATE))
            Dim authStr As String = ""
            If authMode Then
                Console.ForegroundColor = ConsoleColor.Red
                authStr = "    *AUTHORIZED*"
            Else
                Console.ForegroundColor = ConsoleColor.Green
                authStr = "    NORMAL"
            End If
            Console.Write(authStr.PadRight(Console.WindowWidth - SECOND_LINE.Length, " "c))
        Catch ex As Exception
            'When sim is not completely initialized, we get an exception.
        End Try

        'Restore colors and position.
        Console.SetCursorPosition(x, y)
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
    End Sub

    'Simulator major event.
    Private Sub sim_MajorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles sim.MajorLogEvent
        If bShowMajor Then
            Console.WriteLine("MAJOR>>>" + s)
            ShowStatus()
        End If
    End Sub

    'Simulator minor event.
    Private Sub sim_MinorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles sim.MinorLogEvent
        If bShowMinor Then
            Console.WriteLine("MINOR>>>" + s)
            ShowStatus()
        End If
    End Sub

    'Simulator printer data.
    Private Sub sim_PrinterData(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles sim.PrinterData
        If bShowPrinter Then
            Console.WriteLine("PRINTER>>>" + s)
            ShowStatus()
        End If
    End Sub
End Module

'Enumeration with our keys.
Public Enum SimKey
    F2 = 0
    F3
    F4
    F5
    F6
    CtrlC
    Other
End Enum
