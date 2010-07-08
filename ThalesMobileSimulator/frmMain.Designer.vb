<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Public Class frmMain
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    Private mainMenu1 As System.Windows.Forms.MainMenu

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.mainMenu1 = New System.Windows.Forms.MainMenu
        Me.lnkStartStopSim = New System.Windows.Forms.LinkLabel
        Me.txtLog = New System.Windows.Forms.TextBox
        Me.lnkChangeAuth = New System.Windows.Forms.LinkLabel
        Me.SuspendLayout()
        '
        'lnkStartStopSim
        '
        Me.lnkStartStopSim.Location = New System.Drawing.Point(27, 12)
        Me.lnkStartStopSim.Name = "lnkStartStopSim"
        Me.lnkStartStopSim.Size = New System.Drawing.Size(123, 17)
        Me.lnkStartStopSim.TabIndex = 0
        Me.lnkStartStopSim.Text = " Start Simulator "
        Me.lnkStartStopSim.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(3, 67)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.Size = New System.Drawing.Size(170, 110)
        Me.txtLog.TabIndex = 1
        '
        'lnkChangeAuth
        '
        Me.lnkChangeAuth.Enabled = False
        Me.lnkChangeAuth.Location = New System.Drawing.Point(15, 38)
        Me.lnkChangeAuth.Name = "lnkChangeAuth"
        Me.lnkChangeAuth.Size = New System.Drawing.Size(147, 26)
        Me.lnkChangeAuth.TabIndex = 2
        Me.lnkChangeAuth.Text = " Change Authorized State "
        Me.lnkChangeAuth.TextAlign = System.Drawing.ContentAlignment.TopCenter
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(176, 180)
        Me.Controls.Add(Me.lnkChangeAuth)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.lnkStartStopSim)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Menu = Me.mainMenu1
        Me.Name = "frmMain"
        Me.Text = "Thales Simulator"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lnkStartStopSim As System.Windows.Forms.LinkLabel
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents lnkChangeAuth As System.Windows.Forms.LinkLabel
End Class
