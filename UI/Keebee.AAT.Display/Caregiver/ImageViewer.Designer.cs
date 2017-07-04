namespace Keebee.AAT.Display.Caregiver
{
    partial class ImageViewer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewer));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.slideViewerFlash1 = new Keebee.AAT.Display.UserControls.SlideViewer();
            this.lblAutoMode = new System.Windows.Forms.Label();
            this.btnPlay = new MetroFramework.Controls.MetroButton();
            this.btnClose = new MetroFramework.Controls.MetroButton();
            this.btnPrevious = new MetroFramework.Controls.MetroButton();
            this.btnNext = new MetroFramework.Controls.MetroButton();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.White;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblAutoMode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnPlay, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnPrevious, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnNext, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 71F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(640, 360);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 6);
            this.panel1.Controls.Add(this.slideViewerFlash1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 289);
            this.panel1.TabIndex = 12;
            // 
            // slideViewerFlash1
            // 
            this.slideViewerFlash1.BackColor = System.Drawing.Color.Yellow;
            this.slideViewerFlash1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slideViewerFlash1.Location = new System.Drawing.Point(0, 0);
            this.slideViewerFlash1.Name = "slideViewerFlash1";
            this.slideViewerFlash1.Size = new System.Drawing.Size(638, 287);
            this.slideViewerFlash1.TabIndex = 1;
            this.slideViewerFlash1.Click += new System.EventHandler(this.PreviousButtonClick);
            // 
            // lblAutoMode
            // 
            this.lblAutoMode.AutoSize = true;
            this.lblAutoMode.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoMode.Location = new System.Drawing.Point(73, 319);
            this.lblAutoMode.Margin = new System.Windows.Forms.Padding(3, 30, 0, 0);
            this.lblAutoMode.Name = "lblAutoMode";
            this.lblAutoMode.Size = new System.Drawing.Size(59, 13);
            this.lblAutoMode.TabIndex = 16;
            this.lblAutoMode.Text = "Auto Mode";
            // 
            // btnPlay
            // 
            this.btnPlay.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.play_active;
            this.btnPlay.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPlay.Location = new System.Drawing.Point(3, 292);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(64, 65);
            this.btnPlay.TabIndex = 17;
            this.btnPlay.UseSelectable = true;
            this.btnPlay.Click += new System.EventHandler(this.PlayButtonClick);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(573, 292);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 64);
            this.btnClose.TabIndex = 20;
            this.btnClose.UseSelectable = true;
            this.btnClose.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // btnPrevious
            // 
            this.btnPrevious.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.arrow_left;
            this.btnPrevious.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnPrevious.Location = new System.Drawing.Point(167, 292);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(150, 64);
            this.btnPrevious.TabIndex = 18;
            this.btnPrevious.UseSelectable = true;
            this.btnPrevious.Click += new System.EventHandler(this.PreviousButtonClick);
            // 
            // btnNext
            // 
            this.btnNext.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.arrow_right;
            this.btnNext.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnNext.Location = new System.Drawing.Point(323, 292);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(150, 64);
            this.btnNext.TabIndex = 19;
            this.btnNext.UseSelectable = true;
            this.btnNext.Click += new System.EventHandler(this.NextButtonClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "play_active.png");
            this.imageList1.Images.SetKeyName(1, "pause.png");
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(640, 360);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImageViewer";
            this.ShowInTaskbar = false;
            this.Text = "Image Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageViewerFormClosing);
            this.Shown += new System.EventHandler(this.ImageViewerShown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label lblAutoMode;
        private UserControls.SlideViewer slideViewerFlash1;
        private MetroFramework.Controls.MetroButton btnPlay;
        private MetroFramework.Controls.MetroButton btnPrevious;
        private MetroFramework.Controls.MetroButton btnNext;
        private MetroFramework.Controls.MetroButton btnClose;
    }
}