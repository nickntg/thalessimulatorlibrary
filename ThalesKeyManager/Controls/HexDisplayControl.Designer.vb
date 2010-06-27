<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class HexDisplayControl
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
        Me.txtFirst = New System.Windows.Forms.TextBox
        Me.txtSecond = New System.Windows.Forms.TextBox
        Me.txtThird = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'txtFirst
        '
        Me.txtFirst.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtFirst.Location = New System.Drawing.Point(0, 0)
        Me.txtFirst.Name = "txtFirst"
        Me.txtFirst.ReadOnly = True
        Me.txtFirst.Size = New System.Drawing.Size(130, 20)
        Me.txtFirst.TabIndex = 0
        Me.txtFirst.Text = "0123456789ABCDEF"
        Me.txtFirst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtSecond
        '
        Me.txtSecond.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSecond.Location = New System.Drawing.Point(138, 0)
        Me.txtSecond.Name = "txtSecond"
        Me.txtSecond.ReadOnly = True
        Me.txtSecond.Size = New System.Drawing.Size(130, 20)
        Me.txtSecond.TabIndex = 1
        Me.txtSecond.Text = "0123456789ABCDEF"
        Me.txtSecond.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'txtThird
        '
        Me.txtThird.Font = New System.Drawing.Font("Courier New", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtThird.Location = New System.Drawing.Point(276, 0)
        Me.txtThird.Name = "txtThird"
        Me.txtThird.ReadOnly = True
        Me.txtThird.Size = New System.Drawing.Size(130, 20)
        Me.txtThird.TabIndex = 2
        Me.txtThird.Text = "0123456789ABCDEF"
        Me.txtThird.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'HexDisplayControl
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.txtThird)
        Me.Controls.Add(Me.txtSecond)
        Me.Controls.Add(Me.txtFirst)
        Me.Name = "HexDisplayControl"
        Me.Size = New System.Drawing.Size(409, 23)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtFirst As System.Windows.Forms.TextBox
    Friend WithEvents txtSecond As System.Windows.Forms.TextBox
    Friend WithEvents txtThird As System.Windows.Forms.TextBox

End Class
