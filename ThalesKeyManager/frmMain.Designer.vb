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
        Dim TreeNode16 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZMK")
        Dim TreeNode17 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZPK")
        Dim TreeNode18 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("PVK")
        Dim TreeNode19 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TMK")
        Dim TreeNode20 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TPK")
        Dim TreeNode21 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("CVK")
        Dim TreeNode22 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("TAK")
        Dim TreeNode23 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZAK")
        Dim TreeNode24 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("BDK")
        Dim TreeNode25 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_AC")
        Dim TreeNode26 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_SMC")
        Dim TreeNode27 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_DAC")
        Dim TreeNode28 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("MK_CVC3")
        Dim TreeNode29 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("ZEK")
        Dim TreeNode30 As System.Windows.Forms.TreeNode = New System.Windows.Forms.TreeNode("Undefined")
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
        Me.KC = New ThalesKeyManager.KeyControl
        Me.SFD = New System.Windows.Forms.SaveFileDialog
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.UseNormalStorageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.UseOLDStorageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.gbLoad.SuspendLayout()
        Me.gbKeys.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
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
        Me.gbLoad.Location = New System.Drawing.Point(12, 27)
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
        Me.gbKeys.Location = New System.Drawing.Point(12, 109)
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
        TreeNode16.Name = "Node0"
        TreeNode16.Text = "ZMK"
        TreeNode16.ToolTipText = "Zone Master Keys"
        TreeNode17.Name = "Node1"
        TreeNode17.Text = "ZPK"
        TreeNode17.ToolTipText = "Zone PIN Keys"
        TreeNode18.Name = "Node2"
        TreeNode18.Text = "PVK"
        TreeNode18.ToolTipText = "PIN Verification Keys"
        TreeNode19.Name = "Node3"
        TreeNode19.Text = "TMK"
        TreeNode19.ToolTipText = "Terminal Master Keys"
        TreeNode20.Name = "Node4"
        TreeNode20.Text = "TPK"
        TreeNode20.ToolTipText = "Terminal PIN Keys"
        TreeNode21.Name = "Node5"
        TreeNode21.Text = "CVK"
        TreeNode21.ToolTipText = "Card Verification Keys"
        TreeNode22.Name = "Node6"
        TreeNode22.Text = "TAK"
        TreeNode23.Name = "Node7"
        TreeNode23.Text = "ZAK"
        TreeNode24.Name = "Node8"
        TreeNode24.Text = "BDK"
        TreeNode24.ToolTipText = "Base Derivation Keys"
        TreeNode25.Name = "Node9"
        TreeNode25.Text = "MK_AC"
        TreeNode26.Name = "Node10"
        TreeNode26.Text = "MK_SMC"
        TreeNode27.Name = "Node11"
        TreeNode27.Text = "MK_DAC"
        TreeNode28.Name = "Node12"
        TreeNode28.Text = "MK_CVC3"
        TreeNode29.Name = "Node13"
        TreeNode29.Text = "ZEK"
        TreeNode30.Name = "Node14"
        TreeNode30.Text = "Undefined"
        TreeNode30.ToolTipText = "Clear Keys"
        Me.TV.Nodes.AddRange(New System.Windows.Forms.TreeNode() {TreeNode16, TreeNode17, TreeNode18, TreeNode19, TreeNode20, TreeNode21, TreeNode22, TreeNode23, TreeNode24, TreeNode25, TreeNode26, TreeNode27, TreeNode28, TreeNode29, TreeNode30})
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
        'KC
        '
        Me.KC.Enabled = False
        Me.KC.Key = Nothing
        Me.KC.Location = New System.Drawing.Point(215, 19)
        Me.KC.Name = "KC"
        Me.KC.Size = New System.Drawing.Size(619, 248)
        Me.KC.TabIndex = 0
        '
        'SFD
        '
        Me.SFD.Filter = "Thales Key Manager files|*.txt"
        Me.SFD.Title = "Select the Thales Key Manager file to write"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(864, 24)
        Me.MenuStrip1.TabIndex = 5
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UseNormalStorageToolStripMenuItem, Me.UseOLDStorageToolStripMenuItem})
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(77, 20)
        Me.ToolStripMenuItem1.Text = "Key storage"
        '
        'UseNormalStorageToolStripMenuItem
        '
        Me.UseNormalStorageToolStripMenuItem.Name = "UseNormalStorageToolStripMenuItem"
        Me.UseNormalStorageToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.UseNormalStorageToolStripMenuItem.Text = "Use normal storage"
        '
        'UseOLDStorageToolStripMenuItem
        '
        Me.UseOLDStorageToolStripMenuItem.Name = "UseOLDStorageToolStripMenuItem"
        Me.UseOLDStorageToolStripMenuItem.Size = New System.Drawing.Size(167, 22)
        Me.UseOLDStorageToolStripMenuItem.Text = "Use OLD storage"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(864, 437)
        Me.Controls.Add(Me.gbKeys)
        Me.Controls.Add(Me.gbLoad)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Thales Key Manager"
        Me.gbLoad.ResumeLayout(False)
        Me.gbKeys.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

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
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UseNormalStorageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UseOLDStorageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
