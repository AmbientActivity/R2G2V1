using System.Windows.Forms;

namespace Keebee.AAT.Display.Caregiver
{
    partial class CaregiverInterface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaregiverInterface));
            this.imageListMusic = new System.Windows.Forms.ImageList(this.components);
            this.imageListMusicDebug = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbMedia = new System.Windows.Forms.TabControl();
            this.tabImages = new System.Windows.Forms.TabPage();
            this.lvImages = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabMusic = new System.Windows.Forms.TabPage();
            this.lvMusic = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            this.tabVideos = new System.Windows.Forms.TabPage();
            this.lvVideos = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabActivities = new System.Windows.Forms.TabPage();
            this.lvInteractiveResponses = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabPictures = new System.Windows.Forms.TabPage();
            this.lvPictures = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.btnClose = new System.Windows.Forms.Button();
            this.cboResident = new Keebee.AAT.Display.Caregiver.CustomControls.ComboBoxLarge();
            this.lblMediaSource = new System.Windows.Forms.Label();
            this.btnVolume = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tbMedia.SuspendLayout();
            this.tabImages.SuspendLayout();
            this.tabMusic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.tabVideos.SuspendLayout();
            this.tabActivities.SuspendLayout();
            this.tabPictures.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageListMusic
            // 
            this.imageListMusic.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMusic.ImageStream")));
            this.imageListMusic.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMusic.Images.SetKeyName(0, "play.png");
            this.imageListMusic.Images.SetKeyName(1, "play_active.png");
            this.imageListMusic.Images.SetKeyName(2, "pause.png");
            // 
            // imageListMusicDebug
            // 
            this.imageListMusicDebug.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMusicDebug.ImageStream")));
            this.imageListMusicDebug.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListMusicDebug.Images.SetKeyName(0, "play.png");
            this.imageListMusicDebug.Images.SetKeyName(1, "play_active.png");
            this.imageListMusicDebug.Images.SetKeyName(2, "pause.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 446F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.Controls.Add(this.tbMedia, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.cboResident, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMediaSource, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnVolume, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(694, 404);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // tbMedia
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tbMedia, 4);
            this.tbMedia.Controls.Add(this.tabImages);
            this.tbMedia.Controls.Add(this.tabMusic);
            this.tbMedia.Controls.Add(this.tabVideos);
            this.tbMedia.Controls.Add(this.tabActivities);
            this.tbMedia.Controls.Add(this.tabPictures);
            this.tbMedia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMedia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMedia.Location = new System.Drawing.Point(3, 76);
            this.tbMedia.Name = "tbMedia";
            this.tbMedia.SelectedIndex = 0;
            this.tbMedia.Size = new System.Drawing.Size(688, 325);
            this.tbMedia.TabIndex = 0;
            // 
            // tabImages
            // 
            this.tabImages.Controls.Add(this.lvImages);
            this.tabImages.Location = new System.Drawing.Point(4, 25);
            this.tabImages.Name = "tabImages";
            this.tabImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabImages.Size = new System.Drawing.Size(680, 296);
            this.tabImages.TabIndex = 0;
            this.tabImages.Text = "Images";
            this.tabImages.UseVisualStyleBackColor = true;
            // 
            // lvImages
            // 
            this.lvImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvImages.FullRowSelect = false;
            this.lvImages.GridLines = false;
            this.lvImages.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvImages.Location = new System.Drawing.Point(3, 3);
            this.lvImages.Margin = new System.Windows.Forms.Padding(4);
            this.lvImages.MultiSelect = true;
            this.lvImages.Name = "lvImages";
            this.lvImages.Size = new System.Drawing.Size(674, 290);
            this.lvImages.SmallImageList = null;
            this.lvImages.TabIndex = 0;
            this.lvImages.View = System.Windows.Forms.View.LargeIcon;
            this.lvImages.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ImagesListViewColumnWidthChanging);
            this.lvImages.ItemClicked += new System.EventHandler(this.ImagesListViewClick);
            // 
            // tabMusic
            // 
            this.tabMusic.Controls.Add(this.lvMusic);
            this.tabMusic.Controls.Add(this.axWindowsMediaPlayer1);
            this.tabMusic.Location = new System.Drawing.Point(4, 25);
            this.tabMusic.Name = "tabMusic";
            this.tabMusic.Padding = new System.Windows.Forms.Padding(3);
            this.tabMusic.Size = new System.Drawing.Size(680, 296);
            this.tabMusic.TabIndex = 3;
            this.tabMusic.Text = "Music";
            this.tabMusic.UseVisualStyleBackColor = true;
            // 
            // lvMusic
            // 
            this.lvMusic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMusic.FullRowSelect = false;
            this.lvMusic.GridLines = false;
            this.lvMusic.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvMusic.Location = new System.Drawing.Point(3, 3);
            this.lvMusic.Margin = new System.Windows.Forms.Padding(4);
            this.lvMusic.MultiSelect = true;
            this.lvMusic.Name = "lvMusic";
            this.lvMusic.Size = new System.Drawing.Size(674, 290);
            this.lvMusic.SmallImageList = null;
            this.lvMusic.TabIndex = 5;
            this.lvMusic.View = System.Windows.Forms.View.LargeIcon;
            this.lvMusic.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.MusicListViewColumnWidthChanging);
            this.lvMusic.ItemClicked += new System.EventHandler(this.MusicListViewClick);
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(562, 6);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(66, 40);
            this.axWindowsMediaPlayer1.TabIndex = 4;
            this.axWindowsMediaPlayer1.Visible = false;
            this.axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.PlayStateChange);
            // 
            // tabVideos
            // 
            this.tabVideos.Controls.Add(this.lvVideos);
            this.tabVideos.Location = new System.Drawing.Point(4, 25);
            this.tabVideos.Name = "tabVideos";
            this.tabVideos.Padding = new System.Windows.Forms.Padding(3);
            this.tabVideos.Size = new System.Drawing.Size(680, 296);
            this.tabVideos.TabIndex = 1;
            this.tabVideos.Text = "Videos";
            this.tabVideos.UseVisualStyleBackColor = true;
            // 
            // lvVideos
            // 
            this.lvVideos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvVideos.FullRowSelect = false;
            this.lvVideos.GridLines = false;
            this.lvVideos.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvVideos.Location = new System.Drawing.Point(3, 3);
            this.lvVideos.Margin = new System.Windows.Forms.Padding(4);
            this.lvVideos.MultiSelect = true;
            this.lvVideos.Name = "lvVideos";
            this.lvVideos.Size = new System.Drawing.Size(674, 290);
            this.lvVideos.SmallImageList = null;
            this.lvVideos.TabIndex = 0;
            this.lvVideos.View = System.Windows.Forms.View.LargeIcon;
            this.lvVideos.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.VideosListViewColumnWidthChanging);
            this.lvVideos.ItemClicked += new System.EventHandler(this.VideosListViewClick);
            // 
            // tabActivities
            // 
            this.tabActivities.Controls.Add(this.lvInteractiveResponses);
            this.tabActivities.Location = new System.Drawing.Point(4, 25);
            this.tabActivities.Name = "tabActivities";
            this.tabActivities.Padding = new System.Windows.Forms.Padding(3);
            this.tabActivities.Size = new System.Drawing.Size(680, 296);
            this.tabActivities.TabIndex = 5;
            this.tabActivities.Text = "Activities";
            this.tabActivities.UseVisualStyleBackColor = true;
            // 
            // lvInteractiveResponses
            // 
            this.lvInteractiveResponses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvInteractiveResponses.FullRowSelect = false;
            this.lvInteractiveResponses.GridLines = false;
            this.lvInteractiveResponses.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvInteractiveResponses.Location = new System.Drawing.Point(3, 3);
            this.lvInteractiveResponses.Margin = new System.Windows.Forms.Padding(4);
            this.lvInteractiveResponses.MultiSelect = true;
            this.lvInteractiveResponses.Name = "lvInteractiveResponses";
            this.lvInteractiveResponses.Size = new System.Drawing.Size(674, 290);
            this.lvInteractiveResponses.SmallImageList = null;
            this.lvInteractiveResponses.TabIndex = 0;
            this.lvInteractiveResponses.View = System.Windows.Forms.View.LargeIcon;
            this.lvInteractiveResponses.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.InteractiveResponsesListViewColumnWidthChanging);
            this.lvInteractiveResponses.ItemClicked += new System.EventHandler(this.InteractiveResponsesListViewClick);
            // 
            // tabPictures
            // 
            this.tabPictures.Controls.Add(this.lvPictures);
            this.tabPictures.Location = new System.Drawing.Point(4, 25);
            this.tabPictures.Name = "tabPictures";
            this.tabPictures.Padding = new System.Windows.Forms.Padding(3);
            this.tabPictures.Size = new System.Drawing.Size(680, 296);
            this.tabPictures.TabIndex = 4;
            this.tabPictures.Text = "Pictures";
            this.tabPictures.UseVisualStyleBackColor = true;
            // 
            // lvPictures
            // 
            this.lvPictures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvPictures.FullRowSelect = false;
            this.lvPictures.GridLines = false;
            this.lvPictures.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvPictures.Location = new System.Drawing.Point(3, 3);
            this.lvPictures.Margin = new System.Windows.Forms.Padding(4);
            this.lvPictures.MultiSelect = true;
            this.lvPictures.Name = "lvPictures";
            this.lvPictures.Size = new System.Drawing.Size(674, 290);
            this.lvPictures.SmallImageList = null;
            this.lvPictures.TabIndex = 0;
            this.lvPictures.View = System.Windows.Forms.View.LargeIcon;
            this.lvPictures.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.PicturesListViewColumnWidthChanging);
            this.lvPictures.ItemClicked += new System.EventHandler(this.PicturesListViewClick);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(627, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 64);
            this.btnClose.TabIndex = 6;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // cboResident
            // 
            this.cboResident.Location = new System.Drawing.Point(98, 15);
            this.cboResident.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.cboResident.Name = "cboResident";
            this.cboResident.SelectedValue = null;
            this.cboResident.Size = new System.Drawing.Size(440, 47);
            this.cboResident.TabIndex = 7;
            this.cboResident.SelectedIndexChanged += new System.EventHandler(this.ResidentSelectedIndexChanged);
            // 
            // lblMediaSource
            // 
            this.lblMediaSource.AutoSize = true;
            this.lblMediaSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMediaSource.Location = new System.Drawing.Point(3, 25);
            this.lblMediaSource.Margin = new System.Windows.Forms.Padding(3, 25, 0, 0);
            this.lblMediaSource.Name = "lblMediaSource";
            this.lblMediaSource.Size = new System.Drawing.Size(92, 16);
            this.lblMediaSource.TabIndex = 3;
            this.lblMediaSource.Text = "Media Source";
            // 
            // btnVolume
            // 
            this.btnVolume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVolume.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolume.Image = ((System.Drawing.Image)(resources.GetObject("btnVolume.Image")));
            this.btnVolume.Location = new System.Drawing.Point(548, 3);
            this.btnVolume.Name = "btnVolume";
            this.btnVolume.Size = new System.Drawing.Size(70, 64);
            this.btnVolume.TabIndex = 0;
            this.btnVolume.UseVisualStyleBackColor = true;
            this.btnVolume.Click += new System.EventHandler(this.VolumeAdjusterButtonClick);
            // 
            // CaregiverInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 404);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CaregiverInterface";
            this.ShowInTaskbar = false;
            this.Text = "Caregiver Interface";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CaregiverInterfaceFormClosing);
            this.Shown += new System.EventHandler(this.CaregiverInterfaceShown);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tbMedia.ResumeLayout(false);
            this.tabImages.ResumeLayout(false);
            this.tabMusic.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.tabVideos.ResumeLayout(false);
            this.tabActivities.ResumeLayout(false);
            this.tabPictures.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageListMusic;
        private ImageList imageListMusicDebug;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl tbMedia;
        private TabPage tabImages;
        private CustomControls.ListViewLarge lvImages;
        private TabPage tabMusic;
        private CustomControls.ListViewLarge lvMusic;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private TabPage tabVideos;
        private CustomControls.ListViewLarge lvVideos;
        private TabPage tabActivities;
        private CustomControls.ListViewLarge lvInteractiveResponses;
        private TabPage tabPictures;
        private CustomControls.ListViewLarge lvPictures;
        private Button btnClose;
        private CustomControls.ComboBoxLarge cboResident;
        private Label lblMediaSource;
        private Button btnVolume;
    }
}