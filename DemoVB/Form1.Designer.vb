<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.btnAppx = New System.Windows.Forms.Button()
        Me.btnWinrt = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'textBox1
        '
        Me.textBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.textBox1.Location = New System.Drawing.Point(12, 41)
        Me.textBox1.Multiline = True
        Me.textBox1.Name = "textBox1"
        Me.textBox1.ReadOnly = True
        Me.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.textBox1.Size = New System.Drawing.Size(400, 190)
        Me.textBox1.TabIndex = 5
        '
        'btnAppx
        '
        Me.btnAppx.Location = New System.Drawing.Point(162, 12)
        Me.btnAppx.Name = "btnAppx"
        Me.btnAppx.Size = New System.Drawing.Size(204, 23)
        Me.btnAppx.TabIndex = 4
        Me.btnAppx.Text = "Call some Appx-specific WinRT APIs..."
        Me.btnAppx.UseVisualStyleBackColor = True
        '
        'btnWinrt
        '
        Me.btnWinrt.Location = New System.Drawing.Point(12, 12)
        Me.btnWinrt.Name = "btnWinrt"
        Me.btnWinrt.Size = New System.Drawing.Size(144, 23)
        Me.btnWinrt.TabIndex = 3
        Me.btnWinrt.Text = "Call some WinRT APIs..."
        Me.btnWinrt.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(424, 243)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.btnAppx)
        Me.Controls.Add(Me.btnWinrt)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Private WithEvents textBox1 As TextBox
    Private WithEvents btnAppx As Button
    Private WithEvents btnWinrt As Button
End Class
