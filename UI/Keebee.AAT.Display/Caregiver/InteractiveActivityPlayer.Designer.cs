namespace Keebee.AAT.Display.Caregiver
{
    partial class InteractiveActivityPlayer
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
            this.btnClose = new MetroFramework.Controls.MetroButton();
            this.paintingActivity1 = new Keebee.AAT.Display.UserControls.PaintingActivity();
            this.matchingGame1 = new Keebee.AAT.Display.UserControls.MatchingGame();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnClose.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnClose.Location = new System.Drawing.Point(438, -2);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(61, 37);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Exit";
            this.btnClose.UseCustomBackColor = true;
            this.btnClose.UseCustomForeColor = true;
            this.btnClose.UseSelectable = true;
            this.btnClose.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // paintingActivity1
            // 
            this.paintingActivity1.BackColor = System.Drawing.Color.Yellow;
            this.paintingActivity1.Location = new System.Drawing.Point(161, 12);
            this.paintingActivity1.Name = "paintingActivity1";
            this.paintingActivity1.Size = new System.Drawing.Size(118, 54);
            this.paintingActivity1.TabIndex = 1;
            // 
            // matchingGame1
            // 
            this.matchingGame1.BackColor = System.Drawing.Color.Yellow;
            this.matchingGame1.Location = new System.Drawing.Point(12, 12);
            this.matchingGame1.Name = "matchingGame1";
            this.matchingGame1.Size = new System.Drawing.Size(118, 54);
            this.matchingGame1.TabIndex = 0;
            // 
            // InteractiveActivityPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.ClientSize = new System.Drawing.Size(498, 288);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.paintingActivity1);
            this.Controls.Add(this.matchingGame1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InteractiveActivityPlayer";
            this.Text = "ActivityPlayer";
            this.Shown += new System.EventHandler(this.InteractiveActivityPlayerShown);
            this.ResumeLayout(false);

        }

        #endregion

        private UserControls.MatchingGame matchingGame1;
        private UserControls.PaintingActivity paintingActivity1;
        private MetroFramework.Controls.MetroButton btnClose;
    }
}