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
            this.lblActiveResident = new System.Windows.Forms.Label();
            this.radioControl1 = new Keebee.AAT.Display.UserControls.RadioControl();
            this.offScreen1 = new Keebee.AAT.Display.UserControls.OffScreen();
            this.slideViewerFlash1 = new Keebee.AAT.Display.UserControls.SlideViewer();
            this.ambient1 = new Keebee.AAT.Display.UserControls.AmbientPlayer();
            this.matchingGame1 = new Keebee.AAT.Display.UserControls.MatchingGame();
            this.mediaPlayer1 = new Keebee.AAT.Display.UserControls.MediaPlayer();
            this.paintingActivity1 = new Keebee.AAT.Display.UserControls.PaintingActivity();
            this.SuspendLayout();
            // 
            // lblActiveResident
            // 
            this.lblActiveResident.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblActiveResident.AutoSize = true;
            this.lblActiveResident.BackColor = System.Drawing.Color.White;
            this.lblActiveResident.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveResident.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblActiveResident.Location = new System.Drawing.Point(321, 191);
            this.lblActiveResident.Name = "lblActiveResident";
            this.lblActiveResident.Size = new System.Drawing.Size(88, 31);
            this.lblActiveResident.TabIndex = 15;
            this.lblActiveResident.Text = "Public";
            // 
            // radioControl1
            // 
            this.radioControl1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.radioControl1.Location = new System.Drawing.Point(264, 2);
            this.radioControl1.Name = "radioControl1";
            this.radioControl1.Size = new System.Drawing.Size(116, 62);
            this.radioControl1.TabIndex = 11;
            // 
            // offScreen1
            // 
            this.offScreen1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.offScreen1.Location = new System.Drawing.Point(265, 77);
            this.offScreen1.Name = "offScreen1";
            this.offScreen1.Size = new System.Drawing.Size(116, 62);
            this.offScreen1.TabIndex = 9;
            // 
            // slideViewerFlash1
            // 
            this.slideViewerFlash1.BackColor = System.Drawing.Color.Yellow;
            this.slideViewerFlash1.Location = new System.Drawing.Point(133, 0);
            this.slideViewerFlash1.Name = "slideViewerFlash1";
            this.slideViewerFlash1.Size = new System.Drawing.Size(116, 62);
            this.slideViewerFlash1.TabIndex = 8;
            // 
            // ambient1
            // 
            this.ambient1.BackColor = System.Drawing.Color.Yellow;
            this.ambient1.Location = new System.Drawing.Point(133, 76);
            this.ambient1.Name = "ambient1";
            this.ambient1.Size = new System.Drawing.Size(116, 62);
            this.ambient1.TabIndex = 5;
            // 
            // matchingGame1
            // 
            this.matchingGame1.BackColor = System.Drawing.Color.Yellow;
            this.matchingGame1.Location = new System.Drawing.Point(0, 0);
            this.matchingGame1.Name = "matchingGame1";
            this.matchingGame1.Size = new System.Drawing.Size(116, 62);
            this.matchingGame1.TabIndex = 2;
            // 
            // mediaPlayer1
            // 
            this.mediaPlayer1.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.mediaPlayer1.Location = new System.Drawing.Point(0, 76);
            this.mediaPlayer1.Name = "mediaPlayer1";
            this.mediaPlayer1.Size = new System.Drawing.Size(127, 63);
            this.mediaPlayer1.TabIndex = 16;
            // 
            // paintingActivity1
            // 
            this.paintingActivity1.BackColor = System.Drawing.Color.Yellow;
            this.paintingActivity1.Location = new System.Drawing.Point(0, 154);
            this.paintingActivity1.Name = "paintingActivity1";
            this.paintingActivity1.Size = new System.Drawing.Size(116, 55);
            this.paintingActivity1.TabIndex = 17;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(408, 221);
            this.ControlBox = false;
            this.Controls.Add(this.paintingActivity1);
            this.Controls.Add(this.mediaPlayer1);
            this.Controls.Add(this.lblActiveResident);
            this.Controls.Add(this.radioControl1);
            this.Controls.Add(this.offScreen1);
            this.Controls.Add(this.slideViewerFlash1);
            this.Controls.Add(this.ambient1);
            this.Controls.Add(this.matchingGame1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainFormClosing);
            this.Shown += new System.EventHandler(this.MainShown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private AmbientPlayer ambient1;
        private MatchingGame matchingGame1;
        private SlideViewer slideViewerFlash1;
        private OffScreen offScreen1;
        private RadioControl radioControl1;
        private System.Windows.Forms.Label lblActiveResident;
        private MediaPlayer mediaPlayer1;
        private PaintingActivity paintingActivity1;
    }
}

