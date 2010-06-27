<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewClearKey
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNewClearKey))
        Me.Label1 = New System.Windows.Forms.Label
        Me.cboKeyLen = New System.Windows.Forms.ComboBox
        Me.cmdGenerateRandom = New System.Windows.Forms.Button
        Me.txtKey = New System.Windows.Forms.TextBox
        Me.Label2 = New System.Windows.Forms.Label
        Me.cmdOK = New System.Windows.Forms.Button
        Me.cmdEnforceParity = New System.Windows.Forms.Button
        Me.Label3 = New System.Windows.Forms.Label
        Me.txtKeyName = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(64, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Key Length:"
        '
        'cboKeyLen
        '
        Me.cboKeyLen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboKeyLen.FormattingEnabled = True
        Me.cboKeyLen.Items.AddRange(New Object() {"Single-length", "Double-length", "Triple-length"})
        Me.cboKeyLen.Location = New System.Drawing.Point(82, 6)
        Me.cboKeyLen.Name = "cboKeyLen"
        Me.cboKeyLen.Size = New System.Drawing.Size(184, 21)
        Me.cboKeyLen.TabIndex = 0
        '
        'cmdGenerateRandom
        '
        Me.cmdGenerateRandom.Location = New System.Drawing.Point(281, 4)
        Me.cmdGenerateRandom.Name = "cmdGenerateRandom"
        Me.cmdGenerateRandom.Size = New System.Drawing.Size(111, 23)
        Me.cmdGenerateRandom.TabIndex = 1
        Me.cmdGenerateRandom.Text = "Generate Random"
        Me.cmdGenerateRandom.UseVisualStyleBackColor = True
        '
        'txtKey
        '
        Me.txtKey.Location = New System.Drawing.Point(82, 48)
        Me.txtKey.Name = "txtKey"
        Me.txtKey.Size = New System.Drawing.Size(310, 20)
        Me.txtKey.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(48, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(28, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Key:"
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(146, 128)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(111, 23)
        Me.cmdOK.TabIndex = 5
        Me.cmdOK.Text = "Accept Key"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'cmdEnforceParity
        '
        Me.cmdEnforceParity.Location = New System.Drawing.Point(281, 89)
        Me.cmdEnforceParity.Name = "cmdEnforceParity"
        Me.cmdEnforceParity.Size = New System.Drawing.Size(111, 23)
        Me.cmdEnforceParity.TabIndex = 4
        Me.cmdEnforceParity.Text = "Enforce Odd Parity"
        Me.cmdEnforceParity.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(17, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Key Name:"
        '
        'txtKeyName
        '
        Me.txtKeyName.Location = New System.Drawing.Point(82, 91)
        Me.txtKeyName.Name = "txtKeyName"
        Me.txtKeyName.Size = New System.Drawing.Size(184, 20)
        Me.txtKeyName.TabIndex = 3
        '
        'frmNewClearKey
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(403, 161)
        Me.Controls.Add(Me.txtKeyName)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.cmdEnforceParity)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtKey)
        Me.Controls.Add(Me.cmdGenerateRandom)
        Me.Controls.Add(Me.cboKeyLen)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmNewClearKey"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add new clear key"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboKeyLen As System.Windows.Forms.ComboBox
    Friend WithEvents cmdGenerateRandom As System.Windows.Forms.Button
    Friend WithEvents txtKey As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents cmdEnforceParity As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtKeyName As System.Windows.Forms.TextBox
End Class
