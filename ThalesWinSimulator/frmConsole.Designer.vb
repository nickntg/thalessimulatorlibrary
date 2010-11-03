<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmConsole
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmConsole))
        Me.txtIP = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtPort = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtConsole = New System.Windows.Forms.TextBox
        Me.CMCP = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyCtrlCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.PasteCtrlVToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.cmdConnect = New System.Windows.Forms.Button
        Me.CMCP.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtIP
        '
        Me.txtIP.Location = New System.Drawing.Point(97, 6)
        Me.txtIP.Name = "txtIP"
        Me.txtIP.Size = New System.Drawing.Size(137, 21)
        Me.txtIP.TabIndex = 1
        Me.txtIP.Text = "localhost"
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(3, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 23)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Simulator IP:"
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(97, 33)
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(137, 21)
        Me.txtPort.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(3, 36)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 23)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Console port:"
        '
        'txtConsole
        '
        Me.txtConsole.BackColor = System.Drawing.Color.Black
        Me.txtConsole.ContextMenuStrip = Me.CMCP
        Me.txtConsole.Enabled = False
        Me.txtConsole.Font = New System.Drawing.Font("Courier New", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtConsole.ForeColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(192, Byte), Integer), CType(CType(0, Byte), Integer))
        Me.txtConsole.Location = New System.Drawing.Point(8, 62)
        Me.txtConsole.Multiline = True
        Me.txtConsole.Name = "txtConsole"
        Me.txtConsole.ReadOnly = True
        Me.txtConsole.Size = New System.Drawing.Size(650, 398)
        Me.txtConsole.TabIndex = 3
        '
        'CMCP
        '
        Me.CMCP.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyCtrlCToolStripMenuItem, Me.PasteCtrlVToolStripMenuItem})
        Me.CMCP.Name = "CMCP"
        Me.CMCP.Size = New System.Drawing.Size(153, 70)
        '
        'CopyCtrlCToolStripMenuItem
        '
        Me.CopyCtrlCToolStripMenuItem.Name = "CopyCtrlCToolStripMenuItem"
        Me.CopyCtrlCToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.CopyCtrlCToolStripMenuItem.Text = "&Copy (Ctrl-C)"
        '
        'PasteCtrlVToolStripMenuItem
        '
        Me.PasteCtrlVToolStripMenuItem.Name = "PasteCtrlVToolStripMenuItem"
        Me.PasteCtrlVToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.PasteCtrlVToolStripMenuItem.Text = "&Paste (Ctrl-V)"
        '
        'cmdConnect
        '
        Me.cmdConnect.Location = New System.Drawing.Point(240, 6)
        Me.cmdConnect.Name = "cmdConnect"
        Me.cmdConnect.Size = New System.Drawing.Size(416, 48)
        Me.cmdConnect.TabIndex = 0
        Me.cmdConnect.Text = "&Connect to console"
        '
        'frmConsole
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(668, 472)
        Me.Controls.Add(Me.cmdConnect)
        Me.Controls.Add(Me.txtConsole)
        Me.Controls.Add(Me.txtPort)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtIP)
        Me.Controls.Add(Me.Label1)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmConsole"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "CONSOLE"
        Me.CMCP.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtIP As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtPort As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtConsole As System.Windows.Forms.TextBox
    Friend WithEvents cmdConnect As System.Windows.Forms.Button
    Friend WithEvents CMCP As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents CopyCtrlCToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PasteCtrlVToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
