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

Imports ThalesSim.Core

Public Class frmNewClearKey

    Private Sub cmdGenerateRandom_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdGenerateRandom.Click
        Dim kl As Integer = cboKeyLen.SelectedIndex + 1
        If kl > 0 Then
            txtKey.Text = ""
            For i As Integer = 1 To kl
                txtKey.Text = txtKey.Text + Utility.RandomKey(False, Utility.ParityCheck.NoParity)
            Next
        End If
    End Sub

    Private Sub cmdEnforceParity_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdEnforceParity.Click
        If CheckKey() = False Then Return

        txtKey.Text = Utility.MakeParity(txtKey.Text, Utility.ParityCheck.OddParity)
    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        If CheckKey() = False Then Return

        If txtKeyName.Text = "" Then
            MessageBox.Show(Me, "Please provide a key name", "No name", MessageBoxButtons.OK, MessageBoxIcon.Information)
            txtKeyName.Focus()
            Exit Sub
        Else
            Try
                Dim KI As KeyInfo = KeyInfos.InfoList(txtKeyName.Text)
                MessageBox.Show(Me, "Key name already exists", "Duplicate name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                txtKeyName.Focus()
                Exit Sub
            Catch ex As KeyNotFoundException
                Dim KI As New KeyInfo(txtKeyName.Text, txtKey.Text, KeyType.Undefined)
                KeyInfos.InfoList.Add(KI.KeyName, KI)
                MessageBox.Show(Me, "Key added", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
                DialogResult = Windows.Forms.DialogResult.OK
            End Try
        End If

    End Sub

    Private Function CheckKey() As Boolean
        If Utility.IsHexString(txtKey.Text, False) = False Then
            MessageBox.Show(Me, "Text contains invalid characters.", "Invalid characters", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        If txtKey.Text.Length <> 16 AndAlso txtKey.Text.Length <> 32 AndAlso txtKey.Text.Length <> 48 Then
            MessageBox.Show(Me, "Key has an invalid length - must be 16, 32 or 48 hex characters.", "Invalid length", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End If

        Return True
    End Function
End Class