Public Class ThalesSimService

    Dim WithEvents o As ThalesSim.Core.ThalesMain

    Protected Overrides Sub OnStart(ByVal args() As String)
        Try
            Debug.WriteLine("Starting Thales Simulator...")
            Dim configurationAppSettings As System.Configuration.AppSettingsReader = New System.Configuration.AppSettingsReader
            o = New ThalesSim.Core.ThalesMain
            o.StartUp(CType(configurationAppSettings.GetValue("ParameterFile", GetType(String)), String))
            Debug.WriteLine("Simulator started.")
        Catch ex As Exception
            o.ShutDown()
            o = Nothing
            Debug.WriteLine("Error on simulator startup: " + ex.ToString())
            Throw New Exception("Error on simulator startup: " + ex.ToString())
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        If Not o Is Nothing Then
            Debug.WriteLine("Stopping Racal Simulator Service...")
            RemoveHandler o.MajorLogEvent, AddressOf o_MajorLogEvent
            RemoveHandler o.MinorLogEvent, AddressOf o_MinorLogEvent
            RemoveHandler o.PrinterData, AddressOf o_PrinterData
            o.ShutDown()
            o = Nothing
            Debug.WriteLine("Simulator stopped.")
        End If
    End Sub

    Private Sub o_MajorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles o.MajorLogEvent
        Debug.WriteLine("Application> " + s)
    End Sub

    Private Sub o_MinorLogEvent(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles o.MinorLogEvent
        Debug.WriteLine("Command> " + s)
    End Sub

    Private Sub o_PrinterData(ByVal sender As ThalesSim.Core.ThalesMain, ByVal s As String) Handles o.PrinterData
        Debug.WriteLine("Printer> " + s)
    End Sub

End Class
