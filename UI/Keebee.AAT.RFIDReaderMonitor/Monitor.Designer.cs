namespace Keebee.AAT.RFIDReaderMonitor
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
            this.lvReads = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grpMonitor = new System.Windows.Forms.GroupBox();
            this.radOff = new System.Windows.Forms.RadioButton();
            this.radOn = new System.Windows.Forms.RadioButton();
            this.btnClear = new System.Windows.Forms.Button();
            this.lvResidents = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1.SuspendLayout();
            this.grpMonitor.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvReads
            // 
            this.lvReads.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvReads.Location = new System.Drawing.Point(3, 3);
            this.lvReads.Name = "lvReads";
            this.tableLayoutPanel1.SetRowSpan(this.lvReads, 3);
            this.lvReads.Size = new System.Drawing.Size(223, 453);
            this.lvReads.TabIndex = 0;
            this.lvReads.UseCompatibleStateImageBehavior = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 229F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 214F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel1.Controls.Add(this.lvReads, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.grpMonitor, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lvResidents, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 79F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 47F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 294F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(556, 459);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // grpMonitor
            // 
            this.grpMonitor.Controls.Add(this.radOff);
            this.grpMonitor.Controls.Add(this.radOn);
            this.grpMonitor.Location = new System.Drawing.Point(446, 3);
            this.grpMonitor.Name = "grpMonitor";
            this.grpMonitor.Size = new System.Drawing.Size(106, 67);
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
            this.radOff.CheckedChanged += new System.EventHandler(this.MonitorCheckChanged);
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
            this.radOn.CheckedChanged += new System.EventHandler(this.MonitorCheckChanged);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(446, 82);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(106, 41);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.ClearButtonClick);
            // 
            // lvResidents
            // 
            this.lvResidents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvResidents.Location = new System.Drawing.Point(232, 3);
            this.lvResidents.Name = "lvResidents";
            this.tableLayoutPanel1.SetRowSpan(this.lvResidents, 3);
            this.lvResidents.Size = new System.Drawing.Size(208, 453);
            this.lvResidents.TabIndex = 4;
            this.lvResidents.UseCompatibleStateImageBehavior = false;
            // 
            // Monitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(556, 459);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Monitor";
            this.Text = "RFID Reader Monitor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MonitorFormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.grpMonitor.ResumeLayout(false);
            this.grpMonitor.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvReads;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox grpMonitor;
        private System.Windows.Forms.RadioButton radOff;
        private System.Windows.Forms.RadioButton radOn;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.ListView lvResidents;
    }
}

