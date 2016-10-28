using Keebee.AAT.Display.UserControls;

namespace Keebee.AAT.Display
{
    partial class Main
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
            this.ambient1 = new Keebee.AAT.Display.UserControls.AmbientPlayer();
            this.matchingGame1 = new Keebee.AAT.Display.UserControls.MatchingGame();
            this.mediaPlayer1 = new Keebee.AAT.Display.UserControls.MediaPlayer();
            this.slideViewerFlash1 = new Keebee.AAT.Display.UserControls.SlideViewerFlash();
            this.offScreen1 = new Keebee.AAT.Display.UserControls.OffScreen();
            this.SuspendLayout();
            // 
            // ambient1
            // 
            this.ambient1.BackColor = System.Drawing.Color.Yellow;
            this.ambient1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ambient1.Location = new System.Drawing.Point(0, 0);
            this.ambient1.Name = "ambient1";
            this.ambient1.Size = new System.Drawing.Size(434, 211);
            this.ambient1.TabIndex = 5;
            // 
            // matchingGame1
            // 
            this.matchingGame1.BackColor = System.Drawing.Color.Yellow;
            this.matchingGame1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.matchingGame1.Location = new System.Drawing.Point(0, 0);
            this.matchingGame1.Name = "matchingGame1";
            this.matchingGame1.Size = new System.Drawing.Size(434, 211);
            this.matchingGame1.TabIndex = 2;
            // 
            // mediaPlayer1
            // 
            this.mediaPlayer1.BackColor = System.Drawing.Color.Yellow;
            this.mediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mediaPlayer1.Location = new System.Drawing.Point(0, 0);
            this.mediaPlayer1.Name = "mediaPlayer1";
            this.mediaPlayer1.Size = new System.Drawing.Size(434, 211);
            this.mediaPlayer1.TabIndex = 7;
            // 
            // slideViewerFlash1
            // 
            this.slideViewerFlash1.BackColor = System.Drawing.Color.Yellow;
            this.slideViewerFlash1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slideViewerFlash1.Location = new System.Drawing.Point(0, 0);
            this.slideViewerFlash1.Name = "slideViewerFlash1";
            this.slideViewerFlash1.Size = new System.Drawing.Size(434, 211);
            this.slideViewerFlash1.TabIndex = 8;
            // 
            // offScreen1
            // 
            this.offScreen1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.offScreen1.Location = new System.Drawing.Point(358, 151);
            this.offScreen1.Name = "offScreen1";
            this.offScreen1.Size = new System.Drawing.Size(64, 48);
            this.offScreen1.TabIndex = 9;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(434, 211);
            this.ControlBox = false;
            this.Controls.Add(this.offScreen1);
            this.Controls.Add(this.slideViewerFlash1);
            this.Controls.Add(this.mediaPlayer1);
            this.Controls.Add(this.ambient1);
            this.Controls.Add(this.matchingGame1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Main";
            this.Shown += new System.EventHandler(this.MainShown);
            this.ResumeLayout(false);

        }

        #endregion

        private AmbientPlayer ambient1;
        private MatchingGame matchingGame1;
        private MediaPlayer mediaPlayer1;
        private SlideViewerFlash slideViewerFlash1;
        private OffScreen offScreen1;
    }
}

