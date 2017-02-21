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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmbientPlayer));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblInvitation = new System.Windows.Forms.Label();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInvitation
            // 
            this.lblInvitation.BackColor = System.Drawing.Color.Yellow;
            this.lblInvitation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblInvitation.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInvitation.ForeColor = System.Drawing.Color.Black;
            this.lblInvitation.Location = new System.Drawing.Point(0, 0);
            this.lblInvitation.Name = "lblInvitation";
            this.lblInvitation.Size = new System.Drawing.Size(574, 316);
            this.lblInvitation.TabIndex = 1;
            this.lblInvitation.Text = "Invitation Message";
            this.lblInvitation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblInvitation.Click += new System.EventHandler(this.InvitationClick);
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(126, 70);
            this.axWindowsMediaPlayer1.TabIndex = 2;
            // 
            // AmbientPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Controls.Add(this.lblInvitation);
            this.Name = "AmbientPlayer";
            this.Size = new System.Drawing.Size(574, 316);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblInvitation;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
    }
}
