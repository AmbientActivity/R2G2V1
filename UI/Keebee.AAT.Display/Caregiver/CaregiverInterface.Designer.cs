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
            this.tabImagesGeneral = new System.Windows.Forms.TabPage();
            this.lvImagesGeneral = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabMusic = new System.Windows.Forms.TabPage();
            this.lvMusic = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabRadioShows = new System.Windows.Forms.TabPage();
            this.lvRadioShows = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabTVShows = new System.Windows.Forms.TabPage();
            this.lvTVShows = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabActivities = new System.Windows.Forms.TabPage();
            this.lvActivities = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabHomeMovies = new System.Windows.Forms.TabPage();
            this.lvHomeMovies = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.tabImagesPersonal = new System.Windows.Forms.TabPage();
            this.lvImagesPersonal = new Keebee.AAT.Display.Caregiver.CustomControls.ListViewLarge();
            this.cboResident = new Keebee.AAT.Display.Caregiver.CustomControls.ComboBoxLarge();
            this.lblMediaSource = new System.Windows.Forms.Label();
            this.musicPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.radioShowPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            this.btnClose = new MetroFramework.Controls.MetroButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.tbMedia.SuspendLayout();
            this.tabImagesGeneral.SuspendLayout();
            this.tabMusic.SuspendLayout();
            this.tabRadioShows.SuspendLayout();
            this.tabTVShows.SuspendLayout();
            this.tabActivities.SuspendLayout();
            this.tabHomeMovies.SuspendLayout();
            this.tabImagesPersonal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.musicPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioShowPlayer)).BeginInit();
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
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 526F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 73F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbMedia, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cboResident, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMediaSource, 0, 0);
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
            this.tableLayoutPanel1.SetColumnSpan(this.tbMedia, 3);
            this.tbMedia.Controls.Add(this.tabImagesGeneral);
            this.tbMedia.Controls.Add(this.tabMusic);
            this.tbMedia.Controls.Add(this.tabRadioShows);
            this.tbMedia.Controls.Add(this.tabTVShows);
            this.tbMedia.Controls.Add(this.tabActivities);
            this.tbMedia.Controls.Add(this.tabHomeMovies);
            this.tbMedia.Controls.Add(this.tabImagesPersonal);
            this.tbMedia.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMedia.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMedia.Location = new System.Drawing.Point(3, 76);
            this.tbMedia.Name = "tbMedia";
            this.tbMedia.SelectedIndex = 0;
            this.tbMedia.Size = new System.Drawing.Size(688, 325);
            this.tbMedia.TabIndex = 0;
            // 
            // tabImagesGeneral
            // 
            this.tabImagesGeneral.Controls.Add(this.lvImagesGeneral);
            this.tabImagesGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabImagesGeneral.Name = "tabImagesGeneral";
            this.tabImagesGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabImagesGeneral.Size = new System.Drawing.Size(680, 296);
            this.tabImagesGeneral.TabIndex = 0;
            this.tabImagesGeneral.Text = "Images";
            this.tabImagesGeneral.UseVisualStyleBackColor = true;
            // 
            // lvImagesGeneral
            // 
            this.lvImagesGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvImagesGeneral.FullRowSelect = false;
            this.lvImagesGeneral.GridLines = false;
            this.lvImagesGeneral.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvImagesGeneral.Location = new System.Drawing.Point(3, 3);
            this.lvImagesGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.lvImagesGeneral.MultiSelect = true;
            this.lvImagesGeneral.Name = "lvImagesGeneral";
            this.lvImagesGeneral.Size = new System.Drawing.Size(674, 290);
            this.lvImagesGeneral.SmallImageList = null;
            this.lvImagesGeneral.TabIndex = 0;
            this.lvImagesGeneral.View = System.Windows.Forms.View.LargeIcon;
            this.lvImagesGeneral.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ImagesGeneralListViewColumnWidthChanging);
            this.lvImagesGeneral.ItemClicked += new System.EventHandler(this.ImagesGeneralListViewClick);
            // 
            // tabMusic
            // 
            this.tabMusic.Controls.Add(this.lvMusic);
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
            // tabRadioShows
            // 
            this.tabRadioShows.Controls.Add(this.lvRadioShows);
            this.tabRadioShows.Location = new System.Drawing.Point(4, 25);
            this.tabRadioShows.Name = "tabRadioShows";
            this.tabRadioShows.Padding = new System.Windows.Forms.Padding(3);
            this.tabRadioShows.Size = new System.Drawing.Size(680, 296);
            this.tabRadioShows.TabIndex = 3;
            this.tabRadioShows.Text = "Radio Shows";
            this.tabRadioShows.UseVisualStyleBackColor = true;
            // 
            // lvRadioShows
            // 
            this.lvRadioShows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvRadioShows.FullRowSelect = false;
            this.lvRadioShows.GridLines = false;
            this.lvRadioShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvRadioShows.Location = new System.Drawing.Point(3, 3);
            this.lvRadioShows.Margin = new System.Windows.Forms.Padding(4);
            this.lvRadioShows.MultiSelect = true;
            this.lvRadioShows.Name = "lvRadioShows";
            this.lvRadioShows.Size = new System.Drawing.Size(674, 290);
            this.lvRadioShows.SmallImageList = null;
            this.lvRadioShows.TabIndex = 7;
            this.lvRadioShows.View = System.Windows.Forms.View.LargeIcon;
            this.lvRadioShows.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.RadioShowsListViewColumnWidthChanging);
            this.lvRadioShows.ItemClicked += new System.EventHandler(this.RadioShowListViewClick);
            // 
            // tabTVShows
            // 
            this.tabTVShows.Controls.Add(this.lvTVShows);
            this.tabTVShows.Location = new System.Drawing.Point(4, 25);
            this.tabTVShows.Name = "tabTVShows";
            this.tabTVShows.Padding = new System.Windows.Forms.Padding(3);
            this.tabTVShows.Size = new System.Drawing.Size(680, 296);
            this.tabTVShows.TabIndex = 1;
            this.tabTVShows.Text = "TV Shows";
            this.tabTVShows.UseVisualStyleBackColor = true;
            // 
            // lvTVShows
            // 
            this.lvTVShows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTVShows.FullRowSelect = false;
            this.lvTVShows.GridLines = false;
            this.lvTVShows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvTVShows.Location = new System.Drawing.Point(3, 3);
            this.lvTVShows.Margin = new System.Windows.Forms.Padding(4);
            this.lvTVShows.MultiSelect = true;
            this.lvTVShows.Name = "lvTVShows";
            this.lvTVShows.Size = new System.Drawing.Size(674, 290);
            this.lvTVShows.SmallImageList = null;
            this.lvTVShows.TabIndex = 0;
            this.lvTVShows.View = System.Windows.Forms.View.LargeIcon;
            this.lvTVShows.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.TVShowsListViewColumnWidthChanging);
            this.lvTVShows.ItemClicked += new System.EventHandler(this.TVShowsListViewClick);
            // 
            // tabActivities
            // 
            this.tabActivities.Controls.Add(this.lvActivities);
            this.tabActivities.Location = new System.Drawing.Point(4, 25);
            this.tabActivities.Name = "tabActivities";
            this.tabActivities.Padding = new System.Windows.Forms.Padding(3);
            this.tabActivities.Size = new System.Drawing.Size(680, 296);
            this.tabActivities.TabIndex = 5;
            this.tabActivities.Text = "Activities";
            this.tabActivities.UseVisualStyleBackColor = true;
            // 
            // lvActivities
            // 
            this.lvActivities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvActivities.FullRowSelect = false;
            this.lvActivities.GridLines = false;
            this.lvActivities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvActivities.Location = new System.Drawing.Point(3, 3);
            this.lvActivities.Margin = new System.Windows.Forms.Padding(4);
            this.lvActivities.MultiSelect = true;
            this.lvActivities.Name = "lvActivities";
            this.lvActivities.Size = new System.Drawing.Size(674, 290);
            this.lvActivities.SmallImageList = null;
            this.lvActivities.TabIndex = 0;
            this.lvActivities.View = System.Windows.Forms.View.LargeIcon;
            this.lvActivities.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ActivitiesListViewColumnWidthChanging);
            this.lvActivities.ItemClicked += new System.EventHandler(this.InteractivitiesActivitiesListViewClick);
            // 
            // tabHomeMovies
            // 
            this.tabHomeMovies.Controls.Add(this.lvHomeMovies);
            this.tabHomeMovies.Location = new System.Drawing.Point(4, 25);
            this.tabHomeMovies.Name = "tabHomeMovies";
            this.tabHomeMovies.Padding = new System.Windows.Forms.Padding(3);
            this.tabHomeMovies.Size = new System.Drawing.Size(680, 296);
            this.tabHomeMovies.TabIndex = 7;
            this.tabHomeMovies.Text = "Home Movies";
            this.tabHomeMovies.UseVisualStyleBackColor = true;
            // 
            // lvHomeMovies
            // 
            this.lvHomeMovies.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvHomeMovies.FullRowSelect = false;
            this.lvHomeMovies.GridLines = false;
            this.lvHomeMovies.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvHomeMovies.Location = new System.Drawing.Point(3, 3);
            this.lvHomeMovies.Margin = new System.Windows.Forms.Padding(5);
            this.lvHomeMovies.MultiSelect = true;
            this.lvHomeMovies.Name = "lvHomeMovies";
            this.lvHomeMovies.Size = new System.Drawing.Size(674, 290);
            this.lvHomeMovies.SmallImageList = null;
            this.lvHomeMovies.TabIndex = 1;
            this.lvHomeMovies.View = System.Windows.Forms.View.LargeIcon;
            this.lvHomeMovies.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.HomeMoviesListViewColumnWidthChanging);
            this.lvHomeMovies.ItemClicked += new System.EventHandler(this.HomeMoviesListViewClick);
            // 
            // tabImagesPersonal
            // 
            this.tabImagesPersonal.Controls.Add(this.lvImagesPersonal);
            this.tabImagesPersonal.Location = new System.Drawing.Point(4, 25);
            this.tabImagesPersonal.Name = "tabImagesPersonal";
            this.tabImagesPersonal.Padding = new System.Windows.Forms.Padding(3);
            this.tabImagesPersonal.Size = new System.Drawing.Size(680, 296);
            this.tabImagesPersonal.TabIndex = 4;
            this.tabImagesPersonal.Text = "Personal Images";
            this.tabImagesPersonal.UseVisualStyleBackColor = true;
            // 
            // lvImagesPersonal
            // 
            this.lvImagesPersonal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvImagesPersonal.FullRowSelect = false;
            this.lvImagesPersonal.GridLines = false;
            this.lvImagesPersonal.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Clickable;
            this.lvImagesPersonal.Location = new System.Drawing.Point(3, 3);
            this.lvImagesPersonal.Margin = new System.Windows.Forms.Padding(4);
            this.lvImagesPersonal.MultiSelect = true;
            this.lvImagesPersonal.Name = "lvImagesPersonal";
            this.lvImagesPersonal.Size = new System.Drawing.Size(674, 290);
            this.lvImagesPersonal.SmallImageList = null;
            this.lvImagesPersonal.TabIndex = 0;
            this.lvImagesPersonal.View = System.Windows.Forms.View.LargeIcon;
            this.lvImagesPersonal.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.ImagesPersonalListViewColumnWidthChanging);
            this.lvImagesPersonal.ItemClicked += new System.EventHandler(this.ImagesPersonalListViewClick);
            // 
            // cboResident
            // 
            this.cboResident.Location = new System.Drawing.Point(98, 15);
            this.cboResident.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.cboResident.Name = "cboResident";
            this.cboResident.SelectedValue = null;
            this.cboResident.Size = new System.Drawing.Size(520, 47);
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
            // musicPlayer
            // 
            this.musicPlayer.Enabled = true;
            this.musicPlayer.Location = new System.Drawing.Point(3, 369);
            this.musicPlayer.Name = "musicPlayer";
            this.musicPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("musicPlayer.OcxState")));
            this.musicPlayer.Size = new System.Drawing.Size(92, 32);
            this.musicPlayer.TabIndex = 4;
            this.musicPlayer.Visible = false;
            this.musicPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.PlayStateChangeMusic);
            // 
            // radioShowPlayer
            // 
            this.radioShowPlayer.Enabled = true;
            this.radioShowPlayer.Location = new System.Drawing.Point(101, 369);
            this.radioShowPlayer.Name = "radioShowPlayer";
            this.radioShowPlayer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("radioShowPlayer.OcxState")));
            this.radioShowPlayer.Size = new System.Drawing.Size(94, 34);
            this.radioShowPlayer.TabIndex = 8;
            this.radioShowPlayer.Visible = false;
            this.radioShowPlayer.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.PlayStateChangeRadioShows);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = global::Keebee.AAT.Display.Properties.Resources.close;
            this.btnClose.Location = new System.Drawing.Point(624, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(66, 65);
            this.btnClose.TabIndex = 21;
            this.btnClose.UseSelectable = true;
            this.btnClose.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // CaregiverInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 404);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.musicPlayer);
            this.Controls.Add(this.radioShowPlayer);
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
            this.tabImagesGeneral.ResumeLayout(false);
            this.tabMusic.ResumeLayout(false);
            this.tabRadioShows.ResumeLayout(false);
            this.tabTVShows.ResumeLayout(false);
            this.tabActivities.ResumeLayout(false);
            this.tabHomeMovies.ResumeLayout(false);
            this.tabImagesPersonal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.musicPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radioShowPlayer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ImageList imageListMusic;
        private ImageList imageListMusicDebug;
        private TableLayoutPanel tableLayoutPanel1;
        private TabControl tbMedia;
        private TabPage tabImagesGeneral;
        private CustomControls.ListViewLarge lvImagesGeneral;
        private TabPage tabMusic;
        private CustomControls.ListViewLarge lvMusic;
        private AxWMPLib.AxWindowsMediaPlayer musicPlayer;
        private TabPage tabTVShows;
        private CustomControls.ListViewLarge lvTVShows;
        private TabPage tabActivities;
        private CustomControls.ListViewLarge lvActivities;
        private TabPage tabImagesPersonal;
        private CustomControls.ListViewLarge lvImagesPersonal;
        private CustomControls.ComboBoxLarge cboResident;
        private Label lblMediaSource;
        private TabPage tabRadioShows;
        private TabPage tabHomeMovies;
        private CustomControls.ListViewLarge lvHomeMovies;
        private CustomControls.ListViewLarge lvRadioShows;
        private AxWMPLib.AxWindowsMediaPlayer radioShowPlayer;
        private MetroFramework.Controls.MetroButton btnClose;
    }
}