using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Caregiver.CustomControls;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
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
    public partial class CaregiverInterface : Form
    {
        // event handler
        public event EventHandler CaregiverCompleteEvent;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private IEnumerable<ResponseTypePaths> _publicMediaFiles;
        public IEnumerable<ResponseTypePaths> PublicMediaFiles
        {
            set { _publicMediaFiles = value; }
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
        private const int TabIndexActivities = 4;
        private const int TabIndexHomeMovies = 5;
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

        private IEnumerable<ResponseTypePaths> _mediaFiles;

        // playlist
        private const string PlaylistCaregiver = PlaylistName.Caregiver;
        private IWMPPlaylist _musicPlaylist;
        private IWMPPlaylist _radioShowPlaylist;
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

        // image indices
        private const int ImageIndexPlay = 0;
        private const int ImageIndexPlayActive = 1;
        private const int ImageIndexPause = 2;

        // list view column indices
        private const int ListViewAudioColumnStatus = 1;
        private const int ListViewColumnStreamId = 2;

        private const int ListViewIActivitiesColumnDifficultyLevel = 1;
        private const int ListViewIActivitiesColumnName = 2;
        private const int ListViewIActivitiesColumnId = 3;

#if DEBUG
        private const int ThumbnailDimensions = 16;
        private const int ListViewAudioColWidthStatus = 70;
        private const int ListViewAudioColWidthName = 507;
        private const int ListViewMediaColWidthName = 577;
        private const int ListViewActivitiesColWidthName = 593;

        private const int LabelMediaSourceFontSize = 10;
        private const int LabelMediaSourceMarginTop = 25;
        private const int ComboBoxResidentWidth = 465;

        private const int TableLayoutPanelColOneWidth = 100;
        private const int TableLayoutPanelColTwoWidth = 465;

        private const int TabPaddingX = 3;
        private const int TabPaddingY = 3;

        private const int TabPageFontSize = 10;
        private const int TabFontSize = 10;
#elif !DEBUG
        private const int ThumbnailDimensions = 64;
        private const int ListViewAudioColWidthStatus = 150;
        private const int ListViewAudioColWidthName = 1663;
        private const int ListViewMediaColWidthName = 1813;
        private const int ListViewActivitiesColWidthName = 1877;

        private const int LabelMediaSourceFontSize = 20;
        private const int LabelMediaSourceMarginTop = 20;
        private const int ComboBoxResidentWidth = 650;

        private const int TableLayoutPanelColOneWidth = 200;
        private const int TableLayoutPanelColTwoWidth = 650;

        private const int TabPaddingX = 30;
        private const int TabPaddingY = 15;

        private const int TabFontSize = 26;
        private const int TabPageFontSize = 20;
#endif
        // to prevent background workers from lingering
        private bool _formIsClosing;

        #endregion

        private readonly ResidentsClient _residentsClient = new ResidentsClient();
        private readonly ResidentMediaFilesClient _residentMediaFilesClient = new ResidentMediaFilesClient();

        public CaregiverInterface()
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
            ConfigureListViewMedia(lvImagesGeneral);
            ConfigureListViewMedia(lvMusic);
            ConfigureListViewMedia(lvRadioShows);
            ConfigureListViewMedia(lvTVShows);
            ConfigureListViewMedia(lvHomeMovies);
            ConfigureListViewMedia(lvImagesPersonal);
            ConfigureListViewActivities();

            ConfigureTableLayout();
            ConfigureDropdown();
            ConfigureTabLayout();

            musicPlayer.Hide();
            musicPlayer.settings.volume = MediaPlayerControl.DefaultVolume;
        }

        private void ConfigureTableLayout()
        {
            tableLayoutPanel1.ColumnStyles[0].Width = TableLayoutPanelColOneWidth;
            tableLayoutPanel1.ColumnStyles[1].Width = TableLayoutPanelColTwoWidth;
        }
        
        private void ConfigureDropdown()
        {
            lblMediaSource.Font = new Font("Microsoft Sans Serif", LabelMediaSourceFontSize);
            lblMediaSource.Margin = new Padding(3, LabelMediaSourceMarginTop, 0, 0);
            cboResident.Width = ComboBoxResidentWidth;
        }

        private void ConfigureTabLayout()
        {
            tbMedia.Font = new Font("Microsoft Sans Serif", TabFontSize);
            tbMedia.Padding = new Point(TabPaddingX, TabPaddingY);

            tbMedia.TabPages[TabIndexImagesGeneral].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexMusic].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexRadioShows].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexTVShows].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexActivities].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexHomeMovies].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[TabIndexImagesPersonal].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
        }

        private void ConfigureListViewMedia(ListViewLarge lv)
        {
            lv.GridLines = true;
            lv.MultiSelect = false;
            lv.FullRowSelect = true;
            lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lv.View = View.Details;

            switch (lv.Name)
            {
                case "lvImagesGeneral":
                    _imageListImagesGeneral = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListImagesGeneral;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvMusic":
                    lv.SmallImageList = _imageListAudio;
                    lv.Columns.Add("", ThumbnailDimensions);
                    lv.Columns.Add("Status", ListViewAudioColWidthStatus);
                    break;
                case "lvRadioShows":
                    lv.SmallImageList = _imageListAudio;
                    lv.Columns.Add("", ThumbnailDimensions);
                    lv.Columns.Add("Status", ListViewAudioColWidthStatus);
                    break;
                case "lvTVShows":
                    _imageListTVShows = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListTVShows;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvHomeMovies":
                    _imageListHomeMovies = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListHomeMovies;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvImagesPersonal":
                    _imageListImagesPersonal = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListImagesPersonal;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
            }

            lv.Columns.Add("Name", lv.Name == "lvMusic" || lv.Name == "lvRadioShows" 
                ? ListViewAudioColWidthName 
                : ListViewMediaColWidthName);

            lv.Columns.Add("StreamId", 0);
        }

        private void ConfigureListViewActivities()
        {
            lvActivities.GridLines = true;
            lvActivities.MultiSelect = false;
            lvActivities.FullRowSelect = true;
            lvActivities.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvActivities.View = View.Details;

            lvActivities.SmallImageList = _imageListAudio;
            lvActivities.Columns.Add("", 0);
            lvActivities.Columns.Add("GameDifficultyLevel", 0);
            lvActivities.Columns.Add("Name", ListViewActivitiesColWidthName);
            lvActivities.Columns.Add("ResponseId", 0);
        }

        private void ConfigureBackgroundWorkers()
        {
            // image general thumbnails
            _bgwImageGeneralThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwImageGeneralThumbnails.DoWork += LoadImagesGeneralListViewThumbnails;
            _bgwImageGeneralThumbnails.ProgressChanged += UpdateImagesGeneralListViewImage;
            _bgwImageGeneralThumbnails.RunWorkerCompleted += LoadImagesGeneralListViewThumbnailsCompleted;

            // image personal thumbnails
            _bgwImagePersonalThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwImagePersonalThumbnails.DoWork += LoadImagesPersonalListViewThumbnails;
            _bgwImagePersonalThumbnails.ProgressChanged += UpdateImagesPersonalListViewImage;
            _bgwImagePersonalThumbnails.RunWorkerCompleted += LoadImagesPersonalListViewThumbnailsCompleted;

            // tv show thumbnails
            _bgwTVShowThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwTVShowThumbnails.DoWork += LoadTVShowsListViewThumbnails;
            _bgwTVShowThumbnails.ProgressChanged += UpdateTVShowsListViewImage;
            _bgwTVShowThumbnails.RunWorkerCompleted += LoadTVShowsListViewThumbnailsCompleted;

            // home movie thumbnails
            _bgwHomeMovieThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwHomeMovieThumbnails.DoWork += LoadHomeMoviesListViewThumbnails;
            _bgwHomeMovieThumbnails.ProgressChanged += UpdateHomeMoviesListViewImage;
            _bgwHomeMovieThumbnails.RunWorkerCompleted += LoadHomeMoviesListViewThumbnailsCompleted;
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
                _mediaFiles = _publicMediaFiles;
            }
            else
            {
                var media = _residentMediaFilesClient.GetForResident(_currentResident.Id);

                _mediaFiles = media != null 
                    ? _residentMediaFilesClient.GetForResident(_currentResident.Id).MediaResponseTypes 
                    : new List<ResponseTypePaths>();
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

                    arrayList.Add(new {r.Id, Name = name});
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

        private void LoadListViewTVShows()
        {
            try
            {
                lvTVShows.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.TVShows, ResponseTypeId.Television);

                var rowIndex = 0;
                foreach (var f in files)
                {
                    lvTVShows.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        f.Filename,
                                        f.StreamId.ToString()
                                    })
                                    {
                                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                                    });
                    rowIndex++;
                }

                _currentTVShowFiles = GetFilePaths(MediaPathTypeId.TVShows, ResponseTypeId.Television);

                if (_bgwTVShowThumbnails.IsBusy) return;
                lvTVShows.SmallImageList?.Images.Clear();
                _bgwTVShowThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewTVShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewHomeMovies()
        {
            try
            {
                lvHomeMovies.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.HomeMovies, ResponseTypeId.Television);

                var rowIndex = 0;
                foreach (var f in files)
                {
                    lvHomeMovies.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        f.Filename,
                                        f.StreamId.ToString()
                                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }

                _currentHomeMovieFiles = GetFilePaths(MediaPathTypeId.HomeMovies, ResponseTypeId.Television);

                if (_bgwHomeMovieThumbnails.IsBusy) return;
                lvHomeMovies.SmallImageList?.Images.Clear();
                _bgwHomeMovieThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewHomeMovies: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewImages()
        {
            try
            {
                lvImagesGeneral.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.ImagesGeneral, ResponseTypeId.SlideShow).ToArray();
                
                var rowIndex = 0;
                foreach (var f in files)
                {
                    lvImagesGeneral.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        f.Filename,
                                        f.StreamId.ToString()
                                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }

                _currentImageGeneralFiles = GetFilePaths(MediaPathTypeId.ImagesGeneral, ResponseTypeId.SlideShow);

                if (_bgwImageGeneralThumbnails.IsBusy) return;
                lvImagesGeneral.SmallImageList?.Images.Clear();
                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewImages: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewMusic()
        {
            try
            {
                lvMusic.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.Music, ResponseTypeId.Radio).ToArray();
                var rowIndex = 0;
                _totalSongs = files.Count();

                foreach (var f in files)
                {
                    lvMusic.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        string.Empty,
                                        f.Filename,
                                        f.StreamId.ToString()
                                    })
                    {
                        ImageIndex = ImageIndexPlay,
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewMusic: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewRadioShows()
        {
            try
            {
                lvRadioShows.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.RadioShows, ResponseTypeId.Radio).ToArray();
                var rowIndex = 0;
                _totalRadioShows = files.Count();

                foreach (var f in files)
                {
                    lvRadioShows.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        string.Empty,
                                        f.Filename,
                                        f.StreamId.ToString()
                                    })
                    {
                        ImageIndex = ImageIndexPlay,
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewRadioShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewImagesPersonal()
        {
            try
            {
                lvImagesPersonal.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.ImagesPersonal);

                var rowIndex = 0;

                foreach (var f in files)
                {
                    lvImagesPersonal.Items.Add(new ListViewItem(new[]
                    {
                        string.Empty,
                        f.Filename,
                        f.StreamId.ToString()
                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });

                    rowIndex++;
                }

                _currentImagePersonalFiles = GetFilePaths(MediaPathTypeId.ImagesPersonal);

                if (_bgwImagePersonalThumbnails.IsBusy) return;

                lvImagesPersonal.SmallImageList?.Images.Clear();
                _bgwImagePersonalThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewImagesPersonal: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewInteractiveActivities(int residentId)
        {
            try
            {
                lvActivities.Items.Clear();

                var gameDifficulatyLevel = (residentId > 0)
                    ? _currentResident.GameDifficultyLevel : 1;

                var interactiveResponseTypes = _config.ConfigDetails
                    .Where(rt => rt.ResponseType.InteractiveActivityType != null)
                    .Select(rt => rt.ResponseType)
                    .GroupBy(rt => rt.Id, (key, r) => r.FirstOrDefault())
                    .ToArray();

                var rowIndex = 0;
                foreach (var rt in interactiveResponseTypes)
                {
                    lvActivities.Items.Add(new ListViewItem(new[]
                    {
                        string.Empty,
                        gameDifficulatyLevel.ToString(),
                        rt.Description,
                        rt.InteractiveActivityType.Id.ToString()
                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewInteractiveActivities: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTabs(int residentId)
        {
            ShowPersonalMediaTabs(residentId > 0);

            LoadListViewImages();
            LoadListViewMusic();
            LoadListViewRadioShows();
            LoadListViewTVShows();
            LoadListViewInteractiveActivities(residentId);

            if (TabPageExists(tabHomeMovies))
                LoadListViewHomeMovies();

            if (TabPageExists(tabImagesPersonal))
                LoadListViewImagesPersonal();
        }

        private void LoadMusicPlaylist()
        {
            switch (musicPlayer.playState)
            {
                case WMPPlayState.wmppsPlaying:
                    lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPause;
                    lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                    break;
                case WMPPlayState.wmppsPaused:
                    lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlayActive;
                    lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
                    break;
                default:
                    var music = GetFilePaths(MediaPathTypeId.Music, ResponseTypeId.Radio);
                    _musicPlaylist = musicPlayer.LoadPlaylist(PlaylistCaregiver, music);

                    musicPlayer.currentPlaylist = _musicPlaylist;
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
                    lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPause;
                    lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                    break;
                case WMPPlayState.wmppsPaused:
                    lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlayActive;
                    lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
                    break;
                default:
                    var radioShows = GetFilePaths(MediaPathTypeId.RadioShows, ResponseTypeId.Radio);
                    _radioShowPlaylist = radioShowPlayer.LoadPlaylist(PlaylistCaregiver, radioShows);

                    radioShowPlayer.currentPlaylist = _radioShowPlaylist;
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
                var mediaFileQuery = new Helpers.MediaFileQuery(_mediaFiles, _publicMediaFiles, _currentResident.Id);
                filePaths = mediaFileQuery.GetFilePaths(mediaPathTypeId, responseTypeId, streamId);
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
                var mediaFileQuery = new Helpers.MediaFileQuery(_mediaFiles, _publicMediaFiles, _currentResident.Id);
                files = mediaFileQuery.GetMediaFiles(mediaPathTypeId, responseTypeId);  
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
                lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;
                _currentMusicIndex = selectedIndex;
                
                var media = _musicPlaylist.Item[_currentMusicIndex];
                musicPlayer.Ctlcontrols.playItem(media);
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
                lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlay;
                _currentRadioShowIndex = selectedIndex;

                var media = _radioShowPlaylist.Item[_currentRadioShowIndex];
                radioShowPlayer.Ctlcontrols.playItem(media);
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
                        Videos = videos
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
                        Videos = videos
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

                lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;
                lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;

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

                lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlay;
                lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;

                radioShowPlayer.Ctlcontrols.stop();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.StopRadioShows: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayInteractiveActivity(int interactiveActivityId, int difficultyLevel, string interactiveActivityType)
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
                _systemEventLogger.WriteEntry($"Caregiver.PlayInteractiveActivity: {ex.Message}", EventLogEntryType.Error);
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
        private void TVShowsListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvTVShows.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                PlayTVShows(selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.TVShowsListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void HomeMoviesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvHomeMovies.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                PlayHomeMovies(selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.HomeMoviesListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MusicListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvMusic.SelectedIndices[0];
                if (File.Exists(_musicPlaylist.Item[selectedIndex].sourceURL))
                {
                    if (_currentMusicIndex == selectedIndex)
                    {
                        if (musicPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            musicPlayer.Ctlcontrols.pause();
                            lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlayActive;
                            lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
                        }
                        else
                        {
                            StopRadioShows();
                            musicPlayer.Ctlcontrols.play();
                            lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPause;
                            lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
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

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.MusicListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void RadioShowListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvRadioShows.SelectedIndices[0];
                if (File.Exists(_radioShowPlaylist.Item[selectedIndex].sourceURL))
                {
                    if (_currentRadioShowIndex == selectedIndex)
                    {
                        if (radioShowPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            radioShowPlayer.Ctlcontrols.pause();
                            lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlayActive;
                            lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
                        }
                        else
                        {
                            StopMusic();
                            radioShowPlayer.Ctlcontrols.play();
                            lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPause;
                            lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
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

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.MusicListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ImagesGeneralListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvImagesGeneral.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                DisplayImages(MediaPathTypeId.ImagesGeneral, selectedStreamId, ResponseTypeId.SlideShow);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ImagesGeneralListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ImagesPersonalListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvImagesPersonal.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                DisplayImages(MediaPathTypeId.ImagesPersonal, selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ImagesPersonalListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void InteractivitiesActivitiesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var interactiveActivityId = Convert.ToInt32(lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnId].Text);

                var difficultyLevel = Convert.ToInt32(lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnDifficultyLevel].Text);

                var interactiveActivityType = lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnName].Text;

                PlayInteractiveActivity(interactiveActivityId, difficultyLevel, interactiveActivityType);              
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.InteractivitiesActivitiesListViewClick: {ex.Message}", EventLogEntryType.Error);
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
                        index = musicPlayer.CurrentIndex(_musicPlaylist);
                        lvMusic.Items[index].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                        lvMusic.Items[index].ImageIndex = ImageIndexPause;
                        lvMusic.Items[index].Selected = true;
                        _currentMusicIndex = musicPlayer.CurrentIndex(_musicPlaylist);
                        break;

                    case (int)WMPPlayState.wmppsMediaEnded:
                        lvMusic.Items[index].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                        lvMusic.Items[index].ImageIndex = ImageIndexPlay;
                        break;

                    case (int)WMPPlayState.wmppsTransitioning:
                        if (musicPlayer.currentMedia != null)
                        {
                            if (!File.Exists(musicPlayer.currentMedia.sourceURL))
                            {
                                // get the last item in the playlist
                                var lastItem = _musicPlaylist.Item[_totalSongs - 1];

                                // if it is not the last item then play next
                                if (!musicPlayer.currentMedia.isIdentical[lastItem])
                                    musicPlayer.Ctlcontrols.next();
                                else
                                    // otherwise stop the player
                                    musicPlayer.Ctlcontrols.stop();
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
                        index = radioShowPlayer.CurrentIndex(_radioShowPlaylist);
                        lvRadioShows.Items[index].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                        lvRadioShows.Items[index].ImageIndex = ImageIndexPause;
                        lvRadioShows.Items[index].Selected = true;
                        _currentRadioShowIndex = radioShowPlayer.CurrentIndex(_radioShowPlaylist);
                        break;

                    case (int)WMPPlayState.wmppsMediaEnded:
                        lvRadioShows.Items[index].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                        lvRadioShows.Items[index].ImageIndex = ImageIndexPlay;
                        break;

                    case (int)WMPPlayState.wmppsTransitioning:
                        if (radioShowPlayer.currentMedia != null)
                        {
                            if (!File.Exists(radioShowPlayer.currentMedia.sourceURL))
                            {
                                // get the last item in the playlist
                                var lastItem = _radioShowPlaylist.Item[_totalRadioShows - 1];

                                // if it is not the last item then play next
                                if (!radioShowPlayer.currentMedia.isIdentical[lastItem])
                                    radioShowPlayer.Ctlcontrols.next();
                                else
                                    // otherwise stop the player
                                    radioShowPlayer.Ctlcontrols.stop();
                            }
                        }
                        break;
                }
                _currentRadioShowIndex = index;
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
                frmSplash.BringToFront();
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
        private void CaregiverInterfaceShown(object sender, EventArgs e)
        {
            LoadResidentDropDown();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            _formIsClosing = true;
            Close();
        }

        private void CaregiverInterfaceFormClosing(object sender, FormClosingEventArgs e)
        {
            CancelBackgroundWorkers(true);
            StopAudio();
            musicPlayer.ClearPlaylist(PlaylistCaregiver);
            radioShowPlayer.ClearPlaylist(PlaylistCaregiver);
            RaiseCaregiverCompleteEvent();
        }

        // disable column resizing for all list views
        private void ImagesGeneralListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvImagesGeneral.Columns[e.ColumnIndex].Width;
        }

        private void MusicListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvMusic.Columns[e.ColumnIndex].Width;
        }

        private void RadioShowsListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvRadioShows.Columns[e.ColumnIndex].Width;
        }

        private void TVShowsListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvTVShows.Columns[e.ColumnIndex].Width;
        }

        private void ActivitiesListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvActivities.Columns[e.ColumnIndex].Width;
        }

        private void HomeMoviesListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvHomeMovies.Columns[e.ColumnIndex].Width;
        }

        private void ImagesPersonalListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvImagesPersonal.Columns[e.ColumnIndex].Width;
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
        private void UpdateImagesGeneralListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwImageGeneralThumbnails.CancellationPending) return;

                _imageListImagesGeneral.Images.Add((Image) e.UserState);
                lvImagesGeneral.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdatImagesListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesGeneralListViewThumbnails(object sender, DoWorkEventArgs e)
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
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesGeneralListViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesGeneralListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvImagesGeneral.SmallImageList?.Images.Clear();
                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesGeneralListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // image personal thumbnails
        private void UpdateImagesPersonalListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;

                if (_bgwImagePersonalThumbnails.CancellationPending) return;

                _imageListImagesPersonal.Images.Add((Image)e.UserState);
                lvImagesPersonal.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateImagesPersonalListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesPersonalListViewThumbnails(object sender, DoWorkEventArgs e)
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

        private void LoadImagesPersonalListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvImagesPersonal.SmallImageList?.Images.Clear();
                _bgwImageGeneralThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesPersonalListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // tv show thumbnails
        private void UpdateTVShowsListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwTVShowThumbnails.CancellationPending) return;
                
                _imageListTVShows.Images.Add((Image)e.UserState);
                lvTVShows.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateTVShowsListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTVShowsListViewThumbnails(object sender, DoWorkEventArgs e)
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
                _systemEventLogger.WriteEntry($"Caregiver.LoadTVShowsListViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTVShowsListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvTVShows.SmallImageList?.Images.Clear();
                _bgwTVShowThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadTVShowsListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);

            }
        }

        // home movie thumbnails
        private void UpdateHomeMoviesListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_formIsClosing) return;
                if (_bgwHomeMovieThumbnails.CancellationPending) return;

                _imageListHomeMovies.Images.Add((Image) e.UserState);

                if (e.ProgressPercentage < lvHomeMovies.Items.Count)
                {
                    lvHomeMovies.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdateHomeMoviesListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadHomeMoviesListViewThumbnails(object sender, DoWorkEventArgs e)
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
                _systemEventLogger.WriteEntry($"Caregiver.LoadHomeMoviesListViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadHomeMoviesListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvHomeMovies.SmallImageList?.Images.Clear();
                _bgwHomeMovieThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadHomeMoviesListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);

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
        }

        #endregion
    }
}
