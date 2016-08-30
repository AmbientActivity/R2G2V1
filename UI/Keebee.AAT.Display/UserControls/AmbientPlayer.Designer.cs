namespace Keebee.AAT.Display.UserControls
{
    partial class AmbientPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmbientPlayer));
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.lblAmbientPlayer = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(111, 57);
            this.axWindowsMediaPlayer1.TabIndex = 0;
            // 
            // lblAmbientPlayer
            // 
            this.lblAmbientPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAmbientPlayer.ForeColor = System.Drawing.Color.White;
            this.lblAmbientPlayer.Location = new System.Drawing.Point(0, 0);
            this.lblAmbientPlayer.Name = "lblAmbientPlayer";
            this.lblAmbientPlayer.Size = new System.Drawing.Size(309, 211);
            this.lblAmbientPlayer.TabIndex = 1;
            this.lblAmbientPlayer.Text = "Ambient Player";
            this.lblAmbientPlayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AmbientPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.Controls.Add(this.lblAmbientPlayer);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Name = "AmbientPlayer";
            this.Size = new System.Drawing.Size(309, 211);
            this.VisibleChanged += new System.EventHandler(this.AmbientPlayerVisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.Label lblAmbientPlayer;
    }
}
