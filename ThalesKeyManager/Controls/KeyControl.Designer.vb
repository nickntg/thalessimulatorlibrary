<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KeyControl
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.cmdCopyVariant = New System.Windows.Forms.Button
        Me.cmdCopyANSI = New System.Windows.Forms.Button
        Me.cmdCopyPlain = New System.Windows.Forms.Button
        Me.cmdCopyClear = New System.Windows.Forms.Button
        Me.hVariantValue = New ThalesKeyManager.HexDisplayControl
        Me.hANSIValue = New ThalesKeyManager.HexDisplayControl
        Me.hPlainValue = New ThalesKeyManager.HexDisplayControl
        Me.cboKeyType = New System.Windows.Forms.ComboBox
        Me.txtCV = New System.Windows.Forms.TextBox
        Me.txtKeyName = New System.Windows.Forms.TextBox
        Me.hClearValue = New ThalesKeyManager.HexDisplayControl
        Me.Label7 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.Label1 = New System.Windows.Forms.Label
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cmdCopyVariant)
        Me.GroupBox1.Controls.Add(Me.cmdCopyANSI)
        Me.GroupBox1.Controls.Add(Me.cmdCopyPlain)
        Me.GroupBox1.Controls.Add(Me.cmdCopyClear)
        Me.GroupBox1.Controls.Add(Me.hVariantValue)
        Me.GroupBox1.Controls.Add(Me.hANSIValue)
        Me.GroupBox1.Controls.Add(Me.hPlainValue)
        Me.GroupBox1.Controls.Add(Me.cboKeyType)
        Me.GroupBox1.Controls.Add(Me.txtCV)
        Me.GroupBox1.Controls.Add(Me.txtKeyName)
        Me.GroupBox1.Controls.Add(Me.hClearValue)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(619, 248)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        '
        'cmdCopyVariant
        '
        Me.cmdCopyVariant.Location = New System.Drawing.Point(558, 179)
        Me.cmdCopyVariant.Name = "cmdCopyVariant"
        Me.cmdCopyVariant.Size = New System.Drawing.Size(52, 23)
        Me.cmdCopyVariant.TabIndex = 9
        Me.cmdCopyVariant.Text = "COPY"
        Me.cmdCopyVariant.UseVisualStyleBackColor = True
        '
        'cmdCopyANSI
        '
        Me.cmdCopyANSI.Location = New System.Drawing.Point(558, 145)
        Me.cmdCopyANSI.Name = "cmdCopyANSI"
        Me.cmdCopyANSI.Size = New System.Drawing.Size(52, 23)
        Me.cmdCopyANSI.TabIndex = 7
        Me.cmdCopyANSI.Text = "COPY"
        Me.cmdCopyANSI.UseVisualStyleBackColor = True
        '
        'cmdCopyPlain
        '
        Me.cmdCopyPlain.Location = New System.Drawing.Point(558, 111)
        Me.cmdCopyPlain.Name = "cmdCopyPlain"
        Me.cmdCopyPlain.Size = New System.Drawing.Size(52, 23)
        Me.cmdCopyPlain.TabIndex = 5
        Me.cmdCopyPlain.Text = "COPY"
        Me.cmdCopyPlain.UseVisualStyleBackColor = True
        '
        'cmdCopyClear
        '
        Me.cmdCopyClear.Location = New System.Drawing.Point(558, 77)
        Me.cmdCopyClear.Name = "cmdCopyClear"
        Me.cmdCopyClear.Size = New System.Drawing.Size(52, 23)
        Me.cmdCopyClear.TabIndex = 3
        Me.cmdCopyClear.Text = "COPY"
        Me.cmdCopyClear.UseVisualStyleBackColor = True
        '
        'hVariantValue
        '
        Me.hVariantValue.Location = New System.Drawing.Point(143, 179)
        Me.hVariantValue.Name = "hVariantValue"
        Me.hVariantValue.Size = New System.Drawing.Size(409, 27)
        Me.hVariantValue.TabIndex = 8
        '
        'hANSIValue
        '
        Me.hANSIValue.Location = New System.Drawing.Point(143, 145)
        Me.hANSIValue.Name = "hANSIValue"
        Me.hANSIValue.Size = New System.Drawing.Size(409, 27)
        Me.hANSIValue.TabIndex = 6
        '
        'hPlainValue
        '
        Me.hPlainValue.Location = New System.Drawing.Point(143, 111)
        Me.hPlainValue.Name = "hPlainValue"
        Me.hPlainValue.Size = New System.Drawing.Size(409, 27)
        Me.hPlainValue.TabIndex = 4
        '
        'cboKeyType
        '
        Me.cboKeyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboKeyType.FormattingEnabled = True
        Me.cboKeyType.Location = New System.Drawing.Point(143, 46)
        Me.cboKeyType.Name = "cboKeyType"
        Me.cboKeyType.Size = New System.Drawing.Size(272, 21)
        Me.cboKeyType.TabIndex = 1
        '
        'txtCV
        '
        Me.txtCV.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(161, Byte))
        Me.txtCV.Location = New System.Drawing.Point(143, 216)
        Me.txtCV.Name = "txtCV"
        Me.txtCV.ReadOnly = True
        Me.txtCV.Size = New System.Drawing.Size(136, 20)
        Me.txtCV.TabIndex = 10
        Me.txtCV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtKeyName
        '
        Me.txtKeyName.Location = New System.Drawing.Point(143, 12)
        Me.txtKeyName.Name = "txtKeyName"
        Me.txtKeyName.ReadOnly = True
        Me.txtKeyName.Size = New System.Drawing.Size(272, 20)
        Me.txtKeyName.TabIndex = 0
        '
        'hClearValue
        '
        Me.hClearValue.Location = New System.Drawing.Point(143, 77)
        Me.hClearValue.Name = "hClearValue"
        Me.hClearValue.Size = New System.Drawing.Size(409, 27)
        Me.hClearValue.TabIndex = 2
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(66, 220)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(71, 13)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "Check Value:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(7, 186)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(130, 13)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "Encrypted Value (Variant):"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(15, 152)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(122, 13)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Encrypted Value (ANSI):"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(17, 118)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(120, 13)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "Encrypted Value (Plain):"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(73, 84)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(64, 13)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "Clear Value:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(82, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(55, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Key Type:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(78, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(59, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Key Name:"
        '
        'KeyControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "KeyControl"
        Me.Size = New System.Drawing.Size(619, 248)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents hClearValue As ThalesKeyManager.HexDisplayControl
    Friend WithEvents hVariantValue As ThalesKeyManager.HexDisplayControl
    Friend WithEvents hANSIValue As ThalesKeyManager.HexDisplayControl
    Friend WithEvents hPlainValue As ThalesKeyManager.HexDisplayControl
    Friend WithEvents cboKeyType As System.Windows.Forms.ComboBox
    Friend WithEvents txtCV As System.Windows.Forms.TextBox
    Friend WithEvents txtKeyName As System.Windows.Forms.TextBox
    Friend WithEvents cmdCopyClear As System.Windows.Forms.Button
    Friend WithEvents cmdCopyVariant As System.Windows.Forms.Button
    Friend WithEvents cmdCopyANSI As System.Windows.Forms.Button
    Friend WithEvents cmdCopyPlain As System.Windows.Forms.Button

End Class
