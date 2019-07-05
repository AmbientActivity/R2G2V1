namespace Keebee.AAT.Display.Volume
{
    partial class VolumeControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new MetroFramework.Controls.MetroButton();
            this.lblAdjustVolume = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDown = new MetroFramework.Controls.MetroButton();
            this.btnUp = new MetroFramework.Controls.MetroButton();
            this.pbCurrentVolume = new MetroFramework.Controls.MetroProgressBar();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.SteelBlue;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.lblAdjustVolume);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(410, 71);
            this.panel1.TabIndex = 11;
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(345, 2);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(62, 65);
            this.btnClose.TabIndex = 22;
            this.btnClose.UseSelectable = true;
            this.btnClose.Click += new System.EventHandler(this.ButtonCloseClick);
            // 
            // lblAdjustVolume
            // 
            this.lblAdjustVolume.AutoSize = true;
            this.lblAdjustVolume.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAdjustVolume.ForeColor = System.Drawing.Color.White;
            this.lblAdjustVolume.Location = new System.Drawing.Point(99, 19);
            this.lblAdjustVolume.Name = "lblAdjustVolume";
            this.lblAdjustVolume.Size = new System.Drawing.Size(198, 33);
            this.lblAdjustVolume.TabIndex = 11;
            this.lblAdjustVolume.Text = "Volume Control";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnDown, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUp, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.pbCurrentVolume, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 22F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(410, 195);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // btnDown
            // 
            this.btnDown.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.arrow_left;
            this.btnDown.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDown.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnDown.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDown.Location = new System.Drawing.Point(3, 74);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(199, 63);
            this.btnDown.TabIndex = 13;
            this.btnDown.UseSelectable = true;
            this.btnDown.Click += new System.EventHandler(this.ButtonDownClick);
            // 
            // btnUp
            // 
            this.btnUp.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.arrow_right;
            this.btnUp.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnUp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUp.FontSize = MetroFramework.MetroButtonSize.Tall;
            this.btnUp.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnUp.Location = new System.Drawing.Point(208, 74);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(199, 63);
            this.btnUp.TabIndex = 12;
            this.btnUp.UseSelectable = true;
            this.btnUp.Click += new System.EventHandler(this.ButtonUpClick);
            // 
            // pbCurrentVolume
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.pbCurrentVolume, 2);
            this.pbCurrentVolume.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbCurrentVolume.FontSize = MetroFramework.MetroProgressBarSize.Tall;
            this.pbCurrentVolume.FontWeight = MetroFramework.MetroProgressBarWeight.Bold;
            this.pbCurrentVolume.HideProgressText = false;
            this.pbCurrentVolume.Location = new System.Drawing.Point(3, 143);
            this.pbCurrentVolume.Name = "pbCurrentVolume";
            this.pbCurrentVolume.Size = new System.Drawing.Size(404, 49);
            this.pbCurrentVolume.Style = MetroFramework.MetroColorStyle.Green;
            this.pbCurrentVolume.TabIndex = 14;
            this.pbCurrentVolume.UseCustomBackColor = true;
            // 
            // VolumeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 194);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VolumeControl";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblAdjustVolume;
        private MetroFramework.Controls.MetroButton btnClose;
        private MetroFramework.Controls.MetroButton btnDown;
        private MetroFramework.Controls.MetroButton btnUp;
        private MetroFramework.Controls.MetroProgressBar pbCurrentVolume;
    }
}