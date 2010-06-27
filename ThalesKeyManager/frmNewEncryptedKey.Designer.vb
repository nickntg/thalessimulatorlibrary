<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNewEncryptedKey
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmNewEncryptedKey))
        Me.txtKeyName = New System.Windows.Forms.TextBox
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtKey = New System.Windows.Forms.TextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.cboKeyTypes = New System.Windows.Forms.ComboBox
        Me.cmdOK = New System.Windows.Forms.Button
        Me.lblWarning = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'txtKeyName
        '
        Me.txtKeyName.Location = New System.Drawing.Point(71, 91)
        Me.txtKeyName.Name = "txtKeyName"
        Me.txtKeyName.Size = New System.Drawing.Size(184, 20)
        Me.txtKeyName.TabIndex = 9
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "Key Name:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(37, 15)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(28, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "Key:"
        '
        'txtKey
        '
        Me.txtKey.Location = New System.Drawing.Point(71, 12)
        Me.txtKey.Name = "txtKey"
        Me.txtKey.Size = New System.Drawing.Size(310, 20)
        Me.txtKey.TabIndex = 8
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 55)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(55, 13)
        Me.Label1.TabIndex = 12
        Me.Label1.Text = "Key Type:"
        '
        'cboKeyTypes
        '
        Me.cboKeyTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboKeyTypes.FormattingEnabled = True
        Me.cboKeyTypes.Location = New System.Drawing.Point(71, 52)
        Me.cboKeyTypes.Name = "cboKeyTypes"
        Me.cboKeyTypes.Size = New System.Drawing.Size(184, 21)
        Me.cboKeyTypes.TabIndex = 13
        '
        'cmdOK
        '
        Me.cmdOK.Location = New System.Drawing.Point(141, 117)
        Me.cmdOK.Name = "cmdOK"
        Me.cmdOK.Size = New System.Drawing.Size(111, 23)
        Me.cmdOK.TabIndex = 14
        Me.cmdOK.Text = "Accept Key"
        Me.cmdOK.UseVisualStyleBackColor = True
        '
        'lblWarning
        '
        Me.lblWarning.Location = New System.Drawing.Point(261, 52)
        Me.lblWarning.Name = "lblWarning"
        Me.lblWarning.Size = New System.Drawing.Size(127, 88)
        Me.lblWarning.TabIndex = 15
        '
        'frmNewEncryptedKey
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(393, 148)
        Me.Controls.Add(Me.lblWarning)
        Me.Controls.Add(Me.cmdOK)
        Me.Controls.Add(Me.cboKeyTypes)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.txtKeyName)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtKey)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "frmNewEncryptedKey"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add New Encrypted Key"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtKeyName As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtKey As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents cboKeyTypes As System.Windows.Forms.ComboBox
    Friend WithEvents cmdOK As System.Windows.Forms.Button
    Friend WithEvents lblWarning As System.Windows.Forms.Label
End Class
