namespace Keebee.AAT.Display.UserControls
{
    partial class OffScreen
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
            this.lblOff = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblOff
            // 
            this.lblOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 72F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOff.ForeColor = System.Drawing.Color.White;
            this.lblOff.Location = new System.Drawing.Point(0, 0);
            this.lblOff.Name = "lblOff";
            this.lblOff.Size = new System.Drawing.Size(680, 360);
            this.lblOff.TabIndex = 0;
            this.lblOff.Text = "OFF";
            this.lblOff.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OffScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Controls.Add(this.lblOff);
            this.Name = "OffScreen";
            this.Size = new System.Drawing.Size(680, 360);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblOff;
    }
}
