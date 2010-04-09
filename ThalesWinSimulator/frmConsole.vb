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

Public Class frmConsole

    Dim WithEvents w As TCP.WorkerClient
    Delegate Sub DisconnectedFromSimulator(ByVal msg As String)
    Delegate Sub MessageFromSimulator(ByVal msg As String)

    'Try to read the console port automatically.
    Private Sub frmConsole_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            txtPort.Text = Convert.ToInt32(Resources.GetResource(Resources.CONSOLE_PORT)).ToString()
        Catch ex As Exception
            'Local sim not started.
            txtPort.Text = "9997"
        End Try
    End Sub

    'On exit, close the connection to the simulator.
    Private Sub frmConsole_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If w IsNot Nothing Then
            Try
                w.TermClient()
            Catch ex As Exception
            End Try

            w = Nothing
        End If
    End Sub

    'Connect to the simulator's console port.
    Private Sub cmdConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdConnect.Click
        Try
            w = New TCP.WorkerClient(New TcpClient(txtIP.Text, Convert.ToInt32(txtPort.Text)))
            w.InitOps()
            txtConsole.AppendText(vbCrLf + "Connected - Type in commands followed by ENTER." + vbCrLf)
            txtConsole.Enabled = True
            txtIP.Enabled = False
            txtPort.Enabled = False
            cmdConnect.Enabled = False
            txtConsole.Focus()
        Catch ex As Exception
            txtConsole.AppendText(ex.Message + vbCrLf)
        End Try
    End Sub

    'Disconnected from the simulator.
    Private Sub w_Disconnected(ByVal sender As TCP.WorkerClient) Handles w.Disconnected
        Me.Invoke(New DisconnectedFromSimulator(AddressOf Disconnected), New String() {"DISCONNECTED"})
    End Sub

    'Message arrived from the simulator.
    Private Sub w_MessageArrived(ByVal sender As TCP.WorkerClient, ByRef b() As Byte, ByVal len As Integer) Handles w.MessageArrived
        Dim s As String = ""
        For i As Integer = 0 To len - 1
            s = s + Chr(b(i))
        Next
        Me.Invoke(New MessageFromSimulator(AddressOf Message), New String() {s})
    End Sub

    Private Sub Disconnected(ByVal msg As String)
        txtConsole.Enabled = False
        txtIP.Enabled = True
        txtPort.Enabled = True
        cmdConnect.Enabled = True
        txtConsole.AppendText(msg)
    End Sub

    Private Sub Message(ByVal msg As String)
        txtConsole.AppendText(msg)
    End Sub

    Private Sub txtConsole_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtConsole.GotFocus
        txtConsole.ScrollToCaret()
    End Sub

    'Accept user commands and send them to the simulator.
    Private Sub txtConsole_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtConsole.KeyPress
        Static command As String = ""
        If e.KeyChar = vbBack Then
            If command <> "" Then
                command = command.Substring(0, command.Length - 1)
                Dim newText As String = txtConsole.Text.Remove(txtConsole.Text.Length - 1, 1)
                txtConsole.Text = ""
                txtConsole.AppendText(newText)
                txtConsole.ScrollToCaret()
            End If
        ElseIf e.KeyChar = vbCr Then
            w.send(command)
            command = ""
            txtConsole.AppendText(vbCrLf)
        ElseIf Asc(e.KeyChar) = 3 Then 'Ctrl-C
            Clipboard.SetText(txtConsole.SelectedText)
        ElseIf Asc(e.KeyChar) = 22 Then 'Ctrl-V
            command = command + Clipboard.GetText
            txtConsole.AppendText(Clipboard.GetText)
        Else
            command = command + e.KeyChar
            txtConsole.AppendText(e.KeyChar)
        End If
    End Sub

End Class