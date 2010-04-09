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

Public Class frmKeyTypeTable

    Private Sub cmdCalcAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCalcAll.Click
        If txtClearKey.Text.Length <> 16 AndAlso txtClearKey.Text.Length <> 32 AndAlso txtClearKey.Text.Length <> 48 Then
            MessageBox.Show(Me, "Incorrect key size (must be 16, 32 or 48 hex characters)", "INCORRECT KEY LENGTH", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        If Core.Utility.IsHexString(txtClearKey.Text) = False Then
            MessageBox.Show(Me, "Incorrect key (must be 16, 32 or 48 hex characters)", "INCORRECT KEY", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        For Each ctrl As Control In pnl.Controls
            If Not ctrl.Tag Is Nothing Then
                Dim v As String = "", lmk As Core.LMKPairs.LMKPair
                Core.KeyTypeTable.ParseKeyTypeCode(Convert.ToString(ctrl.Tag), lmk, v)
                Dim key As String = ThalesSim.Core.Cryptography.LMK.LMKStorage.LMKVariant(lmk, Convert.ToInt32(v))
                ctrl.Text = ThalesSim.Core.Cryptography.TripleDES.TripleDESEncrypt(New ThalesSim.Core.Cryptography.HexKey(key), txtClearKey.Text)
            End If
        Next
    End Sub
End Class