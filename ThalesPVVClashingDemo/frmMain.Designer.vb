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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.txtPort = New System.Windows.Forms.MaskedTextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtIPAddress = New System.Windows.Forms.TextBox
        Me.GroupBox2 = New System.Windows.Forms.GroupBox
        Me.txtCryptPVK = New System.Windows.Forms.TextBox
        Me.txtCryptTPK = New System.Windows.Forms.TextBox
        Me.txtClearTPK = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.GroupBox3 = New System.Windows.Forms.GroupBox
        Me.txtPIN = New System.Windows.Forms.MaskedTextBox
        Me.Label7 = New System.Windows.Forms.Label
        Me.txtPAN = New System.Windows.Forms.TextBox
        Me.Label6 = New System.Windows.Forms.Label
        Me.cmdFindAllPINs = New System.Windows.Forms.Button
        Me.lblStatus = New System.Windows.Forms.Label
        Me.txtLog = New System.Windows.Forms.TextBox
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 17)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(128, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "IP address or host name:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.txtPort)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.txtIPAddress)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(342, 77)
        Me.GroupBox1.TabIndex = 1
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Thales HSM network parameters"
        '
        'txtPort
        '
        Me.txtPort.Location = New System.Drawing.Point(140, 44)
        Me.txtPort.Mask = "00000"
        Me.txtPort.Name = "txtPort"
        Me.txtPort.Size = New System.Drawing.Size(74, 21)
        Me.txtPort.TabIndex = 4
        Me.txtPort.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.thalesPort
        Me.txtPort.ValidatingType = GetType(Integer)
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(64, 47)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(70, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Port number:"
        '
        'txtIPAddress
        '
        Me.txtIPAddress.Location = New System.Drawing.Point(140, 14)
        Me.txtIPAddress.Name = "txtIPAddress"
        Me.txtIPAddress.Size = New System.Drawing.Size(194, 21)
        Me.txtIPAddress.TabIndex = 1
        Me.txtIPAddress.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.thalesIP
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.txtCryptPVK)
        Me.GroupBox2.Controls.Add(Me.txtCryptTPK)
        Me.GroupBox2.Controls.Add(Me.txtClearTPK)
        Me.GroupBox2.Controls.Add(Me.Label5)
        Me.GroupBox2.Controls.Add(Me.Label4)
        Me.GroupBox2.Controls.Add(Me.Label3)
        Me.GroupBox2.Location = New System.Drawing.Point(12, 95)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(342, 109)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Keys"
        '
        'txtCryptPVK
        '
        Me.txtCryptPVK.Location = New System.Drawing.Point(93, 73)
        Me.txtCryptPVK.Name = "txtCryptPVK"
        Me.txtCryptPVK.Size = New System.Drawing.Size(241, 21)
        Me.txtCryptPVK.TabIndex = 8
        Me.txtCryptPVK.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.encryptedPVK
        '
        'txtCryptTPK
        '
        Me.txtCryptTPK.Location = New System.Drawing.Point(93, 42)
        Me.txtCryptTPK.Name = "txtCryptTPK"
        Me.txtCryptTPK.Size = New System.Drawing.Size(241, 21)
        Me.txtCryptTPK.TabIndex = 7
        Me.txtCryptTPK.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.encryptedTPK
        '
        'txtClearTPK
        '
        Me.txtClearTPK.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.ThalesPVVClashingDemo.My.MySettings.Default, "clearTPK", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
        Me.txtClearTPK.Location = New System.Drawing.Point(93, 11)
        Me.txtClearTPK.Name = "txtClearTPK"
        Me.txtClearTPK.Size = New System.Drawing.Size(241, 21)
        Me.txtClearTPK.TabIndex = 6
        Me.txtClearTPK.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.clearTPK
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 76)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(81, 13)
        Me.Label5.TabIndex = 5
        Me.Label5.Text = "Encrypted PVK:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(6, 45)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(81, 13)
        Me.Label4.TabIndex = 4
        Me.Label4.Text = "Encrypted TPK:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(30, 14)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(57, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Clear TPK:"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.txtPIN)
        Me.GroupBox3.Controls.Add(Me.Label7)
        Me.GroupBox3.Controls.Add(Me.txtPAN)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Location = New System.Drawing.Point(12, 210)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(342, 75)
        Me.GroupBox3.TabIndex = 3
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "PAN and PIN"
        '
        'txtPIN
        '
        Me.txtPIN.Location = New System.Drawing.Point(43, 44)
        Me.txtPIN.Mask = "0000"
        Me.txtPIN.Name = "txtPIN"
        Me.txtPIN.Size = New System.Drawing.Size(54, 21)
        Me.txtPIN.TabIndex = 9
        Me.txtPIN.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.PIN
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(6, 47)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(28, 13)
        Me.Label7.TabIndex = 8
        Me.Label7.Text = "PIN:"
        '
        'txtPAN
        '
        Me.txtPAN.Location = New System.Drawing.Point(43, 17)
        Me.txtPAN.MaxLength = 19
        Me.txtPAN.Name = "txtPAN"
        Me.txtPAN.Size = New System.Drawing.Size(291, 21)
        Me.txtPAN.TabIndex = 7
        Me.txtPAN.Text = Global.ThalesPVVClashingDemo.My.MySettings.Default.PAN
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(6, 20)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(31, 13)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "PAN:"
        '
        'cmdFindAllPINs
        '
        Me.cmdFindAllPINs.Location = New System.Drawing.Point(105, 294)
        Me.cmdFindAllPINs.Name = "cmdFindAllPINs"
        Me.cmdFindAllPINs.Size = New System.Drawing.Size(154, 23)
        Me.cmdFindAllPINs.TabIndex = 4
        Me.cmdFindAllPINs.Text = "Find all PINs"
        Me.cmdFindAllPINs.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.Location = New System.Drawing.Point(360, 299)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(322, 14)
        Me.lblStatus.TabIndex = 6
        '
        'txtLog
        '
        Me.txtLog.Location = New System.Drawing.Point(360, 18)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtLog.Size = New System.Drawing.Size(322, 268)
        Me.txtLog.TabIndex = 7
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(686, 322)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.cmdFindAllPINs)
        Me.Controls.Add(Me.GroupBox3)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.Font = New System.Drawing.Font("Tahoma", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PVV Clashing Demo"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtIPAddress As System.Windows.Forms.TextBox
    Friend WithEvents txtPort As System.Windows.Forms.MaskedTextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents txtCryptPVK As System.Windows.Forms.TextBox
    Friend WithEvents txtCryptTPK As System.Windows.Forms.TextBox
    Friend WithEvents txtClearTPK As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents txtPIN As System.Windows.Forms.MaskedTextBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtPAN As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents cmdFindAllPINs As System.Windows.Forms.Button
    Friend WithEvents lblStatus As System.Windows.Forms.Label
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
End Class
