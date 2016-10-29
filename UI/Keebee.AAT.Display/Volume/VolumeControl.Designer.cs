namespace Keebee.AAT.Display.Volume
{
    partial class VolumeControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolumeControl));
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblAdjustVolume = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.rbVolumeUp = new Keebee.AAT.Display.Caregiver.CustomControls.RepeatButton();
            this.rbVolumeDown = new Keebee.AAT.Display.Caregiver.CustomControls.RepeatButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pbCurrentVolume = new System.Windows.Forms.ProgressBar();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Highlight;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.lblAdjustVolume);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 71);
            this.panel1.TabIndex = 11;
            // 
            // lblAdjustVolume
            // 
            this.lblAdjustVolume.AutoSize = true;
            this.lblAdjustVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdjustVolume.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.lblAdjustVolume.Location = new System.Drawing.Point(96, 19);
            this.lblAdjustVolume.Name = "lblAdjustVolume";
            this.lblAdjustVolume.Size = new System.Drawing.Size(200, 31);
            this.lblAdjustVolume.TabIndex = 11;
            this.lblAdjustVolume.Text = "Volume Control";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(343, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 64);
            this.btnClose.TabIndex = 10;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.ButtonCloseClick);
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(3, 183);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(111, 2);
            this.axWindowsMediaPlayer1.TabIndex = 8;
            this.axWindowsMediaPlayer1.Visible = false;
            // 
            // rbVolumeUp
            // 
            this.rbVolumeUp.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbVolumeUp.Location = new System.Drawing.Point(208, 74);
            this.rbVolumeUp.Name = "rbVolumeUp";
            this.rbVolumeUp.Size = new System.Drawing.Size(196, 56);
            this.rbVolumeUp.TabIndex = 6;
            this.rbVolumeUp.Text = ">";
            this.rbVolumeUp.UseVisualStyleBackColor = true;
            this.rbVolumeUp.Click += new System.EventHandler(this.ButtonUpClick);
            // 
            // rbVolumeDown
            // 
            this.rbVolumeDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rbVolumeDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbVolumeDown.Location = new System.Drawing.Point(6, 74);
            this.rbVolumeDown.Name = "rbVolumeDown";
            this.rbVolumeDown.Size = new System.Drawing.Size(196, 56);
            this.rbVolumeDown.TabIndex = 7;
            this.rbVolumeDown.Text = "<";
            this.rbVolumeDown.UseVisualStyleBackColor = true;
            this.rbVolumeDown.Click += new System.EventHandler(this.ButtonDownClick);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pbCurrentVolume, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.axWindowsMediaPlayer1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.rbVolumeUp, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.rbVolumeDown, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(410, 189);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // pbCurrentVolume
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pbCurrentVolume, 2);
            this.pbCurrentVolume.Location = new System.Drawing.Point(3, 136);
            this.pbCurrentVolume.MarqueeAnimationSpeed = 0;
            this.pbCurrentVolume.Name = "pbCurrentVolume";
            this.pbCurrentVolume.Size = new System.Drawing.Size(398, 40);
            this.pbCurrentVolume.Step = 1;
            this.pbCurrentVolume.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbCurrentVolume.TabIndex = 9;
            // 
            // VolumeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 189);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolumeControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ProgressBar pbCurrentVolume;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private Caregiver.CustomControls.RepeatButton rbVolumeUp;
        private Caregiver.CustomControls.RepeatButton rbVolumeDown;
        private System.Windows.Forms.Label lblAdjustVolume;
        private System.Windows.Forms.Button btnClose;
    }
}