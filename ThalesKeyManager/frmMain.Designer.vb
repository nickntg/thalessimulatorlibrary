<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim TreeNode1 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZMK")
        Dim TreeNode2 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZPK")
        Dim TreeNode3 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("PVK")
        Dim TreeNode4 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TMK")
        Dim TreeNode5 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TPK")
        Dim TreeNode6 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("CVK")
        Dim TreeNode7 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TAK")
        Dim TreeNode8 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZAK")
        Dim TreeNode9 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("BDK")
        Dim TreeNode10 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_AC")
        Dim TreeNode11 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_SMC")
        Dim TreeNode12 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_DAC")
        Dim TreeNode13 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_CVC3")
        Dim TreeNode14 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZEK")
        Dim TreeNode15 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Undefined")
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.OFD = New System.Windows.Forms.OpenFileDialog
        Me.cmdLoadKeyFile = New System.Windows.Forms.Button
        Me.gbLoad = New System.Windows.Forms.GroupBox
        Me.cmdStartFresh = New System.Windows.Forms.Button
        Me.gbKeys = New System.Windows.Forms.GroupBox
        Me.cmdClearAll = New System.Windows.Forms.Button
        Me.cmdClearSelected = New System.Windows.Forms.Button
        Me.cmdAddEncryptedKey = New System.Windows.Forms.Button
        Me.cmdAddClearKey = New System.Windows.Forms.Button
        Me.TV = New System.Windows.Forms.TreeView
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.SFD = New System.Windows.Forms.SaveFileDialog
        Me.KC = New ThalesKeyManager.KeyControl
        Me.gbLoad.SuspendLayout()
        Me.gbKeys.SuspendLayout()
        Me.SuspendLayout()
        '
        'OFD
        '
        Me.OFD.DefaultExt = "txt"
        Me.OFD.FileName = "OpenFileDialog1"
        Me.OFD.Filter = "Thales Key Manager files|*.txt"
        Me.OFD.Title = "Select the Thales Key Manager file to open"
        '
        'cmdLoadKeyFile
        '
        Me.cmdLoadKeyFile.Location = New System.Drawing.Point(231, 19)
        Me.cmdLoadKeyFile.Name = "cmdLoadKeyFile"
        Me.cmdLoadKeyFile.Size = New System.Drawing.Size(179, 45)
        Me.cmdLoadKeyFile.TabIndex = 2
        Me.cmdLoadKeyFile.Text = "Click to load a key manager file"
        Me.cmdLoadKeyFile.UseVisualStyleBackColor = True
        '
        'gbLoad
        '
        Me.gbLoad.Controls.Add(Me.cmdStartFresh)
        Me.gbLoad.Controls.Add(Me.cmdLoadKeyFile)
        Me.gbLoad.Location = New System.Drawing.Point(12, 12)
        Me.gbLoad.Name = "gbLoad"
        Me.gbLoad.Size = New System.Drawing.Size(840, 75)
        Me.gbLoad.TabIndex = 3
        Me.gbLoad.TabStop = False
        Me.gbLoad.Text = "Keys File"
        '
        'cmdStartFresh
        '
        Me.cmdStartFresh.Location = New System.Drawing.Point(430, 19)
        Me.cmdStartFresh.Name = "cmdStartFresh"
        Me.cmdStartFresh.Size = New System.Drawing.Size(179, 45)
        Me.cmdStartFresh.TabIndex = 3
        Me.cmdStartFresh.Text = "Click to start without keys"
        Me.cmdStartFresh.UseVisualStyleBackColor = True
        '
        'gbKeys
        '
        Me.gbKeys.Controls.Add(Me.cmdClearAll)
        Me.gbKeys.Controls.Add(Me.cmdClearSelected)
        Me.gbKeys.Controls.Add(Me.cmdAddEncryptedKey)
        Me.gbKeys.Controls.Add(Me.cmdAddClearKey)
        Me.gbKeys.Controls.Add(Me.TV)
        Me.gbKeys.Controls.Add(Me.KC)
        Me.gbKeys.Enabled = False
        Me.gbKeys.Location = New System.Drawing.Point(12, 93)
        Me.gbKeys.Name = "gbKeys"
        Me.gbKeys.Size = New System.Drawing.Size(840, 316)
        Me.gbKeys.TabIndex = 4
        Me.gbKeys.TabStop = False
        Me.gbKeys.Text = "Keys"
        '
        'cmdClearAll
        '
        Me.cmdClearAll.Location = New System.Drawing.Point(608, 273)
        Me.cmdClearAll.Name = "cmdClearAll"
        Me.cmdClearAll.Size = New System.Drawing.Size(179, 30)
        Me.cmdClearAll.TabIndex = 9
        Me.cmdClearAll.Text = "Delete all keys"
        Me.cmdClearAll.UseVisualStyleBackColor = True
        '
        'cmdClearSelected
        '
        Me.cmdClearSelected.Location = New System.Drawing.Point(423, 273)
        Me.cmdClearSelected.Name = "cmdClearSelected"
        Me.cmdClearSelected.Size = New System.Drawing.Size(179, 30)
        Me.cmdClearSelected.TabIndex = 8
        Me.cmdClearSelected.Text = "Delete selected key"
        Me.cmdClearSelected.UseVisualStyleBackColor = True
        '
        'cmdAddEncryptedKey
        '
        Me.cmdAddEncryptedKey.Location = New System.Drawing.Point(238, 273)
        Me.cmdAddEncryptedKey.Name = "cmdAddEncryptedKey"
        Me.cmdAddEncryptedKey.Size = New System.Drawing.Size(179, 30)
        Me.cmdAddEncryptedKey.TabIndex = 7
        Me.cmdAddEncryptedKey.Text = "Add encrypted key"
        Me.cmdAddEncryptedKey.UseVisualStyleBackColor = True
        '
        'cmdAddClearKey
        '
        Me.cmdAddClearKey.Location = New System.Drawing.Point(53, 273)
        Me.cmdAddClearKey.Name = "cmdAddClearKey"
        Me.cmdAddClearKey.Size = New System.Drawing.Size(179, 30)
        Me.cmdAddClearKey.TabIndex = 6
        Me.cmdAddClearKey.Text = "Add clear-text key"
        Me.cmdAddClearKey.UseVisualStyleBackColor = True
        '
        'TV
        '
        Me.TV.ImageIndex = 0
        Me.TV.ImageList = Me.ImageList1
        Me.TV.Location = New System.Drawing.Point(6, 19)
        Me.TV.Name = "TV"
        TreeNode1.Name = "Node0"
        TreeNode1.Text = "ZMK"
        TreeNode1.ToolTipText = "Zone Master Keys"
        TreeNode2.Name = "Node1"
        TreeNode2.Text = "ZPK"
        TreeNode2.ToolTipText = "Zone PIN Keys"
        TreeNode3.Name = "Node2"
        TreeNode3.Text = "PVK"
        TreeNode3.ToolTipText = "PIN Verification Keys"
        TreeNode4.Name = "Node3"
        TreeNode4.Text = "TMK"
        TreeNode4.ToolTipText = "Terminal Master Keys"
        TreeNode5.Name = "Node4"
        TreeNode5.Text = "TPK"
        TreeNode5.ToolTipText = "Terminal PIN Keys"
        TreeNode6.Name = "Node5"
        TreeNode6.Text = "CVK"
        TreeNode6.ToolTipText = "Card Verification Keys"
        TreeNode7.Name = "Node6"
        TreeNode7.Text = "TAK"
        TreeNode8.Name = "Node7"
        TreeNode8.Text = "ZAK"
        TreeNode9.Name = "Node8"
        TreeNode9.Text = "BDK"
        TreeNode9.ToolTipText = "Base Derivation Keys"
        TreeNode10.Name = "Node9"
        TreeNode10.Text = "MK_AC"
        TreeNode11.Name = "Node10"
        TreeNode11.Text = "MK_SMC"
        TreeNode12.Name = "Node11"
        TreeNode12.Text = "MK_DAC"
        TreeNode13.Name = "Node12"
        TreeNode13.Text = "MK_CVC3"
        TreeNode14.Name = "Node13"
        TreeNode14.Text = "ZEK"
        TreeNode15.Name = "Node14"
        TreeNode15.Text = "Undefined"
        TreeNode15.ToolTipText = "Clear Keys"
        Me.TV.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode1, TreeNode2, TreeNode3, TreeNode4, TreeNode5, TreeNode6, TreeNode7, TreeNode8, TreeNode9, TreeNode10, TreeNode11, TreeNode12, TreeNode13, TreeNode14, TreeNode15})
        Me.TV.SelectedImageIndex = 0
        Me.TV.Size = New System.Drawing.Size(203, 248)
        Me.TV.TabIndex = 5
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "clear.ico")
        Me.ImageList1.Images.SetKeyName(1, "keys.ico")
        '
        'SFD
        '
        Me.SFD.Filter = "Thales Key Manager files|*.txt"
        Me.SFD.Title = "Select the Thales Key Manager file to write"
        '
        'KC
        '
        Me.KC.Enabled = False
        Me.KC.Key = Nothing
        Me.KC.Location = New System.Drawing.Point(215, 19)
        Me.KC.Name = "KC"
        Me.KC.Size = New System.Drawing.Size(619, 248)
        Me.KC.TabIndex = 0
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(864, 416)
        Me.Controls.Add(Me.gbKeys)
        Me.Controls.Add(Me.gbLoad)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Thales Key Manager"
        Me.gbLoad.ResumeLayout(False)
        Me.gbKeys.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents OFD As System.Windows.Forms.OpenFileDialog
    Friend WithEvents cmdLoadKeyFile As System.Windows.Forms.Button
    Friend WithEvents gbLoad As System.Windows.Forms.GroupBox
    Friend WithEvents gbKeys As System.Windows.Forms.GroupBox
    Friend WithEvents KC As ThalesKeyManager.KeyControl
    Friend WithEvents TV As System.Windows.Forms.TreeView
    Friend WithEvents cmdStartFresh As System.Windows.Forms.Button
    Friend WithEvents ImageList1 As System.Windows.Forms.ImageList
    Friend WithEvents cmdClearAll As System.Windows.Forms.Button
    Friend WithEvents cmdClearSelected As System.Windows.Forms.Button
    Friend WithEvents cmdAddEncryptedKey As System.Windows.Forms.Button
    Friend WithEvents cmdAddClearKey As System.Windows.Forms.Button
    Friend WithEvents SFD As System.Windows.Forms.SaveFileDialog
End Class
