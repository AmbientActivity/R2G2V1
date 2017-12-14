namespace Keebee.AAT.Display
{
    partial class Splash
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
            this.lblLoadingMedia = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblLoadingMedia
            // 
            this.lblLoadingMedia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoadingMedia.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoadingMedia.Location = new System.Drawing.Point(0, 0);
            this.lblLoadingMedia.Name = "lblLoadingMedia";
            this.lblLoadingMedia.Size = new System.Drawing.Size(406, 103);
            this.lblLoadingMedia.TabIndex = 0;
            this.lblLoadingMedia.Text = "Initializing ABBY...";
            this.lblLoadingMedia.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Splash
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 103);
            this.ControlBox = false;
            this.Controls.Add(this.lblLoadingMedia);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Splash";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Splash";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.SplashShown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblLoadingMedia;
    }
}