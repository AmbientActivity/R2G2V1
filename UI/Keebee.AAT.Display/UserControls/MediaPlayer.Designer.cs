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
            this.pbDial = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDial)).BeginInit();
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
            this.pbRadioPanel.Location = new System.Drawing.Point(148, 3);
            this.pbRadioPanel.Name = "pbRadioPanel";
            this.pbRadioPanel.Size = new System.Drawing.Size(179, 92);
            this.pbRadioPanel.TabIndex = 5;
            this.pbRadioPanel.TabStop = false;
            // 
            // pbDial
            // 
            this.pbDial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pbDial.Location = new System.Drawing.Point(352, 6);
            this.pbDial.Name = "pbDial";
            this.pbDial.Size = new System.Drawing.Size(19, 181);
            this.pbDial.TabIndex = 6;
            this.pbDial.TabStop = false;
            // 
            // MediaPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.pbDial);
            this.Controls.Add(this.pbRadioPanel);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "MediaPlayer";
            this.Size = new System.Drawing.Size(404, 233);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.PictureBox pbRadioPanel;
        private System.Windows.Forms.PictureBox pbDial;
    }
}
