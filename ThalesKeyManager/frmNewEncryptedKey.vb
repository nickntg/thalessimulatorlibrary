Public Class frmNewEncryptedKey

    Private Sub frmNewEncryptedKey_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim strs() As String = [Enum].GetNames(GetType(KeyType))
        For Each s As String In strs
            If s.IndexOf(KeyType.Undefined.ToString) = -1 Then
                cboKeyTypes.Items.Add(s)
            End If
        Next
    End Sub

    Private Sub txtKey_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles txtKey.TextChanged
        CheckWarning()
    End Sub

    Private Sub cboKeyTypes_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboKeyTypes.SelectedIndexChanged
        CheckWarning()
    End Sub

    Private Sub CheckWarning()
        lblWarning.Text = ""
        If txtKey.Text <> "" AndAlso cboKeyTypes.SelectedIndex <> -1 Then
            Try
                Dim k As New CryptoKey(txtKey.Text, CType([Enum].Parse(GetType(KeyType), cboKeyTypes.Text), KeyType))
                If k.IsOddParity = False Then
                    lblWarning.Text = "Key does not have odd parity"
                End If
            Catch ex As Exception
                lblWarning.Text = "Key appears to be invalid"
            End Try
        End If
    End Sub

    Private Sub cmdOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOK.Click
        If txtKey.Text <> "" AndAlso cboKeyTypes.SelectedIndex <> -1 Then
            If txtKeyName.Text <> "" Then
                Try
                    Dim k As New CryptoKey(txtKey.Text, CType([Enum].Parse(GetType(KeyType), cboKeyTypes.Text), KeyType))
                    Try
                        Dim KI As KeyInfo = KeyInfos.InfoList(txtKeyName.Text)
                        MessageBox.Show(Me, "Key name already exists", "Duplicate name", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        txtKeyName.Focus()
                        Exit Sub
                    Catch ex As KeyNotFoundException
                        Dim KI As New KeyInfo(txtKeyName.Text, k)
                        KeyInfos.InfoList.Add(KI.KeyName, KI)
                        MessageBox.Show(Me, "Key added", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        DialogResult = Windows.Forms.DialogResult.OK
                    End Try
                Catch ex As Exception
                    MessageBox.Show(Me, "Key appears to be invalid", "Invalid key", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            Else
                MessageBox.Show(Me, "Please enter a key name", "No name", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            MessageBox.Show(Me, "Please enter both key and key type", "Undefined key", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class