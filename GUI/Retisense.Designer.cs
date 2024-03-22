namespace MoCapSequencer.GUI
{
    partial class Retisense
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRetisenseR = new System.Windows.Forms.Label();
            this.lblRetisenseL = new System.Windows.Forms.Label();
            this.picPressure = new System.Windows.Forms.PictureBox();
            this.chkAutoConnect = new System.Windows.Forms.CheckBox();
            this.lstDevices = new System.Windows.Forms.ListBox();
            this.cmdSet = new System.Windows.Forms.Button();
            this.cmdScan = new System.Windows.Forms.Button();
            this.cmdConnect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picPressure)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRetisenseR
            // 
            this.lblRetisenseR.AutoSize = true;
            this.lblRetisenseR.ForeColor = System.Drawing.Color.Red;
            this.lblRetisenseR.Location = new System.Drawing.Point(5, 21);
            this.lblRetisenseR.Name = "lblRetisenseR";
            this.lblRetisenseR.Size = new System.Drawing.Size(104, 13);
            this.lblRetisenseR.TabIndex = 8;
            this.lblRetisenseR.Text = "Right: Disconnected";
            // 
            // lblRetisenseL
            // 
            this.lblRetisenseL.AutoSize = true;
            this.lblRetisenseL.ForeColor = System.Drawing.Color.Red;
            this.lblRetisenseL.Location = new System.Drawing.Point(5, 8);
            this.lblRetisenseL.Name = "lblRetisenseL";
            this.lblRetisenseL.Size = new System.Drawing.Size(97, 13);
            this.lblRetisenseL.TabIndex = 7;
            this.lblRetisenseL.Text = "Left: Disconnected";
            // 
            // picPressure
            // 
            this.picPressure.Location = new System.Drawing.Point(259, 8);
            this.picPressure.Name = "picPressure";
            this.picPressure.Size = new System.Drawing.Size(220, 277);
            this.picPressure.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPressure.TabIndex = 11;
            this.picPressure.TabStop = false;
            // 
            // chkAutoConnect
            // 
            this.chkAutoConnect.AutoSize = true;
            this.chkAutoConnect.Location = new System.Drawing.Point(8, 49);
            this.chkAutoConnect.Name = "chkAutoConnect";
            this.chkAutoConnect.Size = new System.Drawing.Size(116, 17);
            this.chkAutoConnect.TabIndex = 12;
            this.chkAutoConnect.Text = "Connect on startup";
            this.chkAutoConnect.UseVisualStyleBackColor = true;
            // 
            // lstDevices
            // 
            this.lstDevices.FormattingEnabled = true;
            this.lstDevices.Location = new System.Drawing.Point(0, 135);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lstDevices.Size = new System.Drawing.Size(253, 147);
            this.lstDevices.TabIndex = 14;
            // 
            // cmdSet
            // 
            this.cmdSet.Location = new System.Drawing.Point(53, 106);
            this.cmdSet.Name = "cmdSet";
            this.cmdSet.Size = new System.Drawing.Size(49, 23);
            this.cmdSet.TabIndex = 15;
            this.cmdSet.Text = "Set";
            this.cmdSet.UseVisualStyleBackColor = true;
            // 
            // cmdScan
            // 
            this.cmdScan.Location = new System.Drawing.Point(0, 106);
            this.cmdScan.Name = "cmdScan";
            this.cmdScan.Size = new System.Drawing.Size(49, 23);
            this.cmdScan.TabIndex = 16;
            this.cmdScan.Text = "Scan";
            this.cmdScan.UseVisualStyleBackColor = true;
            // 
            // cmdConnect
            // 
            this.cmdConnect.Location = new System.Drawing.Point(178, 106);
            this.cmdConnect.Name = "cmdConnect";
            this.cmdConnect.Size = new System.Drawing.Size(75, 23);
            this.cmdConnect.TabIndex = 10;
            this.cmdConnect.Text = "Connect";
            this.cmdConnect.UseVisualStyleBackColor = true;
            // 
            // Retisense
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdScan);
            this.Controls.Add(this.cmdSet);
            this.Controls.Add(this.lstDevices);
            this.Controls.Add(this.chkAutoConnect);
            this.Controls.Add(this.picPressure);
            this.Controls.Add(this.cmdConnect);
            this.Controls.Add(this.lblRetisenseR);
            this.Controls.Add(this.lblRetisenseL);
            this.DoubleBuffered = true;
            this.Name = "Retisense";
            this.Size = new System.Drawing.Size(494, 295);
            ((System.ComponentModel.ISupportInitialize)(this.picPressure)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblRetisenseR;
        private System.Windows.Forms.Label lblRetisenseL;
        private System.Windows.Forms.PictureBox picPressure;
        private System.Windows.Forms.CheckBox chkAutoConnect;
        private System.Windows.Forms.ListBox lstDevices;
        private System.Windows.Forms.Button cmdSet;
        private System.Windows.Forms.Button cmdScan;
        private System.Windows.Forms.Button cmdConnect;
    }
}
