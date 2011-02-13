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

    Dim o As ThalesSim.Core.ThalesMain

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If gbKeys.Enabled = False Then Exit Sub
        SFD.FileName = ""
        SFD.ShowDialog(Me)
        If SFD.FileName <> "" Then
            KeyInfos.WriteToFile(SFD.FileName)
        End If
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        o = New ThalesSim.Core.ThalesMain
        o.StartUpWithoutTCP("ThalesParameters.xml")
    End Sub

    Private Sub cmdStartFresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdStartFresh.Click
        gbLoad.Enabled = False
        gbKeys.Enabled = True
    End Sub

    Private Sub cmdLoadKeyFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLoadKeyFile.Click
        OFD.FileName = ""
        OFD.ShowDialog(Me)
        If OFD.FileName <> "" Then
            KeyInfos.ReadFromFile(OFD.FileName)

            AddAllKeys()

            gbLoad.Enabled = False
            gbKeys.Enabled = True
        End If
    End Sub

    Private Sub cmdAddClearKey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddClearKey.Click
        Dim kcount As Integer = KeyInfos.InfoList.Count
        Using frm As New frmNewClearKey
            frm.ShowDialog(Me)
        End Using
        If KeyInfos.InfoList.Count <> kcount Then
            Dim node As TreeNode = GetNodeByType(KeyType.Undefined)
            node.Nodes.Clear()
            For Each keyName As String In KeyInfos.InfoList.Keys
                If KeyInfos.InfoList(keyName).Key.KeyType = KeyType.Undefined Then
                    AddKey(node, keyName)
                End If
            Next
            KC.Key = Nothing
            KC.Enabled = False
        End If
    End Sub

    Private Sub cmdAddEncryptedKey_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdAddEncryptedKey.Click
        Dim kcount As Integer = KeyInfos.InfoList.Count
        Using frm As New frmNewEncryptedKey
            frm.ShowDialog(Me)
        End Using

        If KeyInfos.InfoList.Count <> kcount Then
            For Each node As TreeNode In TV.Nodes
                node.Nodes.Clear()
            Next

            AddAllKeys()

            KC.Key = Nothing
            KC.Enabled = False
        End If
    End Sub

    Private Function GetNodeByType(ByVal kt As KeyType) As TreeNode
        For Each node As TreeNode In TV.Nodes
            If node.Text = kt.ToString Then
                Return node
            End If
        Next
        Return Nothing
    End Function

    Private Function GetNodeByKeyName(ByVal keyName As String) As TreeNode
        For Each node As TreeNode In TV.Nodes
            For Each subnode As TreeNode In node.Nodes
                If subnode.Text = keyName Then
                    Return subnode
                End If
            Next
        Next
        Return Nothing
    End Function

    Private Sub AddKey(ByVal parentNode As TreeNode, ByVal KI As KeyInfo)
        Dim newNode As New TreeNode(KI.KeyName)
        newNode.ImageIndex = 1
        newNode.SelectedImageIndex = 1
        newNode.Tag = KI
        parentNode.Nodes.Add(newNode)
    End Sub

    Private Sub AddKey(ByVal parentNode As TreeNode, ByVal keyName As String)
        Dim newNode As New TreeNode(keyName)
        newNode.ImageIndex = 1
        newNode.SelectedImageIndex = 1
        newNode.Tag = KeyInfos.InfoList(keyName)
        parentNode.Nodes.Add(newNode)
    End Sub

    Private Sub AddAllKeys()
        For Each keyName As String In KeyInfos.InfoList.Keys
            Dim KI As KeyInfo = KeyInfos.InfoList(keyName)
            AddKey(GetNodeByType(KI.Key.KeyType), KI)
        Next
    End Sub

    Private Sub TV_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles TV.AfterSelect
        If (TV.SelectedNode IsNot Nothing) AndAlso (TypeOf (TV.SelectedNode.Tag) Is KeyInfo) Then
            KC.Key = CType(TV.SelectedNode.Tag, KeyInfo)
            KC.Enabled = True
        Else
            KC.Key = Nothing
            KC.Enabled = False
        End If
    End Sub

    Private Sub KC_KeyTypeChanged(ByVal sender As Object, ByVal keyName As String, ByVal keyType As KeyType) Handles KC.KeyTypeChanged
        Dim keyNode As TreeNode = GetNodeByKeyName(keyName)
        Dim newNode As TreeNode = GetNodeByType(keyType)
        Dim oldNode As TreeNode = keyNode.Parent
        oldNode.Nodes.Remove(keyNode)
        newNode.Nodes.Add(keyNode)
        TV.SelectedNode = keyNode
    End Sub

    Private Sub UseNormalStorageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UseNormalStorageToolStripMenuItem.Click
        ThalesSim.Core.Cryptography.LMK.LMKStorage.UseOldLMKStorage = False
    End Sub

    Private Sub UseOLDStorageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UseOLDStorageToolStripMenuItem.Click
        ThalesSim.Core.Cryptography.LMK.LMKStorage.UseOldLMKStorage = True
    End Sub
End Class