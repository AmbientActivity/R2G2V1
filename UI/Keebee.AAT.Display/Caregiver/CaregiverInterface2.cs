using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using MetroFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class CaregiverInterface2 : Form
    {
        // event handler
        public event EventHandler CaregiverCompleteEvent;

        private SystemEventLogger _systemEventLogger;

        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private IEnumerable<ResponseTypePaths> _publicMedia;

        public IEnumerable<ResponseTypePaths> PublicMediaFiles
        {
            set { _publicMedia = value; }
        }

        private Config _config;

        public Config Config
        {
            set { _config = value; }
        }

        private readonly Resident _publicProfile = new Resident
        {
            Id = PublicProfileSource.Id,
            FirstName = PublicProfileSource.Description,
            GameDifficultyLevel = 1
        };

        #region local variables

        // constants
        private const int TabIndexImagesGeneral = 0;
        private const int TabIndexMusic = 1;
        private const int TabIndexRadioShows = 2;
        private const int TabIndexTVShows = 3;
        private const int TabIndexHomeMovies = 4;
        private const int TabIndexActivities = 5;
        private const int TabIndexImagesPersonal = 6;

        // delegate
        private delegate void RaiseCaregiverCompleteEventDelegate();

        // background workers
        private BackgroundWorker _bgwImageGeneralThumbnails;
        private BackgroundWorker _bgwImagePersonalThumbnails;
        private BackgroundWorker _bgwTVShowThumbnails;
        private BackgroundWorker _bgwHomeMovieThumbnails;

        // image lists
        private ImageList _imageListImagesGeneral;
        private ImageList _imageListImagesPersonal;
        private ImageList _imageListTVShows;
        private ImageList _imageListHomeMovies;
        private readonly ImageList _imageListAudio;

        private IEnumerable<ResponseTypePaths> _media;

        // playlist
        private string[] _musicPlaylist;
        private string[] _radioShowPlaylist;
        private int _totalSongs;
        private int _totalRadioShows;

        // current values
        private Resident _currentResident;
        private int _currentMusicIndex;
        private int _currentRadioShowIndex;
        private string[] _currentImageGeneralFiles;
        private string[] _currentImagePersonalFiles;
        private string[] _currentTVShowFiles;
        private string[] _currentHomeMovieFiles;

        // audio state image indices
        private const int ImageIndexPlay = 0;
        private const int ImageIndexPlayActive = 1;
        private const int ImageIndexPause = 2;

        // grid view column indices
        private const int GridViewAudioCellImage = 0;
        private const int GridViewAudioCellStatus = 1;
        private const int GridViewColumnStreamId = 2;

        private const int GridViewIActivitiesColumnDifficultyLevel = 0;
        private const int GridViewIActivitiesColumnName = 1;
        private const int GridViewIActivitiesColumnId = 2;

#if DEBUG
        private const int ThumbnailDimensions = 16;
        private const int GridViewAudioColWidthStatus = 70;
        private const int GridViewAudioColWidthName = 540;
        private const int GridViewMediaColWidthName = 620;
        private const int GridViewActivitiesColWidthName = 625;

        //private const int LabelMediaSourceFontSize = 10;
        //private const int LabelMediaSourceMarginTop = 25;
        private const int ComboBoxResidentWidth = 465;

        private const int TableLayoutPanelColOneWidth = 100;
        private const int TableLayoutPanelColTwoWidth = 465;

        private const int TabPaddingX = 3;
        private const int TabPaddingY = 3;

        private const int GridFontSize = 9;
        private const int GridRowHeight = 25;
#elif !DEBUG
        private const int ThumbnailDimensions = 64;
        private const int GridViewAudioColWidthStatus = 120;
        private const int GridViewAudioColWidthName = 1730;
        private const int GridViewMediaColWidthName = 1850;
        private const int GridViewActivitiesColWidthName = 1910;

        //private const int LabelMediaSourceFontSize = 20;
        //private const int LabelMediaSourceMarginTop = 20;
        private const int ComboBoxResidentWidth = 650;

        private const int TableLayoutPanelColOneWidth = 200;
        private const int TableLayoutPanelColTwoWidth = 650;

        private const int TabPaddingX = 30;
        private const int TabPaddingY = 15;

        private const int GridFontSize = 16;
        private const int GridRowHeight = 55;
#endif
        // to prevent background workers from lingering
        private bool _formIsClosing;

        #endregion

        private readonly ResidentsClient _residentsClient = new ResidentsClient();
        private readonly ResidentMediaFilesClient _residentMediaFilesClient = new ResidentMediaFilesClient();

        public CaregiverInterface2()
        {
            InitializeComponent();
#if DEBUG
            _imageListAudio = imageListMusicDebug;
#elif !DEBUG
            _imageListAudio = imageListMusic;
#endif
            ConfigureControls();
            ConfigureBackgroundWorkers();
            InitializeStartupPosition();
            _currentResident = _publicProfile;
        }

        #region initiialization

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width / 3;
            Height = SystemInformation.PrimaryMonitorSize.Height / 3;

#elif !DEBUG
            WindowState = FormWindowState.Maximized;
#endif
        }

        private void ConfigureControls()
        {
            ConfigureGridMedia(grdImagesGeneral);
            ConfigureGridMedia(grdMusic);
            ConfigureGridMedia(grdRadioShows);
            ConfigureGridMedia(grdTVShows);
            ConfigureGridMedia(grdHomeMovies);
            ConfigureGridMedia(grdImagesPersonal);
            ConfigureGridViewActivities();

            ConfigureTableLayout();
            ConfigureDropdown();
            ConfigureTabLayout();

            musicPlayer.Hide();
            musicPlayer.settings.volume = MediaPlayerControl.DefaultVolume;

            radioShowPlayer.Hide();
            radioShowPlayer.settings.volume = MediaPlayerControl.DefaultVolume;
        }

        private void ConfigureTableLayout()
        {
            tableLayoutPanel1.ColumnStyles[0].Width = TableLayoutPanelColOneWidth;
            tableLayoutPanel1.ColumnStyles[1].Width = TableLayoutPanelColTwoWidth;
        }

        private void ConfigureDropdown()
        {
            lblMediaSource.FontSize = MetroLabelSize.Tall;
            cboResident.FontSize = MetroComboBoxSize.Tall;
            cboResident.Width = ComboBoxResidentWidth;
        }

        private void ConfigureTabLayout()
        {
            tbMedia.Padding = new Point(TabPaddingX, TabPaddingY);
#if !DEBUG
            tbMedia.FontSize = MetroTabControlSize.Tall;
#endif
        }

        private static void ConfigureGridMediaStyle(DataGridView grd)
        {
            grd.MultiSelect = false;
            grd.RowHeadersVisible = false;
            grd.Font = new Font("Segoe UI", GridFontSize);
            grd.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.WhiteSmoke
            };
            grd.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White
            };
            grd.DefaultCellStyle = new DataGridViewCellStyle
            {
                ForeColor = Color.FromArgb(50, 50, 50),
                SelectionBackColor = Color.FromArgb(220, 220, 220),
                SelectionForeColor = Color.Black
            };
        }

        private void ConfigureGridMedia(DataGridView grd)
        {
            ConfigureGridMediaStyle(grd);

            switch (grd.Name)
            {
                case "grdImagesGeneral":
                    grd.Columns.Add(new DataGridViewImageColumn
                    {
                        HeaderText = null,
                        Width = ThumbnailDimensions,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                    _imageListImagesGeneral = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    break;
                case "grdImagesPersonal":
                    grd.Columns.Add(new DataGridViewImageColumn
                    {
                        HeaderText = null,
                        Width = ThumbnailDimensions,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                    _imageListImagesPersonal = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    break;
                case "grdMusic":
                case "grdRadioShows":
                    grd.Columns.Add(new DataGridViewImageColumn
                    {
                        HeaderText = null,
                        Width = ThumbnailDimensions,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable              
                    });
                    grd.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "State",                     
                        Width = GridViewAudioColWidthStatus,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                    break;
                case "grdTVShows":
                    grd.Columns.Add(new DataGridViewImageColumn
                    {
                        HeaderText = null,
                        Width = ThumbnailDimensions,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                    _imageListTVShows = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    break;
                case "grdHomeMovies":
                    grd.Columns.Add(new DataGridViewImageColumn
                    {
                        HeaderText = null,
                        Width = ThumbnailDimensions,
                        Resizable = DataGridViewTriState.False,
                        SortMode = DataGridViewColumnSortMode.NotSortable
                    });
                    _imageListImagesPersonal = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    _imageListHomeMovies = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    break;
            }

            grd.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Description",
                Width = grd.Name == "grdMusic" || grd.Name == "grdRadioShows"
                    ? GridViewAudioColWidthName
                    : GridViewMediaColWidthName,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
            grd.Columns.Add(new DataGridViewTextBoxColumn { Visible = false });
        }

        private void ConfigureGridViewActivities()
        {
            ConfigureGridMediaStyle(grdActivities);

            // game difficulty level
            grdActivities.Columns.Add(new DataGridViewTextBoxColumn { Visible = false });

            grdActivities.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Description",
                Width = GridViewActivitiesColWidthName,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            // response id
            grdActivities.Columns.Add(new DataGridViewTextBoxColumn { Visible = false });
        }

        private void ConfigureBackgroundWorkers()
        {
            // image general thumbnails
            _bgwImageGeneralThumbnails = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bgwImageGeneralThumbnails.DoWork += LoadImagesGeneralGridViewThumbnails;
            _bgwImageGeneralThumbnails.ProgressChanged += UpdateImagesGeneralGridViewImage;
            _bgwImageGeneralThumbnails.RunWorkerCompleted += LoadImagesGeneralGridViewThumbnailsCompleted;

            // image personal thumbnails
            _bgwImagePersonalThumbnails = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bgwImagePersonalThumbnails.DoWork += LoadImagesPersonalGridViewThumbnails;
            _bgwImagePersonalThumbnails.ProgressChanged += UpdateImagesPersonalGridViewImage;
            _bgwImagePersonalThumbnails.RunWorkerCompleted += LoadImagesPersonalGridViewThumbnailsCompleted;

            // tv show thumbnails
            _bgwTVShowThumbnails = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bgwTVShowThumbnails.DoWork += LoadTVShowsGridViewThumbnails;
            _bgwTVShowThumbnails.ProgressChanged += UpdateTVShowsGridViewImage;
            _bgwTVShowThumbnails.RunWorkerCompleted += LoadTVShowsGridViewThumbnailsCompleted;

            // home movie thumbnails
            _bgwHomeMovieThumbnails = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _bgwHomeMovieThumbnails.DoWork += LoadHomeMoviesGridViewThumbnails;
            _bgwHomeMovieThumbnails.ProgressChanged += UpdateHomeMoviesGridViewImage;
            _bgwHomeMovieThumbnails.RunWorkerCompleted += LoadHomeMoviesGridViewThumbnailsCompleted;
        }

        #endregion

        #region loaders
  
        private void LoadResidentMedia(int residentId)
        {
            _currentResident = residentId == PublicProfileSource.Id
                ? _publicProfile
                : _residentsClient.Get(residentId);

            if (residentId == PublicProfileSource.Id)
            {
                _media = _publicMedia;
            }
            else
            {
                var media = _residentMediaFilesClient.GetForResident(_currentResident.Id);
                _media = media ?? new List<ResponseTypePaths>();
            }
        }

        private void LoadResidentDropDown()
        {
            try
            {
                var residents = _residentsClient.Get().ToList();
                var arrayList = new ArrayList();

                var residentList = new List<Resident> { _publicProfile }
                    .Union(residents
                        .OrderBy(o => o.LastName).ThenBy(o => o.FirstName))
                    .ToArray();

                foreach (var r in residentList)
                {
                    var name = (r.LastName != null)
                        ? $"{r.LastName}, {r.FirstName}"
                        : r.FirstName;

                    arrayList.Add(new { r.Id, Name = name });
                }

                cboResident.ValueMember = "Id";
                cboResident.DisplayMember = "Name";
                cboResident.DataSource = arrayList;
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadResidentDropDown: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadGridViewTVShows()
        {
            try
            {
                grdTVShows.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.TVShows, ResponseTypeId.Television);

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell());
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdTVShows.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdTVShows.RowCount > 1)
                {
                    grdTVShows.FirstDisplayedScrollingRowIndex = 1;
                    grdTVShows.FirstDisplayedScrollingRowIndex = 0;
                }
                grdTVShows.ClearSelection();

                _currentTVShowFiles = GetFilePaths(MediaPathTypeId.TVShows, ResponseTypeId.Television);

                if (_bgwTVShowThumbnails.IsBusy) return;
                _bgwTVShowThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewTVShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadGridViewHomeMovies()
        {
            try
            {
                grdHomeMovies.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.HomeMovies, ResponseTypeId.Television);

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell());
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdHomeMovies.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdHomeMovies.RowCount > 1)
                {
                    grdHomeMovies.FirstDisplayedScrollingRowIndex = 1;
                    grdHomeMovies.FirstDisplayedScrollingRowIndex = 0;
                }
                grdHomeMovies.ClearSelection();

                _currentHomeMovieFiles = GetFilePaths(MediaPathTypeId.HomeMovies, ResponseTypeId.Television);

                if (_bgwHomeMovieThumbnails.IsBusy) return;
                _bgwHomeMovieThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewHomeMovies: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void LoadGridViewImagesGeneral()
        {
            try
            {
                grdImagesGeneral.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.ImagesGeneral, ResponseTypeId.SlideShow).ToArray();

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell());
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdImagesGeneral.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdImagesGeneral.RowCount > 1)
                {
                    grdImagesGeneral.FirstDisplayedScrollingRowIndex = 1;
                    grdImagesGeneral.FirstDisplayedScrollingRowIndex = 0;
                }
                grdImagesGeneral.ClearSelection();

                _currentImageGeneralFiles = GetFilePaths(MediaPathTypeId.ImagesGeneral, ResponseTypeId.SlideShow);

                if (_bgwImageGeneralThumbnails.IsBusy) return;
                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewImagesGeneral: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadGridViewMusic()
        {
            try
            {
                grdMusic.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.Music, ResponseTypeId.Radio).ToArray();
                _totalSongs = files.Count();

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell { Value = _imageListAudio.Images[ImageIndexPlay] });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = null });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdMusic.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdMusic.RowCount > 1)
                {
                    grdMusic.FirstDisplayedScrollingRowIndex = 1;
                    grdMusic.FirstDisplayedScrollingRowIndex = 0;
                }
                grdMusic.ClearSelection();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadGridViewRadioShows()
        {
            try
            {
                grdRadioShows.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.RadioShows, ResponseTypeId.Radio).ToArray();

                _totalRadioShows = files.Count();

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell { Value = _imageListAudio.Images[ImageIndexPlay] });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = null });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdRadioShows.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdRadioShows.RowCount > 1)
                {
                    grdRadioShows.FirstDisplayedScrollingRowIndex = 1;
                    grdRadioShows.FirstDisplayedScrollingRowIndex = 0;
                }
                grdRadioShows.ClearSelection();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewRadioShows: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void LoadGridViewImagesPersonal()
        {
            try
            {
                grdImagesPersonal.Rows.Clear();

                var files = GetMediaFiles(MediaPathTypeId.ImagesPersonal);

                foreach (var f in files)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewImageCell());
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.Filename });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = f.StreamId });
                    row.Height = GridRowHeight;
                    grdImagesPersonal.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdImagesPersonal.RowCount > 1)
                {
                    grdImagesPersonal.FirstDisplayedScrollingRowIndex = 1;
                    grdImagesPersonal.FirstDisplayedScrollingRowIndex = 0;
                }
                grdImagesPersonal.ClearSelection();

                _currentImagePersonalFiles = GetFilePaths(MediaPathTypeId.ImagesPersonal);

                if (_bgwImagePersonalThumbnails.IsBusy) return;
                _bgwImagePersonalThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewImagesPersonal: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void LoadGridViewInteractiveActivities(int residentId)
        {
            try
            {
                grdActivities.Rows.Clear();

                var gameDifficulatyLevel = (residentId > 0)
                    ? _currentResident.GameDifficultyLevel
                    : 1;

                var interactiveResponseTypes = _config.ConfigDetails
                    .Where(rt => rt.ResponseType.InteractiveActivityType != null)
                    .Select(rt => rt.ResponseType)
                    .GroupBy(rt => rt.Id, (key, r) => r.FirstOrDefault())
                    .ToArray();

                foreach (var rt in interactiveResponseTypes)
                {
                    var row = new DataGridViewRow();
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = gameDifficulatyLevel });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = rt.Description });
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = rt.InteractiveActivityType.Id });
                    row.Height = GridRowHeight;
                    grdActivities.Rows.Add(row);
                }
                // scrolling to the top requires 2 statements for some reason
                if (grdActivities.RowCount > 1)
                {
                    grdActivities.FirstDisplayedScrollingRowIndex = 1;
                    grdActivities.FirstDisplayedScrollingRowIndex = 0;
                }
                grdActivities.ClearSelection();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadGridViewInteractiveActivities: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void LoadTabs(int residentId)
        {
            ShowPersonalMediaTabs(residentId > 0);

            LoadGridViewImagesGeneral();
            LoadGridViewMusic();
            LoadGridViewRadioShows();
            LoadGridViewTVShows();
            LoadGridViewInteractiveActivities(residentId);

            if (TabPageExists(tabHomeMovies))
                LoadGridViewHomeMovies();

            if (TabPageExists(tabImagesPersonal))
                LoadGridViewImagesPersonal();
        }

        private void LoadMusicPlaylist()
        {
            switch (musicPlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPause];
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = "Playing...";
                    break;
                case WMPPlayState.wmppsPaused:
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlayActive];
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = "Paused";
                    break;
                default:
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];
                    grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = string.Empty;
                    _musicPlaylist = GetFilePaths(MediaPathTypeId.Music, ResponseTypeId.Radio);
                    musicPlayer.Ctlcontrols.stop();
                    _currentMusicIndex = 0;
                    break;
            }
        }

        private void LoadRadioShowPlaylist()
        {
            switch (radioShowPlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                    grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPause];
                    grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = "Playing...";
                    break;
                case WMPPlayState.wmppsPaused:
                    grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlayActive];
                    grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = "Paused";
                    break;
                default:
                    _radioShowPlaylist = GetFilePaths(MediaPathTypeId.RadioShows, ResponseTypeId.Radio);
                    radioShowPlayer.Ctlcontrols.stop();
                    _currentRadioShowIndex = 0;
                    break;
            }
        }

        #endregion

        #region file queries

        // get filenames with full path (for the thumbnails)
        private string[] GetFilePaths(int mediaPathTypeId, int? responseTypeId = null, Guid? streamId = null)
        {
            string[] filePaths = null;
            try
            {
                if (_media != null)
                {
                    var mediaFileQuery = new Helpers.MediaFileQuery(_media, _publicMedia, _currentResident.Id);
                    filePaths = mediaFileQuery.GetFilePaths(mediaPathTypeId, responseTypeId, streamId);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetFilePaths: {ex.Message}", EventLogEntryType.Error);
            }

            return filePaths;
        }

        // get filenames with no extensions or path (for displaying on the screen)
        private IEnumerable<MediaFile> GetMediaFiles(int mediaPathTypeId, int? responseTypeId = null)
        {
            IEnumerable<MediaFile> files = null;
            try
            {
                if (_media != null)
                {
                    var mediaFileQuery = new Helpers.MediaFileQuery(_media, _publicMedia, _currentResident.Id);
                    files = mediaFileQuery.GetMediaFiles(mediaPathTypeId, responseTypeId);
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetMediaFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        #endregion

        #region play media

        private void DisplayImages(int mediaTypeId, Guid selectedStreamId, int? responseTypdId = null)
        {
            try
            {
                var images = GetFilePaths(mediaTypeId, responseTypdId, selectedStreamId);

                if (File.Exists(images[0]))
                {
                    var imageViewer = new ImageViewer
                    {
                        EventLogger = _systemEventLogger,
                        Images = images
                    };

                    imageViewer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom { MessageText = "This image is no longer available" };
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.DisplayImages: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayMusic(int selectedIndex)
        {
            try
            {
                grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = string.Empty;
                grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];

                _currentMusicIndex = selectedIndex;

                musicPlayer.URL = _musicPlaylist[_currentMusicIndex];
                musicPlayer.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayRadioShow(int selectedIndex)
        {
            try
            {
                grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = string.Empty;
                grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];

                _currentRadioShowIndex = selectedIndex;

                radioShowPlayer.URL = _radioShowPlaylist[_currentRadioShowIndex];
                radioShowPlayer.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayRadioShow: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayTVShows(Guid selectedStreamId)
        {
            try
            {
                var videos = GetFilePaths(MediaPathTypeId.TVShows, ResponseTypeId.Television, selectedStreamId);

                if (File.Exists(videos[0]))
                {
                    var videoPlayer = new VideoPlayer
                    {
                        EventLogger = _systemEventLogger,
                        Video = videos[0] // just play one at a time
                    };

                    StopAudio();
                    videoPlayer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom { MessageText = "This video is no longer available" };
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayTVShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayHomeMovies(Guid selectedStreamId)
        {
            try
            {
                var videos = GetFilePaths(MediaPathTypeId.HomeMovies, ResponseTypeId.Television, selectedStreamId);

                if (File.Exists(videos[0]))
                {
                    var videoPlayer = new VideoPlayer
                    {
                        EventLogger = _systemEventLogger,
                        Video = videos[0] // just play one at a time
                    };

                    StopAudio();
                    videoPlayer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom { MessageText = "This video is no longer available" };
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayHomeMovies: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StopAudio()
        {
            StopMusic();
            StopRadioShows();
        }

        private void StopMusic()
        {
            try
            {
                if (musicPlayer.playState != WMPPlayState.wmppsPlaying &&
                    musicPlayer.playState != WMPPlayState.wmppsPaused)
                    return;

                grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];
                grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = string.Empty;

                musicPlayer.Ctlcontrols.stop();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.StopMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StopRadioShows()
        {
            try
            {
                if (radioShowPlayer.playState != WMPPlayState.wmppsPlaying &&
                    radioShowPlayer.playState != WMPPlayState.wmppsPaused)
                    return;

                grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];
                grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = string.Empty;

                radioShowPlayer.Ctlcontrols.stop();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.StopRadioShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayInteractiveActivity(int interactiveActivityId, int difficultyLevel,
            string interactiveActivityType)
        {
            try
            {
                switch (interactiveActivityId)
                {
                    case InteractiveActivityTypeId.MatchingGame:
                        var shapes = GetFilePaths(MediaPathTypeId.MatchingGameShapes, ResponseTypeId.MatchingGame);
                        var sounds = GetFilePaths(MediaPathTypeId.MatchingGameSounds, ResponseTypeId.MatchingGame);

                        // ensure there are enough shapes and sounds to play the game
                        var gameSetup = new MatchingGameSetup();
                        var totalShapes = gameSetup.GetTotalShapes(shapes);
                        var totalSounds = gameSetup.GetTotalSounds(sounds);

                        var matchingGamePlayer = new InteractiveActivityPlayer
                        {
                            InteractiveActivityId = interactiveActivityId,
                            ResidentId = _currentResident.Id,
                            SystemEventLogger = _systemEventLogger,
                            Shapes = totalShapes,
                            Sounds = totalSounds,
                            DifficultyLevel = difficultyLevel,
                            ActivityName = interactiveActivityType,
                            IsActiveEventLog = _config.IsActiveEventLog
                        };
                        StopAudio();
                        matchingGamePlayer.ShowDialog();
                        break;

                    case InteractiveActivityTypeId.PaintingActivity:
                        var paintingActivityPlayer = new InteractiveActivityPlayer
                        {
                            InteractiveActivityId = interactiveActivityId,
                            ResidentId = _currentResident.Id,
                            SystemEventLogger = _systemEventLogger,
                            ActivityName = interactiveActivityType,
                            IsActiveEventLog = _config.IsActiveEventLog
                        };
                        StopAudio();
                        paintingActivityPlayer.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayInteractiveActivity: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        #endregion

        #region event handlers

        // to alert the caller (the Display App Main form)
        private void RaiseCaregiverCompleteEvent()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                Invoke(new RaiseCaregiverCompleteEventDelegate(RaiseCaregiverCompleteEvent));
            }
            else
            {
                CaregiverCompleteEvent?.Invoke(new object(), new EventArgs());
            }
        }

        // list views
        private void TVShowsGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(grdTVShows.SelectedRows[0].Cells[GridViewColumnStreamId].Value.ToString());

                PlayTVShows(selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.TVShowsGridViewCellClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void HomeMoviesGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(grdHomeMovies.SelectedRows[0].Cells[GridViewColumnStreamId].Value.ToString());

                PlayHomeMovies(selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.HomeMoviesGridViewCellClick: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void MusicGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedIndex = e.RowIndex;

                if (File.Exists(_musicPlaylist[selectedIndex]))
                {
                    if (_currentMusicIndex == selectedIndex)
                    {
                        if (musicPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            musicPlayer.Ctlcontrols.pause();
                            grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlayActive];
                            grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = "Paused";
                        }
                        else
                        {
                            StopRadioShows();

                            if (musicPlayer.playState != WMPPlayState.wmppsPaused)
                                musicPlayer.URL = _musicPlaylist[selectedIndex];
                            musicPlayer.Ctlcontrols.play();
                            grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPause];
                            grdMusic.Rows[_currentMusicIndex].Cells[GridViewAudioCellStatus].Value = "Playing...";
                        }
                    }
                    else
                    {
                        StopRadioShows();
                        PlayMusic(selectedIndex);
                    }
                }
                else
                {
                    var messageBox = new MessageBoxCustom { MessageText = "This song is no longer available" };
                    messageBox.ShowDialog();
                }

                // remove focus from the selected item in the GridView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.MusicGridViewCellClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void RadioShowGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedIndex = e.RowIndex;

                if (File.Exists(_radioShowPlaylist[selectedIndex]))
                {
                    if (_currentRadioShowIndex == selectedIndex)
                    {
                        if (radioShowPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            radioShowPlayer.Ctlcontrols.pause();
                            grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlayActive];
                            grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = "Paused";
                        }
                        else
                        {
                            StopMusic();

                            if (radioShowPlayer.playState != WMPPlayState.wmppsPaused)
                                radioShowPlayer.URL = _radioShowPlaylist[selectedIndex];
                            radioShowPlayer.Ctlcontrols.play();
                            grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPause];
                            grdRadioShows.Rows[_currentRadioShowIndex].Cells[GridViewAudioCellStatus].Value = "Playing...";
                        }
                    }
                    else
                    {
                        StopMusic();
                        PlayRadioShow(selectedIndex);
                    }
                }
                else
                {
                    var messageBox = new MessageBoxCustom { MessageText = "This show is no longer available" };
                    messageBox.ShowDialog();
                }

                // remove focus from the selected item in the GridView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.RadioShowGridViewCellClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ImagesGeneralGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(grdImagesGeneral.SelectedRows[0].Cells[GridViewColumnStreamId].Value.ToString());

                DisplayImages(MediaPathTypeId.ImagesGeneral, selectedStreamId, ResponseTypeId.SlideShow);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ImagesGeneralGridViewCellClick: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void ImagesPersonalGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(grdImagesPersonal.SelectedRows[0].Cells[GridViewColumnStreamId].Value.ToString());

                DisplayImages(MediaPathTypeId.ImagesPersonal, selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ImagesPersonalGridViewCellClick: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        private void ActivitiesGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var interactiveActivityId = Convert.ToInt32(grdActivities.SelectedRows[0]
                    .Cells[GridViewIActivitiesColumnId].Value);

                var difficultyLevel = Convert.ToInt32(grdActivities.SelectedRows[0]
                    .Cells[GridViewIActivitiesColumnDifficultyLevel].Value);

                var interactiveActivityType = grdActivities.SelectedRows[0]
                    .Cells[GridViewIActivitiesColumnName].Value.ToString();

                PlayInteractiveActivity(interactiveActivityId, difficultyLevel, interactiveActivityType);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ActivitiesGridViewCellClick: {ex.Message}",
                    EventLogEntryType.Error);
            }
        }

        // media player
        private void PlayStateChangeMusic(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                var index = _currentMusicIndex;

                switch (e.newState)
                {
                    case (int)WMPPlayState.wmppsPlaying:
                        grdMusic.Rows[index].Cells[GridViewAudioCellStatus].Value = "Playing...";
                        grdMusic.Rows[index].Cells[0].Value = _imageListAudio.Images[ImageIndexPause];
                        grdMusic.Rows[index].Selected = true;
                        break;

                    case (int)WMPPlayState.wmppsMediaEnded:
                        grdMusic.Rows[index].Cells[GridViewAudioCellStatus].Value = string.Empty;
                        grdMusic.Rows[index].Cells[0].Value = _imageListAudio.Images[ImageIndexPlay];

                        if (_currentMusicIndex < _totalSongs - 1)
                        {
                            _currentMusicIndex++;
                            musicPlayer.URL = _musicPlaylist[_currentMusicIndex];
                        }
                        break;

                    case (int)WMPPlayState.wmppsReady:
                        try
                        {
                            musicPlayer.Ctlcontrols.play();
                        }
                        catch { }

                        break;

                    // in case a file gets deleted while it's running
                    case (int)WMPPlayState.wmppsTransitioning:
                        if (musicPlayer.currentMedia != null)
                        {
                            if (!File.Exists(musicPlayer.currentMedia.sourceURL))
                            {
                                // get the last item in the playlist
                                var lastItem = _musicPlaylist[_totalSongs - 1];
                                var lastMedia = musicPlayer.newMedia(lastItem);

                                // if it is not the last item then play next
                                if (!musicPlayer.currentMedia.isIdentical[lastMedia])
                                {
                                    _currentMusicIndex++;
                                    musicPlayer.URL = _musicPlaylist[_currentMusicIndex];
                                }
                                else
                                {
                                    // otherwise stop the player
                                    musicPlayer.Ctlcontrols.stop();
                                    _currentMusicIndex = 0;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayStateChangeMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayStateChangeRadioShows(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                var index = _currentRadioShowIndex;

                switch (e.newState)
                {
                    case (int)WMPPlayState.wmppsPlaying:
                        grdRadioShows.Rows[index].Cells[GridViewAudioCellStatus].Value = "Playing...";
                        grdRadioShows.Rows[index].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPause];;
                        grdRadioShows.Rows[index].Selected = true;
                        break;

                    case (int)WMPPlayState.wmppsMediaEnded:
                        grdRadioShows.Rows[index].Cells[GridViewAudioCellStatus].Value = string.Empty;
                        grdRadioShows.Rows[index].Cells[GridViewAudioCellImage].Value = _imageListAudio.Images[ImageIndexPlay];;

                        if (_currentRadioShowIndex < _totalRadioShows - 1)
                        {
                            _currentRadioShowIndex++;
                            radioShowPlayer.URL = _radioShowPlaylist[_currentRadioShowIndex];
                        }
                        break;

                    case (int)WMPPlayState.wmppsReady:
                        try
                        {
                            radioShowPlayer.Ctlcontrols.play();
                        }
                        catch { }

                        break;

                    // in case a file gets deleted while it's running
                    case (int)WMPPlayState.wmppsTransitioning:
                        if (radioShowPlayer.currentMedia != null)
                        {
                            if (!File.Exists(radioShowPlayer.currentMedia.sourceURL))
                            {
                                // get the last item in the playlist
                                var lastItem = _radioShowPlaylist[_totalRadioShows - 1];
                                var lastMedia = radioShowPlayer.newMedia(lastItem);

                                // if it is not the last item then play next
                                if (!radioShowPlayer.currentMedia.isIdentical[lastMedia])
                                {
                                    _currentRadioShowIndex++;
                                    radioShowPlayer.URL = _radioShowPlaylist[_currentRadioShowIndex];
                                }
                                else
                                {
                                    // otherwise stop the player
                                    radioShowPlayer.Ctlcontrols.stop();
                                    _currentRadioShowIndex = 0;
                                }
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayStateChangeMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // configuration/management
        private void ResidentSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CancelBackgroundWorkers();

                var frmSplash = new Splash();
                frmSplash.Show();
                Application.DoEvents();

                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                LoadResidentMedia(residentId);

                StopAudio();
                LoadTabs(residentId);

                frmSplash.Close();

                LoadMusicPlaylist();
                LoadRadioShowPlaylist();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ResidentSelectedIndexChanged: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // caregiver
        private void CaregiverInterface2Shown(object sender, EventArgs e)
        {
            LoadResidentDropDown();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            _formIsClosing = true;
            Close();
        }

        private void CaregiverInterface2FormClosing(object sender, FormClosingEventArgs e)
        {
            CancelBackgroundWorkers(true);
            StopAudio();
            RaiseCaregiverCompleteEvent();
        }

        // data grid error handlers
        private void ImagesGeneralGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.ImagesGeneralGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        private void MusicGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            var message = e.Exception.Message;

            _systemEventLogger.WriteEntry($"Caregiver.MusicGridViewDataError: {message}", EventLogEntryType.Error);
        }

        private void RadioShowsGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.RadioShowsGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        private void TVShowsGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.TVShowsGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        private void HomeMoviesGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.HomeMoviesGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        private void ActivitiesGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.ActivitiesGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        private void ImagesPersonalGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            _systemEventLogger.WriteEntry($"Caregiver.ImagesPersonalGridViewDataError: {e.Exception.Message}", EventLogEntryType.Error);
        }

        #endregion

        #region helpers

        private void ShowPersonalMediaTabs(bool show)
        {
            if (show)
            {
                if (!TabPageExists(tabHomeMovies))
                    tbMedia.TabPages.Add(tabHomeMovies);
                if (!TabPageExists(tabImagesPersonal))
                    tbMedia.TabPages.Add(tabImagesPersonal);
            }
            else
            {
                tbMedia.TabPages.Remove(tabHomeMovies);
                tbMedia.TabPages.Remove(tabImagesPersonal);
            }
        }

        private bool TabPageExists(TabPage tabPage)
        {
            return tbMedia.TabPages.Cast<TabPage>().Contains(tabPage);
        }

        #endregion

        #region background workers (for thumbnails)

        // image general thumbnails
        private void UpdateImagesGeneralGridViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwImageGeneralThumbnails.CancellationPending) return;

                _imageListImagesGeneral.Images.Add((Image)e.UserState);
                grdImagesGeneral.Rows[e.ProgressPercentage].Cells[0].Value = (Image)e.UserState;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateImagesGeneralGridViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesGeneralGridViewThumbnails(object sender, DoWorkEventArgs e)
        {
            try
            {
                var rowIndex = 0;
                var files = _currentImageGeneralFiles;

                foreach (var file in files)
                {
                    if (_bgwImageGeneralThumbnails.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var thumbnail = Thumbnail.Picture.Get(file, ThumbnailDimensions);
                    _bgwImageGeneralThumbnails.ReportProgress(rowIndex, thumbnail);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesGeneralGridViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesGeneralGridViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesGeneralGridViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // image personal thumbnails
        private void UpdateImagesPersonalGridViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;

                if (_bgwImagePersonalThumbnails.CancellationPending) return;

                _imageListImagesPersonal.Images.Add((Image)e.UserState);
                grdImagesPersonal.Rows[e.ProgressPercentage].Cells[0].Value = _imageListImagesPersonal.Images[e.ProgressPercentage];
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateImagesPersonalGridViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesPersonalGridViewThumbnails(object sender, DoWorkEventArgs e)
        {
            var rowIndex = 0;
            var files = _currentImagePersonalFiles;

            foreach (var file in files)
            {
                if (_bgwImagePersonalThumbnails.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var thumbnail = Thumbnail.Picture.Get(file, ThumbnailDimensions);
                _bgwImagePersonalThumbnails.ReportProgress(rowIndex, thumbnail);

                rowIndex++;
            }
        }

        private void LoadImagesPersonalGridViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesPersonalGridViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // tv show thumbnails
        private void UpdateTVShowsGridViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwTVShowThumbnails.CancellationPending) return;

                _imageListTVShows.Images.Add((Image)e.UserState);

                if (e.ProgressPercentage < grdTVShows.Rows.Count)
                {           
                    grdTVShows.Rows[e.ProgressPercentage].Cells[0].Value = _imageListTVShows.Images[e.ProgressPercentage];
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateTVShowsGridViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTVShowsGridViewThumbnails(object sender, DoWorkEventArgs e)
        {
            try
            {
                var rowIndex = 0;
                var files = _currentTVShowFiles;

                foreach (var file in files)
                {
                    if (_bgwTVShowThumbnails.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var thumbnail = Thumbnail.Video.Get(file, 2, 5);
                    _bgwTVShowThumbnails.ReportProgress(rowIndex, thumbnail);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadTVShowsGridViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTVShowsGridViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                _bgwTVShowThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadTVShowsGridViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);

            }
        }

        // home movie thumbnails
        private void UpdateHomeMoviesGridViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwHomeMovieThumbnails.CancellationPending) return;

                _imageListHomeMovies.Images.Add((Image)e.UserState);

                if (e.ProgressPercentage < grdHomeMovies.Rows.Count)
                {
                    grdHomeMovies.Rows[e.ProgressPercentage].Cells[0].Value = _imageListTVShows.Images[e.ProgressPercentage];
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateHomeMoviesGridViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadHomeMoviesGridViewThumbnails(object sender, DoWorkEventArgs e)
        {
            try
            {
                var rowIndex = 0;
                var files = _currentHomeMovieFiles;

                foreach (var file in files)
                {
                    if (_bgwHomeMovieThumbnails.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var thumbnail = Thumbnail.Video.Get(file, 2, 5);
                    _bgwHomeMovieThumbnails.ReportProgress(rowIndex, thumbnail);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadHomeMoviesGridViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadHomeMoviesGridViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                _bgwHomeMovieThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadHomeMoviesGridViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // cancel workers
        private void CancelBackgroundWorkers(bool dispose = false)
        {
            if (_bgwImageGeneralThumbnails != null)
            {
                if (_bgwImageGeneralThumbnails.IsBusy)
                {
                    _bgwImageGeneralThumbnails.CancelAsync();
                }
            }

            if (_bgwImagePersonalThumbnails != null)
            {
                if (_bgwImagePersonalThumbnails.IsBusy)
                {
                    _bgwImagePersonalThumbnails.CancelAsync();
                }
            }

            if (_bgwTVShowThumbnails != null)
            {
                if (_bgwTVShowThumbnails.IsBusy)
                {
                    _bgwTVShowThumbnails.CancelAsync();
                }
            }

            if (_bgwHomeMovieThumbnails != null)
            {
                if (_bgwHomeMovieThumbnails.IsBusy)
                {
                    _bgwHomeMovieThumbnails.CancelAsync();
                }
            }

            if (!dispose) return;
            _bgwImageGeneralThumbnails?.Dispose();
            _bgwImagePersonalThumbnails?.Dispose();
            _bgwTVShowThumbnails?.Dispose();
            _bgwHomeMovieThumbnails?.Dispose();
        }

        #endregion
    }
}
