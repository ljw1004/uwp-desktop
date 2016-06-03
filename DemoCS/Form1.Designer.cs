namespace DemoCS
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnWinrt = new System.Windows.Forms.Button();
            this.btnAppx = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnWinrt
            // 
            this.btnWinrt.Location = new System.Drawing.Point(12, 12);
            this.btnWinrt.Name = "btnWinrt";
            this.btnWinrt.Size = new System.Drawing.Size(144, 23);
            this.btnWinrt.TabIndex = 0;
            this.btnWinrt.Text = "Call some WinRT APIs...";
            this.btnWinrt.UseVisualStyleBackColor = true;
            this.btnWinrt.Click += new System.EventHandler(this.btnWinrt_Click);
            // 
            // btnAppx
            // 
            this.btnAppx.Location = new System.Drawing.Point(162, 12);
            this.btnAppx.Name = "btnAppx";
            this.btnAppx.Size = new System.Drawing.Size(204, 23);
            this.btnAppx.TabIndex = 1;
            this.btnAppx.Text = "Call some Appx-specific WinRT APIs...";
            this.btnAppx.UseVisualStyleBackColor = true;
            this.btnAppx.Click += new System.EventHandler(this.btnAppx_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 41);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(555, 185);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 238);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnAppx);
            this.Controls.Add(this.btnWinrt);
            this.Name = "Form1";
            this.Text = "DemoCS";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnWinrt;
        private System.Windows.Forms.Button btnAppx;
        private System.Windows.Forms.TextBox textBox1;
    }
}

