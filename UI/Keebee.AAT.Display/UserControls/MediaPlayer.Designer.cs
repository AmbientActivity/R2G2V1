namespace Keebee.AAT.Display.UserControls
{
    partial class MediaPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MediaPlayer));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.pbRadioPanel = new System.Windows.Forms.PictureBox();
            this.lblDial = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(3, 3);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(123, 56);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            // 
            // pbRadioPanel
            // 
            this.pbRadioPanel.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.radio_panel;
            this.pbRadioPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbRadioPanel.Location = new System.Drawing.Point(132, 0);
            this.pbRadioPanel.Name = "pbRadioPanel";
            this.pbRadioPanel.Size = new System.Drawing.Size(115, 59);
            this.pbRadioPanel.TabIndex = 1;
            this.pbRadioPanel.TabStop = false;
            // 
            // lblDial
            // 
            this.lblDial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.lblDial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDial.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDial.Location = new System.Drawing.Point(289, 3);
            this.lblDial.Name = "lblDial";
            this.lblDial.Size = new System.Drawing.Size(13, 89);
            this.lblDial.TabIndex = 2;
            this.lblDial.Text = " ";
            this.lblDial.Visible = false;
            // 
            // MediaPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.lblDial);
            this.Controls.Add(this.pbRadioPanel);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "MediaPlayer";
            this.Size = new System.Drawing.Size(360, 211);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.PictureBox pbRadioPanel;
        private System.Windows.Forms.Label lblDial;
    }
}
