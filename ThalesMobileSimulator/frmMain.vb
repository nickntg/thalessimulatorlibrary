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

Public Class frmMain

    Dim WithEvents sim As ThalesMain
    Dim cmdsProcessed As Integer = 0

    Private Delegate Sub ShowLogString(ByVal msg As String)

    'Stars/stops the simulator.
    Private Sub lnkStartStopSim_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkStartStopSim.Click
        If sim IsNot Nothing Then
            sim.ShutDown()
            Me.Close()
        Else
            Try
                sim = New ThalesMain()
                sim.StartUp("nofile.xml")
                Log.Logger.CurrentLogLevel = Log.Logger.LogLevel.Errror
                txtLog.Text = sim.SayConfiguration + vbCrLf + vbCrLf + "IP Addresses list "
                Dim HE As System.Net.IPHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostEntry("localhost").HostName)
                For i As Integer = 0 To HE.AddressList.GetLength(0) - 1
                    txtLog.Text = txtLog.Text + vbCrLf + HE.AddressList(i).ToString
                Next
                lnkStartStopSim.Text = " Exit simulator "
                lnkChangeAuth.Enabled = True
            Catch ex As Exception
                sim = Nothing
                txtLog.Text = "Startup error: " + ex.Message
            End Try
        End If
    End Sub

    'Shows a string in the message box.
    Private Sub ShowString(ByVal msg As String)
        txtLog.Text = msg
    End Sub

    'A command has been called.
    Private Sub sim_CommandCalled(ByVal sender As ThalesMain, ByVal commandCode As String) Handles sim.CommandCalled
        cmdsProcessed += 1
        Me.Invoke(New ShowLogString(AddressOf ShowString), New String() {"Command " + commandCode + " called." + vbCrLf + "Host command #" + cmdsProcessed.ToString})
    End Sub

    'Major event.
    Private Sub sim_MajorLogEvent(ByVal sender As ThalesMain, ByVal s As String) Handles sim.MajorLogEvent
        Me.Invoke(New ShowLogString(AddressOf ShowString), New String() {s})
    End Sub

    'Minor event.
    Private Sub sim_MinorLogEvent(ByVal sender As ThalesMain, ByVal s As String) Handles sim.MinorLogEvent
        Me.Invoke(New ShowLogString(AddressOf ShowString), New String() {s})
    End Sub

    'Chanve the authorized state.
    Private Sub lnkChangeAuth_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lnkChangeAuth.Click
        Resources.UpdateResource(Resources.AUTHORIZED_STATE, _
                                 Not Convert.ToBoolean(Resources.GetResource(Resources.AUTHORIZED_STATE)))
        Dim authMode As Boolean = Convert.ToBoolean(Resources.GetResource(Resources.AUTHORIZED_STATE))
        If authMode Then
            txtLog.Text = "In authorized mode"
        Else
            txtLog.Text = "Not in authorized mode"
        End If
    End Sub

    'Center controls and make textbox get max space.
    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        lnkChangeAuth.Left = (Me.Width - lnkChangeAuth.Width) / 2
        lnkStartStopSim.Left = (Me.Width - lnkStartStopSim.Width) / 2
        txtLog.Width = Me.Width - txtLog.Left * 2
        txtLog.Height = Me.Height - txtLog.Top - 2
    End Sub

End Class