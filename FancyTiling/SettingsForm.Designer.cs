namespace FancyTiling
{
    partial class SettingsForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.lblDirectory = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.chkShuffle = new System.Windows.Forms.CheckBox();
            this.chkMirror = new System.Windows.Forms.CheckBox();
            this.chkSlowZoom = new System.Windows.Forms.CheckBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(360, 199);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "Save";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(441, 199);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Use pictures from:";
            // 
            // lblDirectory
            // 
            this.lblDirectory.Location = new System.Drawing.Point(69, 42);
            this.lblDirectory.Name = "lblDirectory";
            this.lblDirectory.Size = new System.Drawing.Size(339, 23);
            this.lblDirectory.TabIndex = 8;
            this.lblDirectory.Text = "label2";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnBrowse.Location = new System.Drawing.Point(441, 37);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label2
            // 
            this.label2.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label2.Location = new System.Drawing.Point(-2, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(531, 23);
            this.label2.TabIndex = 10;
            this.label2.Text = "______________________________________________________________________________";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 100);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Slideshow speed (seconds):";
            // 
            // chkShuffle
            // 
            this.chkShuffle.AutoSize = true;
            this.chkShuffle.Checked = true;
            this.chkShuffle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShuffle.Location = new System.Drawing.Point(167, 122);
            this.chkShuffle.Name = "chkShuffle";
            this.chkShuffle.Size = new System.Drawing.Size(99, 17);
            this.chkShuffle.TabIndex = 13;
            this.chkShuffle.Text = "Shuffle pictures";
            this.chkShuffle.UseVisualStyleBackColor = true;
            this.chkShuffle.CheckedChanged += new System.EventHandler(this.chkShuffle_CheckedChanged);
            // 
            // chkMirror
            // 
            this.chkMirror.AutoSize = true;
            this.chkMirror.Checked = true;
            this.chkMirror.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMirror.Location = new System.Drawing.Point(167, 145);
            this.chkMirror.Name = "chkMirror";
            this.chkMirror.Size = new System.Drawing.Size(183, 17);
            this.chkMirror.TabIndex = 14;
            this.chkMirror.Text = "Mirror images to screen resolution";
            this.chkMirror.UseVisualStyleBackColor = true;
            this.chkMirror.CheckedChanged += new System.EventHandler(this.chkMirror_CheckedChanged);
            // 
            // chkSlowZoom
            // 
            this.chkSlowZoom.AutoSize = true;
            this.chkSlowZoom.Checked = true;
            this.chkSlowZoom.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSlowZoom.Location = new System.Drawing.Point(167, 168);
            this.chkSlowZoom.Name = "chkSlowZoom";
            this.chkSlowZoom.Size = new System.Drawing.Size(77, 17);
            this.chkSlowZoom.TabIndex = 15;
            this.chkSlowZoom.Text = "Slow zoom";
            this.chkSlowZoom.UseVisualStyleBackColor = true;
            this.chkSlowZoom.Visible = false;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(167, 98);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(167, 191);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(70, 17);
            this.checkBox1.TabIndex = 17;
            this.checkBox1.Text = "Use XNA";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 234);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.chkSlowZoom);
            this.Controls.Add(this.chkMirror);
            this.Controls.Add(this.chkShuffle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.lblDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "ScreenSaver Settings";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblDirectory;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkShuffle;
        private System.Windows.Forms.CheckBox chkMirror;
        private System.Windows.Forms.CheckBox chkSlowZoom;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}