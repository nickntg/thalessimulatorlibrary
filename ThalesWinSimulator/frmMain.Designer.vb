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

    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtParameters As System.Windows.Forms.TextBox
    Friend WithEvents rbDebug As System.Windows.Forms.RadioButton
    Friend WithEvents rbVerbose As System.Windows.Forms.RadioButton
    Friend WithEvents rbInfo As System.Windows.Forms.RadioButton
    Friend WithEvents rbWarning As System.Windows.Forms.RadioButton
    Friend WithEvents rbError As System.Windows.Forms.RadioButton
    Friend WithEvents rbNothing As System.Windows.Forms.RadioButton
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents sb As System.Windows.Forms.StatusBar
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
    Friend WithEvents pbAbout As System.Windows.Forms.PictureBox
    Friend WithEvents txtMajorEvents As System.Windows.Forms.TextBox
    Friend WithEvents txtMinorEvents As System.Windows.Forms.TextBox
    Friend WithEvents txtPrinterOutput As System.Windows.Forms.TextBox
    Friend WithEvents cmdClearMajor As System.Windows.Forms.Button
    Friend WithEvents cmdCopyMajor As System.Windows.Forms.Button
    Friend WithEvents cmdClearMinor As System.Windows.Forms.Button
    Friend WithEvents cmdClearPrinter As System.Windows.Forms.Button
    Friend WithEvents cmdCopyMinor As System.Windows.Forms.Button
    Friend WithEvents cmdCopyPrinter As System.Windows.Forms.Button
    Friend WithEvents cmdStartSim As System.Windows.Forms.Button
    Friend WithEvents cmdStopSim As System.Windows.Forms.Button
    Friend WithEvents cmdChangeAuth As System.Windows.Forms.Button
    Friend WithEvents cmdLMK As System.Windows.Forms.Button
    Friend WithEvents authMode As System.Windows.Forms.StatusBarPanel
    Friend WithEvents status As System.Windows.Forms.StatusBarPanel
    Friend WithEvents various As System.Windows.Forms.StatusBarPanel
    Friend WithEvents gb As System.Windows.Forms.GroupBox
    Friend WithEvents cmdLMKPairs As System.Windows.Forms.Button

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Label1 = New System.Windows.Forms.Label
        Me.txtParameters = New System.Windows.Forms.TextBox
        Me.gb = New System.Windows.Forms.GroupBox
        Me.rbNothing = New System.Windows.Forms.RadioButton
        Me.rbError = New System.Windows.Forms.RadioButton
        Me.rbWarning = New System.Windows.Forms.RadioButton
        Me.rbInfo = New System.Windows.Forms.RadioButton
        Me.rbVerbose = New System.Windows.Forms.RadioButton
        Me.rbDebug = New System.Windows.Forms.RadioButton
        Me.txtMajorEvents = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtMinorEvents = New System.Windows.Forms.TextBox
        Me.Label4 = New System.Windows.Forms.Label
        Me.txtPrinterOutput = New System.Windows.Forms.TextBox
        Me.sb = New System.Windows.Forms.StatusBar
        Me.authMode = New System.Windows.Forms.StatusBarPanel
        Me.status = New System.Windows.Forms.StatusBarPanel
        Me.various = New System.Windows.Forms.StatusBarPanel
        Me.cmdClearMajor = New System.Windows.Forms.Button
        Me.cmdCopyMajor = New System.Windows.Forms.Button
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cmdClearMinor = New System.Windows.Forms.Button
        Me.cmdClearPrinter = New System.Windows.Forms.Button
        Me.cmdCopyMinor = New System.Windows.Forms.Button
        Me.cmdCopyPrinter = New System.Windows.Forms.Button
        Me.cmdStartSim = New System.Windows.Forms.Button
        Me.cmdStopSim = New System.Windows.Forms.Button
        Me.cmdChangeAuth = New System.Windows.Forms.Button
        Me.pbAbout = New System.Windows.Forms.PictureBox
        Me.cmdLMK = New System.Windows.Forms.Button
        Me.cmdLMKPairs = New System.Windows.Forms.Button
        Me.cmdConsole = New System.Windows.Forms.Button
        Me.gb.SuspendLayout()
        CType(Me.authMode, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.status, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.various, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbAbout, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(8, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(88, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Parameters file:"
        '
        'txtParameters
        '
        Me.txtParameters.Location = New System.Drawing.Point(96, 5)
        Me.txtParameters.Name = "txtParameters"
        Me.txtParameters.Size = New System.Drawing.Size(240, 21)
        Me.txtParameters.TabIndex = 1
        Me.txtParameters.Text = "..\..\..\ThalesCore\ThalesParameters.xml"
        '
        'gb
        '
        Me.gb.Controls.Add(Me.rbNothing)
        Me.gb.Controls.Add(Me.rbError)
        Me.gb.Controls.Add(Me.rbWarning)
        Me.gb.Controls.Add(Me.rbInfo)
        Me.gb.Controls.Add(Me.rbVerbose)
        Me.gb.Controls.Add(Me.rbDebug)
        Me.gb.Enabled = False
        Me.gb.Location = New System.Drawing.Point(8, 32)
        Me.gb.Name = "gb"
        Me.gb.Size = New System.Drawing.Size(85, 176)
        Me.gb.TabIndex = 2
        Me.gb.TabStop = False
        Me.gb.Text = "Detail level"
        '
        'rbNothing
        '
        Me.rbNothing.Location = New System.Drawing.Point(8, 144)
        Me.rbNothing.Name = "rbNothing"
        Me.rbNothing.Size = New System.Drawing.Size(64, 24)
        Me.rbNothing.TabIndex = 5
        Me.rbNothing.Tag = "0"
        Me.rbNothing.Text = "Nothing"
        '
        'rbError
        '
        Me.rbError.Location = New System.Drawing.Point(8, 120)
        Me.rbError.Name = "rbError"
        Me.rbError.Size = New System.Drawing.Size(64, 24)
        Me.rbError.TabIndex = 4
        Me.rbError.Tag = "1"
        Me.rbError.Text = "Error"
        '
        'rbWarning
        '
        Me.rbWarning.Location = New System.Drawing.Point(8, 96)
        Me.rbWarning.Name = "rbWarning"
        Me.rbWarning.Size = New System.Drawing.Size(77, 24)
        Me.rbWarning.TabIndex = 3
        Me.rbWarning.Tag = "2"
        Me.rbWarning.Text = "Warning"
        '
        'rbInfo
        '
        Me.rbInfo.Location = New System.Drawing.Point(8, 72)
        Me.rbInfo.Name = "rbInfo"
        Me.rbInfo.Size = New System.Drawing.Size(64, 24)
        Me.rbInfo.TabIndex = 2
        Me.rbInfo.Tag = "3"
        Me.rbInfo.Text = "Info"
        '
        'rbVerbose
        '
        Me.rbVerbose.Location = New System.Drawing.Point(8, 48)
        Me.rbVerbose.Name = "rbVerbose"
        Me.rbVerbose.Size = New System.Drawing.Size(64, 24)
        Me.rbVerbose.TabIndex = 1
        Me.rbVerbose.Tag = "4"
        Me.rbVerbose.Text = "Verbose"
        '
        'rbDebug
        '
        Me.rbDebug.Checked = True
        Me.rbDebug.Location = New System.Drawing.Point(8, 24)
        Me.rbDebug.Name = "rbDebug"
        Me.rbDebug.Size = New System.Drawing.Size(64, 24)
        Me.rbDebug.TabIndex = 0
        Me.rbDebug.TabStop = True
        Me.rbDebug.Tag = "5"
        Me.rbDebug.Text = "Debug"
        '
        'txtMajorEvents
        '
        Me.txtMajorEvents.Location = New System.Drawing.Point(99, 60)
        Me.txtMajorEvents.Multiline = True
        Me.txtMajorEvents.Name = "txtMajorEvents"
        Me.txtMajorEvents.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtMajorEvents.Size = New System.Drawing.Size(304, 320)
        Me.txtMajorEvents.TabIndex = 3
        Me.txtMajorEvents.WordWrap = False
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label2.Location = New System.Drawing.Point(99, 37)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(144, 23)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Application Events"
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label3.Location = New System.Drawing.Point(449, 37)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(144, 23)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Command Events"
        '
        'txtMinorEvents
        '
        Me.txtMinorEvents.Location = New System.Drawing.Point(449, 60)
        Me.txtMinorEvents.Multiline = True
        Me.txtMinorEvents.Name = "txtMinorEvents"
        Me.txtMinorEvents.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtMinorEvents.Size = New System.Drawing.Size(304, 320)
        Me.txtMinorEvents.TabIndex = 5
        Me.txtMinorEvents.WordWrap = False
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.Label4.Location = New System.Drawing.Point(99, 386)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(144, 23)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Printer Output"
        '
        'txtPrinterOutput
        '
        Me.txtPrinterOutput.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtPrinterOutput.Location = New System.Drawing.Point(99, 412)
        Me.txtPrinterOutput.Multiline = True
        Me.txtPrinterOutput.Name = "txtPrinterOutput"
        Me.txtPrinterOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtPrinterOutput.Size = New System.Drawing.Size(653, 114)
        Me.txtPrinterOutput.TabIndex = 7
        Me.txtPrinterOutput.WordWrap = False
        '
        'sb
        '
        Me.sb.Location = New System.Drawing.Point(0, 535)
        Me.sb.Name = "sb"
        Me.sb.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.authMode, Me.status, Me.various})
        Me.sb.ShowPanels = True
        Me.sb.Size = New System.Drawing.Size(792, 22)
        Me.sb.SizingGrip = False
        Me.sb.TabIndex = 9
        '
        'authMode
        '
        Me.authMode.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.authMode.Name = "authMode"
        Me.authMode.Width = 10
        '
        'status
        '
        Me.status.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents
        Me.status.Name = "status"
        Me.status.Text = "Stopped"
        Me.status.Width = 56
        '
        'various
        '
        Me.various.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
        Me.various.Name = "various"
        Me.various.Width = 726
        '
        'cmdClearMajor
        '
        Me.cmdClearMajor.BackgroundImage = CType(resources.GetObject("cmdClearMajor.BackgroundImage"), System.Drawing.Image)
        Me.cmdClearMajor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdClearMajor.Location = New System.Drawing.Point(411, 61)
        Me.cmdClearMajor.Name = "cmdClearMajor"
        Me.cmdClearMajor.Size = New System.Drawing.Size(32, 32)
        Me.cmdClearMajor.TabIndex = 10
        Me.ToolTip1.SetToolTip(Me.cmdClearMajor, "Click to clear application events")
        '
        'cmdCopyMajor
        '
        Me.cmdCopyMajor.BackgroundImage = CType(resources.GetObject("cmdCopyMajor.BackgroundImage"), System.Drawing.Image)
        Me.cmdCopyMajor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdCopyMajor.Location = New System.Drawing.Point(411, 100)
        Me.cmdCopyMajor.Name = "cmdCopyMajor"
        Me.cmdCopyMajor.Size = New System.Drawing.Size(32, 32)
        Me.cmdCopyMajor.TabIndex = 11
        Me.ToolTip1.SetToolTip(Me.cmdCopyMajor, "Click to copy application events to the clipboard")
        '
        'cmdClearMinor
        '
        Me.cmdClearMinor.BackgroundImage = CType(resources.GetObject("cmdClearMinor.BackgroundImage"), System.Drawing.Image)
        Me.cmdClearMinor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdClearMinor.Location = New System.Drawing.Point(760, 61)
        Me.cmdClearMinor.Name = "cmdClearMinor"
        Me.cmdClearMinor.Size = New System.Drawing.Size(32, 32)
        Me.cmdClearMinor.TabIndex = 12
        Me.ToolTip1.SetToolTip(Me.cmdClearMinor, "Click to clear command events")
        '
        'cmdClearPrinter
        '
        Me.cmdClearPrinter.BackgroundImage = CType(resources.GetObject("cmdClearPrinter.BackgroundImage"), System.Drawing.Image)
        Me.cmdClearPrinter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdClearPrinter.Location = New System.Drawing.Point(757, 410)
        Me.cmdClearPrinter.Name = "cmdClearPrinter"
        Me.cmdClearPrinter.Size = New System.Drawing.Size(32, 32)
        Me.cmdClearPrinter.TabIndex = 14
        Me.ToolTip1.SetToolTip(Me.cmdClearPrinter, "Click to clear printer output")
        '
        'cmdCopyMinor
        '
        Me.cmdCopyMinor.BackgroundImage = CType(resources.GetObject("cmdCopyMinor.BackgroundImage"), System.Drawing.Image)
        Me.cmdCopyMinor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdCopyMinor.Location = New System.Drawing.Point(760, 100)
        Me.cmdCopyMinor.Name = "cmdCopyMinor"
        Me.cmdCopyMinor.Size = New System.Drawing.Size(32, 32)
        Me.cmdCopyMinor.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.cmdCopyMinor, "Click to copy command events to the clipboard")
        '
        'cmdCopyPrinter
        '
        Me.cmdCopyPrinter.BackgroundImage = CType(resources.GetObject("cmdCopyPrinter.BackgroundImage"), System.Drawing.Image)
        Me.cmdCopyPrinter.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdCopyPrinter.Location = New System.Drawing.Point(757, 450)
        Me.cmdCopyPrinter.Name = "cmdCopyPrinter"
        Me.cmdCopyPrinter.Size = New System.Drawing.Size(32, 32)
        Me.cmdCopyPrinter.TabIndex = 15
        Me.ToolTip1.SetToolTip(Me.cmdCopyPrinter, "Click to copy printer output to the clipboard")
        '
        'cmdStartSim
        '
        Me.cmdStartSim.Location = New System.Drawing.Point(8, 224)
        Me.cmdStartSim.Name = "cmdStartSim"
        Me.cmdStartSim.Size = New System.Drawing.Size(75, 48)
        Me.cmdStartSim.TabIndex = 16
        Me.cmdStartSim.Text = "Start Simulator"
        '
        'cmdStopSim
        '
        Me.cmdStopSim.Enabled = False
        Me.cmdStopSim.Location = New System.Drawing.Point(8, 280)
        Me.cmdStopSim.Name = "cmdStopSim"
        Me.cmdStopSim.Size = New System.Drawing.Size(75, 48)
        Me.cmdStopSim.TabIndex = 17
        Me.cmdStopSim.Text = "Stop Simulator"
        '
        'cmdChangeAuth
        '
        Me.cmdChangeAuth.Enabled = False
        Me.cmdChangeAuth.Location = New System.Drawing.Point(8, 337)
        Me.cmdChangeAuth.Name = "cmdChangeAuth"
        Me.cmdChangeAuth.Size = New System.Drawing.Size(75, 55)
        Me.cmdChangeAuth.TabIndex = 18
        Me.cmdChangeAuth.Text = "Change Authorized Mode"
        '
        'pbAbout
        '
        Me.pbAbout.Image = CType(resources.GetObject("pbAbout.Image"), System.Drawing.Image)
        Me.pbAbout.Location = New System.Drawing.Point(757, 5)
        Me.pbAbout.Name = "pbAbout"
        Me.pbAbout.Size = New System.Drawing.Size(32, 32)
        Me.pbAbout.TabIndex = 20
        Me.pbAbout.TabStop = False
        '
        'cmdLMK
        '
        Me.cmdLMK.Enabled = False
        Me.cmdLMK.Location = New System.Drawing.Point(8, 400)
        Me.cmdLMK.Name = "cmdLMK"
        Me.cmdLMK.Size = New System.Drawing.Size(75, 32)
        Me.cmdLMK.TabIndex = 21
        Me.cmdLMK.Text = "LMK Store"
        '
        'cmdLMKPairs
        '
        Me.cmdLMKPairs.Enabled = False
        Me.cmdLMKPairs.Location = New System.Drawing.Point(8, 440)
        Me.cmdLMKPairs.Name = "cmdLMKPairs"
        Me.cmdLMKPairs.Size = New System.Drawing.Size(75, 32)
        Me.cmdLMKPairs.TabIndex = 23
        Me.cmdLMKPairs.Text = "LMK Pairs"
        '
        'cmdConsole
        '
        Me.cmdConsole.BackgroundImage = CType(resources.GetObject("cmdConsole.BackgroundImage"), System.Drawing.Image)
        Me.cmdConsole.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdConsole.Location = New System.Drawing.Point(17, 478)
        Me.cmdConsole.Name = "cmdConsole"
        Me.cmdConsole.Size = New System.Drawing.Size(55, 47)
        Me.cmdConsole.TabIndex = 24
        Me.cmdConsole.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdConsole.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 14)
        Me.ClientSize = New System.Drawing.Size(792, 557)
        Me.Controls.Add(Me.cmdConsole)
        Me.Controls.Add(Me.cmdLMKPairs)
        Me.Controls.Add(Me.cmdLMK)
        Me.Controls.Add(Me.pbAbout)
        Me.Controls.Add(Me.cmdChangeAuth)
        Me.Controls.Add(Me.cmdStopSim)
        Me.Controls.Add(Me.cmdStartSim)
        Me.Controls.Add(Me.cmdCopyPrinter)
        Me.Controls.Add(Me.cmdClearPrinter)
        Me.Controls.Add(Me.cmdCopyMinor)
        Me.Controls.Add(Me.cmdClearMinor)
        Me.Controls.Add(Me.cmdCopyMajor)
        Me.Controls.Add(Me.cmdClearMajor)
        Me.Controls.Add(Me.sb)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtPrinterOutput)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtMinorEvents)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtMajorEvents)
        Me.Controls.Add(Me.gb)
        Me.Controls.Add(Me.txtParameters)
        Me.Controls.Add(Me.Label1)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Thales Simulator"
        Me.gb.ResumeLayout(False)
        CType(Me.authMode, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.status, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.various, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbAbout, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cmdConsole As System.Windows.Forms.Button
End Class
