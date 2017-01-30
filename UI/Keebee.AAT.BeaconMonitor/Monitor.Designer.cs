namespace Keebee.AAT.BeaconMonitor
{
    partial class Monitor
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
            this.lvBeacons = new System.Windows.Forms.ListView();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpMonitor = new System.Windows.Forms.GroupBox();
            this.radOff = new System.Windows.Forms.RadioButton();
            this.radOn = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblActiveResident = new System.Windows.Forms.Label();
            this.lblRssi = new System.Windows.Forms.Label();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnRestartBeaconService = new System.Windows.Forms.Button();
            this.grpMonitor.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvBeacons
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.lvBeacons, 2);
            this.lvBeacons.Location = new System.Drawing.Point(3, 33);
            this.lvBeacons.Name = "lvBeacons";
            this.tableLayoutPanel1.SetRowSpan(this.lvBeacons, 3);
            this.lvBeacons.Size = new System.Drawing.Size(660, 226);
            this.lvBeacons.TabIndex = 4;
            this.lvBeacons.UseCompatibleStateImageBehavior = false;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(669, 102);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(106, 41);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.ClearButtonClick);
            // 
            // grpMonitor
            // 
            this.grpMonitor.Controls.Add(this.radOff);
            this.grpMonitor.Controls.Add(this.radOn);
            this.grpMonitor.Location = new System.Drawing.Point(669, 33);
            this.grpMonitor.Name = "grpMonitor";
            this.grpMonitor.Size = new System.Drawing.Size(106, 63);
            this.grpMonitor.TabIndex = 2;
            this.grpMonitor.TabStop = false;
            this.grpMonitor.Text = "Monitor";
            // 
            // radOff
            // 
            this.radOff.AutoSize = true;
            this.radOff.Location = new System.Drawing.Point(6, 42);
            this.radOff.Name = "radOff";
            this.radOff.Size = new System.Drawing.Size(39, 17);
            this.radOff.TabIndex = 2;
            this.radOff.TabStop = true;
            this.radOff.Text = "Off";
            this.radOff.UseVisualStyleBackColor = true;
            this.radOff.Click += new System.EventHandler(this.MonitorCheckChanged);
            // 
            // radOn
            // 
            this.radOn.AutoSize = true;
            this.radOn.Location = new System.Drawing.Point(6, 19);
            this.radOn.Name = "radOn";
            this.radOn.Size = new System.Drawing.Size(39, 17);
            this.radOn.TabIndex = 1;
            this.radOn.TabStop = true;
            this.radOn.Text = "On";
            this.radOn.UseVisualStyleBackColor = true;
            this.radOn.Click += new System.EventHandler(this.MonitorCheckChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 499F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 167F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 274F));
            this.tableLayoutPanel1.Controls.Add(this.btnRestartBeaconService, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.grpMonitor, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lvBeacons, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblActiveResident, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblRssi, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCaption, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 294F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(778, 263);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // lblActiveResident
            // 
            this.lblActiveResident.AutoSize = true;
            this.lblActiveResident.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblActiveResident.ForeColor = System.Drawing.Color.Red;
            this.lblActiveResident.Location = new System.Drawing.Point(3, 0);
            this.lblActiveResident.Name = "lblActiveResident";
            this.lblActiveResident.Size = new System.Drawing.Size(80, 29);
            this.lblActiveResident.TabIndex = 5;
            this.lblActiveResident.Text = "Public";
            // 
            // lblRssi
            // 
            this.lblRssi.AutoSize = true;
            this.lblRssi.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRssi.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblRssi.Location = new System.Drawing.Point(669, 0);
            this.lblRssi.Name = "lblRssi";
            this.lblRssi.Size = new System.Drawing.Size(26, 29);
            this.lblRssi.TabIndex = 7;
            this.lblRssi.Text = "0";
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCaption.Location = new System.Drawing.Point(502, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(161, 24);
            this.lblCaption.TabIndex = 8;
            this.lblCaption.Text = "Average Strength:";
            // 
            // btnRestartBeaconService
            // 
            this.btnRestartBeaconService.Location = new System.Drawing.Point(669, 149);
            this.btnRestartBeaconService.Name = "btnRestartBeaconService";
            this.btnRestartBeaconService.Size = new System.Drawing.Size(106, 41);
            this.btnRestartBeaconService.TabIndex = 9;
            this.btnRestartBeaconService.Text = "Restart Beacon Service";
            this.btnRestartBeaconService.UseVisualStyleBackColor = true;
            this.btnRestartBeaconService.Click += new System.EventHandler(this.RetsartBeaconServiceClick);
            // 
            // Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 263);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Monitor";
            this.Text = "Beacon Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonitorFormClosing);
            this.grpMonitor.ResumeLayout(false);
            this.grpMonitor.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvBeacons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpMonitor;
        private System.Windows.Forms.RadioButton radOff;
        private System.Windows.Forms.RadioButton radOn;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblActiveResident;
        private System.Windows.Forms.Label lblRssi;
        private System.Windows.Forms.Label lblCaption;
        private System.Windows.Forms.Button btnRestartBeaconService;
    }
}

