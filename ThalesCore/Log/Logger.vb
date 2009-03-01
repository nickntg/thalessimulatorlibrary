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

Namespace Log

    ''' <summary>
    ''' Logging class.
    ''' </summary>
    ''' <remarks>
    ''' The Logger class provides application-wide logging facilities.
    ''' </remarks>
    Public Class Logger

        ''' <summary>
        ''' Granularity of the logging level.
        ''' </summary>
        ''' <remarks>
        ''' Defines the logging level granularity.
        ''' </remarks>
        Public Enum LogLevel
            ''' <summary>
            ''' No logging.
            ''' </summary>
            ''' <remarks>
            ''' No logging.
            ''' </remarks>
            NoLogging = 0
            ''' <summary>
            ''' Log errors only.
            ''' </summary>
            ''' <remarks>
            ''' Log errors only.
            ''' </remarks>
            Errror = 1
            ''' <summary>
            ''' Same as Error, plus warning messages.
            ''' </summary>
            ''' <remarks>
            ''' Same as Error, plus warning messages.
            ''' </remarks>
            Warning = 2
            ''' <summary>
            ''' Same as Warning, plus informational messages.
            ''' </summary>
            ''' <remarks>
            ''' Same as Warning, plus informational messages.
            ''' </remarks>
            Info = 3
            ''' <summary>
            ''' Same as Info, plus verbose messages.
            ''' </summary>
            ''' <remarks>
            ''' Same as Info, plus verbose messages.
            ''' </remarks>
            Verbose = 4
            ''' <summary>
            ''' Logs everything.
            ''' </summary>
            ''' <remarks>
            ''' Logs everything.
            ''' </remarks>
            Debug = 5
        End Enum

        Private Shared curLogLevel As LogLevel = LogLevel.NoLogging
        Private Shared ILP As ILogProcs = Nothing

        ''' <summary>
        ''' Gets or sets the current logging level.
        ''' </summary>
        ''' <remarks>
        ''' Gets or sets the current logging level.
        ''' </remarks>
        Public Shared Property CurrentLogLevel() As LogLevel
            Get
                Return curLogLevel
            End Get
            Set(ByVal Value As LogLevel)
                curLogLevel = Value
            End Set
        End Property

        ''' <summary>
        ''' Sets the logging interface.
        ''' </summary>
        ''' <remarks>
        ''' Sets the logging interface.
        ''' </remarks>
        Public Shared WriteOnly Property LogInterface() As ILogProcs
            Set(ByVal Value As ILogProcs)
                ILP = Value
            End Set
        End Property

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action. The same applies if the passed logging level is lower than the
        ''' <see cref="Logger.CurrentLogLevel"/>.
        ''' </remarks>
        Public Shared Sub Major(ByVal s As String, ByVal level As LogLevel)
            If Not ILP Is Nothing Then
                If Convert.ToInt32(level) <= Convert.ToInt32(curLogLevel) Then
                    ILP.GetMajor(s)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major error event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MajorError(ByVal s As String)
            Major(s, LogLevel.Errror)
        End Sub

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major warning event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MajorWarning(ByVal s As String)
            Major(s, LogLevel.Warning)
        End Sub

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major informational event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MajorInfo(ByVal s As String)
            Major(s, LogLevel.Info)
        End Sub

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major verbose event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MajorVerbose(ByVal s As String)
            Major(s, LogLevel.Verbose)
        End Sub

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a major debug event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MajorDebug(ByVal s As String)
            Major(s, LogLevel.Debug)
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action. The same applies if the passed logging level is lower than the
        ''' <see cref="Logger.CurrentLogLevel"/>.
        ''' </remarks>
        Public Shared Sub Minor(ByVal s As String, ByVal level As LogLevel)
            If Not ILP Is Nothing Then
                If Convert.ToInt32(level) <= Convert.ToInt32(curLogLevel) Then
                    ILP.GetMinor(s)
                End If
            End If
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor error event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MinorError(ByVal s As String)
            Minor(s, LogLevel.Errror)
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor warning event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MinorWarning(ByVal s As String)
            Minor(s, LogLevel.Warning)
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor informational event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MinorInfo(ByVal s As String)
            Minor(s, LogLevel.Info)
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor verbose event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MinorVerbose(ByVal s As String)
            Minor(s, LogLevel.Verbose)
        End Sub

        ''' <summary>
        ''' Logs a Minor event.
        ''' </summary>
        ''' <remarks>
        ''' Logs a Minor debug event. If the <see cref="Log.Logger.LogInterface"/> has not been set, the method
        ''' takes no action.
        ''' </remarks>
        Public Shared Sub MinorDebug(ByVal s As String)
            Minor(s, LogLevel.Debug)
        End Sub

    End Class

    ''' <summary>
    ''' Logging interface.
    ''' </summary>
    ''' <remarks>
    ''' The logging interface is used by the <see cref="Logger"/> class to direct
    ''' logging messages to an implementor.
    ''' </remarks>
    Public Interface ILogProcs

        ''' <summary>
        ''' Logs a major event.
        ''' </summary>
        ''' <remarks>
        ''' Called when a major event is logged.
        ''' </remarks>
        Sub GetMajor(ByVal s As String)

        ''' <summary>
        ''' Logs a minor event.
        ''' </summary>
        ''' <remarks>
        ''' Called when a minor event is logged.
        ''' </remarks>
        Sub GetMinor(ByVal s As String)

    End Interface

End Namespace
