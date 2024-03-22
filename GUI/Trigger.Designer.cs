namespace MoCapSequencer.GUI
{
    partial class Trigger
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
            this.lblPort = new System.Windows.Forms.Label();
            this.cmdSetPort = new System.Windows.Forms.Button();
            this.cmdSendTrigger = new System.Windows.Forms.Button();
            this.nudPulseLength = new System.Windows.Forms.NumericUpDown();
            this.lblPulseLength = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseLength)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.ForeColor = System.Drawing.Color.Red;
            this.lblPort.Location = new System.Drawing.Point(81, 8);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(27, 13);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "N/A";
            // 
            // cmdSetPort
            // 
            this.cmdSetPort.Location = new System.Drawing.Point(0, 3);
            this.cmdSetPort.Name = "cmdSetPort";
            this.cmdSetPort.Size = new System.Drawing.Size(75, 23);
            this.cmdSetPort.TabIndex = 2;
            this.cmdSetPort.Text = "Set Port";
            this.cmdSetPort.UseVisualStyleBackColor = true;
            this.cmdSetPort.Click += new System.EventHandler(this.cmdSetPort_Click);
            // 
            // cmdSendTrigger
            // 
            this.cmdSendTrigger.Enabled = false;
            this.cmdSendTrigger.Location = new System.Drawing.Point(0, 32);
            this.cmdSendTrigger.Name = "cmdSendTrigger";
            this.cmdSendTrigger.Size = new System.Drawing.Size(75, 23);
            this.cmdSendTrigger.TabIndex = 4;
            this.cmdSendTrigger.Text = "Send";
            this.cmdSendTrigger.UseVisualStyleBackColor = true;
            this.cmdSendTrigger.Click += new System.EventHandler(this.cmdSendTrigger_Click);
            // 
            // nudPulseLength
            // 
            this.nudPulseLength.Location = new System.Drawing.Point(97, 61);
            this.nudPulseLength.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.nudPulseLength.Name = "nudPulseLength";
            this.nudPulseLength.Size = new System.Drawing.Size(105, 20);
            this.nudPulseLength.TabIndex = 5;
            this.nudPulseLength.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // lblPulseLength
            // 
            this.lblPulseLength.AutoSize = true;
            this.lblPulseLength.Location = new System.Drawing.Point(3, 63);
            this.lblPulseLength.Name = "lblPulseLength";
            this.lblPulseLength.Size = new System.Drawing.Size(88, 13);
            this.lblPulseLength.TabIndex = 6;
            this.lblPulseLength.Text = "PulseLength (ms)";
            // 
            // Trigger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblPulseLength);
            this.Controls.Add(this.nudPulseLength);
            this.Controls.Add(this.cmdSendTrigger);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.cmdSetPort);
            this.Name = "Trigger";
            this.Size = new System.Drawing.Size(205, 90);
            ((System.ComponentModel.ISupportInitialize)(this.nudPulseLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Button cmdSetPort;
        private System.Windows.Forms.Button cmdSendTrigger;
        private System.Windows.Forms.NumericUpDown nudPulseLength;
        private System.Windows.Forms.Label lblPulseLength;
    }
}
