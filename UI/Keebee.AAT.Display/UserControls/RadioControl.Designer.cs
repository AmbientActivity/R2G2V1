namespace Keebee.AAT.Display.UserControls
{
    partial class RadioControl
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
            this.pbRadioPanel = new System.Windows.Forms.PictureBox();
            this.pbDial = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDial)).BeginInit();
            this.SuspendLayout();
            // 
            // pbRadioPanel
            // 
            this.pbRadioPanel.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.radio_panel;
            this.pbRadioPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbRadioPanel.Location = new System.Drawing.Point(26, 22);
            this.pbRadioPanel.Name = "pbRadioPanel";
            this.pbRadioPanel.Size = new System.Drawing.Size(179, 92);
            this.pbRadioPanel.TabIndex = 6;
            this.pbRadioPanel.TabStop = false;
            // 
            // pbDial
            // 
            this.pbDial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.pbDial.Location = new System.Drawing.Point(233, 34);
            this.pbDial.Name = "pbDial";
            this.pbDial.Size = new System.Drawing.Size(19, 181);
            this.pbDial.TabIndex = 7;
            this.pbDial.TabStop = false;
            // 
            // RadioControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.pbDial);
            this.Controls.Add(this.pbRadioPanel);
            this.Name = "RadioControl";
            this.Size = new System.Drawing.Size(430, 288);
            ((System.ComponentModel.ISupportInitialize)(this.pbRadioPanel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbDial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbRadioPanel;
        private System.Windows.Forms.PictureBox pbDial;
    }
}
