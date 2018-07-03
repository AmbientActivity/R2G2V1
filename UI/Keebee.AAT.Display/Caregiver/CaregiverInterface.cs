using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Caregiver.CustomControls;
using Keebee.AAT.Display.Helpers;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Display.Caregiver.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
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
        #region declaration

        // players
        private ImageViewer _imageViewer;
        private InteractiveActivityPlayer _activityPlayer;
        private VideoPlayer _videoPlayer;

        private int _timeout;
        private Config _config;
        private IEnumerable<ResponseTypePaths> _publicMedia;

        private readonly Resident _publicProfile = new Resident
        {
            Id = PublicProfileSource.Id,
            FirstName = PublicProfileSource.Description,
            GameDifficultyLevel = 1
        };

        #endregion

        #region public properties

        public IEnumerable<ResponseTypePaths> PublicMediaFiles
        {
            set { _publicMedia = value; }
        }

        public Config Config
        {
            set { _config = value; }
        }

        public int Timeout
        {
            set { _timeout = value; }
        }

        #endregion

        #region local variables

        private readonly ResidentsClient _residentsClient = new ResidentsClient();
        private readonly ResidentMediaFilesClient _residentMediaFilesClient = new ResidentMediaFilesClient();
        private readonly InteractiveActivityTypesClient _interactiveActivityTypesClient = new InteractiveActivityTypesClient();
        private IEnumerable<Thumbnail> _thumbnails;

        // constants
        private const string FontFamily = "Tahoma";

        private const int TabIndexImagesGeneral = 0;
        private const int TabIndexMusic = 1;
        private const int TabIndexRadioShows = 2;
        private const int TabIndexTVShows = 3;
        private const int TabIndexActivities = 4;
        private const int TabIndexHomeMovies = 5;
        private const int TabIndexImagesPersonal = 6;     

        // timer
        private Timer _timer;

        // image lists
        private ImageList _imageListImagesGeneral;
        private ImageList _imageListImagesPersonal;
        private ImageList _imageListTVShows;
        private ImageList _imageListHomeMovies;
        private ImageList _imageListActivities;
        private readonly ImageList _imageListAudio;

        private IEnumerable<ResponseTypePaths> _media;
        private readonly IEnumerable<InteractiveActivityType> _activityTypes;

        // playlist
        private string[] _musicPlaylist;
        private string[] _radioShowPlaylist;
        private int _totalSongs;
        private int _totalRadioShows;

        // current values
        private Resident _currentResident;
        private int _currentMusicIndex;
        private int _currentRadioShowIndex;

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
        private const int ListViewIActivitiesColumnFile = 4;

#if DEBUG
        private const int ThumbnailDimensions = 16;
        private const int ListViewAudioColWidthStatus = 70;
        private const int ListViewAudioColWidthName = 507;
        private const int ListViewMediaColWidthName = 577;
        private const int ListViewActivitiesColWidthName = 593;
        private const int ListViewFontSize = 12;
        
        private const int LabelMediaSourceFontSize = 10;
        private const int LabelMediaSourceMarginTop = 25;
        private const int ComboBoxResidentWidth = 465;

        private const int TableLayoutPanelColOneWidth = 100;
        private const int TableLayoutPanelColTwoWidth = 465;

        private const int TabPaddingX = 5;
        private const int TabPaddingY = 5;

        private const int TabPageFontSize = 12;
        private const int TabFontSize = 12;
#elif !DEBUG
        private const int ThumbnailDimensions = 64;
        private const int ListViewAudioColWidthStatus = 150;
        private const int ListViewAudioColWidthName = 1663;
        private const int ListViewMediaColWidthName = 1813;
        private const int ListViewActivitiesColWidthName = 1877;
        private const int ListViewFontSize = 20;

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
        #endregion

        public CaregiverInterface()
        {
            InitializeComponent();
#if DEBUG
            _imageListAudio = imageListMusicDebug;
#elif !DEBUG
            _imageListAudio = imageListMusic;
#endif
            ConfigureControls();
            InitializeStartupPosition();
            LoadThumbnails();
            _currentResident = _publicProfile;
            _activityTypes = _interactiveActivityTypesClient.Get();
        }

        #region initiialization

        private void InitializeStartupPosition()
        {
            ShowInTaskbar = false;

#if DEBUG
            StartPosition = FormStartPosition.Manual;
            Location = new Point(0, 0);

            // set form size to 1/3 primary monitor size
            Width = SystemInformation.PrimaryMonitorSize.Width/3;
            Height = SystemInformation.PrimaryMonitorSize.Height/3;

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
            lblMediaSource.Font = new Font(FontFamily, LabelMediaSourceFontSize);
            lblMediaSource.Margin = new Padding(3, LabelMediaSourceMarginTop, 0, 0);
            cboMediaSource.Width = ComboBoxResidentWidth;
        }

        private void ConfigureTabLayout()
        {
            tbMedia.Font = new Font(FontFamily, TabFontSize);
            tbMedia.Padding = new Point(TabPaddingX, TabPaddingY);

            tbMedia.TabPages[TabIndexImagesGeneral].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexMusic].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexRadioShows].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexTVShows].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexActivities].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexHomeMovies].Font = new Font(FontFamily, TabPageFontSize);
            tbMedia.TabPages[TabIndexImagesPersonal].Font = new Font(FontFamily, TabPageFontSize);

            ShowPersonalMediaTabs(false);
        }

        private void ConfigureListViewMedia(ListViewLarge lv)
        {
            lv.GridLines = true;
            lv.MultiSelect = false;
            lv.FullRowSelect = true;
            lv.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lv.View = View.Details;
            lv.ColorListViewHeader(Color.SteelBlue, Color.White);
            lv.Font = new Font(FontFamily, ListViewFontSize);

            switch (lv.Name)
            {
                case "lvImagesGeneral":
                    _imageListImagesGeneral = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    lv.SmallImageList = _imageListImagesGeneral;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvMusic":
                    lv.SmallImageList = _imageListAudio;
                    lv.Columns.Add("", ThumbnailDimensions);
                    lv.Columns.Add("State", ListViewAudioColWidthStatus);
                    break;
                case "lvRadioShows":
                    lv.SmallImageList = _imageListAudio;
                    lv.Columns.Add("", ThumbnailDimensions);
                    lv.Columns.Add("State", ListViewAudioColWidthStatus);
                    break;
                case "lvTVShows":
                    _imageListTVShows = new ImageList {ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)};
                    lv.SmallImageList = _imageListTVShows;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvHomeMovies":
                    _imageListHomeMovies = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    lv.SmallImageList = _imageListHomeMovies;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvImagesPersonal":
                    _imageListImagesPersonal = new ImageList
                    {
                        ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
                    };
                    lv.SmallImageList = _imageListImagesPersonal;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
            }

            lv.Columns.Add("Description", lv.Name == "lvMusic" || lv.Name == "lvRadioShows" || lv.Name == "lvActivities"
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
            lvActivities.ColorListViewHeader(Color.SteelBlue, Color.White);

            _imageListActivities = new ImageList
            {
                ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions)
            };
            lvActivities.SmallImageList = _imageListActivities;
            lvActivities.Columns.Add("", ThumbnailDimensions);
            lvActivities.Columns.Add("GameDifficultyLevel", 0);
            lvActivities.Columns.Add("Description", ListViewActivitiesColWidthName);
            lvActivities.Columns.Add("ResponseId", 0);
            lvActivities.Columns.Add("SwfFile", 0);
        }

        #endregion

        #region loaders

        private void LoadThumbnails()
        {
            var thumbnailsClient = new ThumbnailsClient();
            _thumbnails = thumbnailsClient.Get();
        }

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

                var residentList = new List<Resident> {_publicProfile}
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

                cboMediaSource.ValueMember = "Id";
                cboMediaSource.DisplayMember = "Name";
                cboMediaSource.DataSource = arrayList;
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadResidentDropDown: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void LoadListViewTVShows()
        {
            try
            {
                lvTVShows.Items.Clear();
                _imageListTVShows.Images.Clear();

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

                    var image = ThumbnailHelper.GetImageFromByteArray(f.Thumbnail);
                    if (image != null)
                    {
                        _imageListTVShows.Images.Add(image);
                        lvTVShows.Items[rowIndex].ImageIndex = rowIndex;
                    }

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewTVShows: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void LoadListViewHomeMovies()
        {
            try
            {
                lvHomeMovies.Items.Clear();
                _imageListHomeMovies.Images.Clear();

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

                    var image = ThumbnailHelper.GetImageFromByteArray(f.Thumbnail);
                    if (image != null)
                    {
                        _imageListHomeMovies.Images.Add(image);
                        lvHomeMovies.Items[rowIndex].ImageIndex = rowIndex;
                    }
                    
                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewHomeMovies: {ex.Message}", SystemEventLogType.Display,
                    EventLogEntryType.Error);
            }
        }

        private void LoadListViewImages()
        {
            try
            {
                lvImagesGeneral.Items.Clear();
                _imageListImagesGeneral.Images.Clear();

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

                    var image = ThumbnailHelper.GetImageFromByteArray(f.Thumbnail);
                    if (image != null)
                    {
                        _imageListImagesGeneral.Images.Add(image);
                        lvImagesGeneral.Items[rowIndex].ImageIndex = rowIndex;
                    }

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewImages: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewMusic: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewRadioShows: {ex.Message}",
                    SystemEventLogType.Display,
                    EventLogEntryType.Error);
            }
        }

        private void LoadListViewImagesPersonal()
        {
            try
            {
                lvImagesPersonal.Items.Clear();
                _imageListImagesPersonal.Images.Clear();

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

                    var image = ThumbnailHelper.GetImageFromByteArray(f.Thumbnail);
                    if (image != null)
                    {
                        _imageListImagesPersonal.Images.Add(image);
                        lvImagesPersonal.Items[rowIndex].ImageIndex = rowIndex;
                    }

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewImagesPersonal: {ex.Message}",
                    SystemEventLogType.Display,
                    EventLogEntryType.Error);
            }
        }

        private void LoadListViewInteractiveActivities(int residentId)
        {
            try
            {
                lvActivities.Items.Clear();

                var gameDifficulatyLevel = (residentId > 0)
                    ? _currentResident.GameDifficultyLevel
                    : 1;

                var rowIndex = 0;
                foreach (var at in _activityTypes)
                {
                    lvActivities.Items.Add(new ListViewItem(new[]
                    {
                        string.Empty,
                        gameDifficulatyLevel.ToString(),
                        at.Description,
                        at.Id.ToString(),
                        at.SwfFile
                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });

                    var image = ActivityThumbnail.Get(at.Id);
                    if (image != null)
                    {
                        _imageListActivities.Images.Add(image);
                        lvActivities.Items[rowIndex].ImageIndex = rowIndex;
                    }

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.LoadListViewInteractiveActivities: {ex.Message}",
                    SystemEventLogType.Display,
                    EventLogEntryType.Error);
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
                    lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPause;
                    lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                    break;
                case WMPPlayState.wmppsPaused:
                    lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlayActive;
                    lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
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

        // get filenames with full path (and no thumbnails)
        private string[] GetFilePaths(int mediaPathTypeId, int? responseTypeId = null, Guid? streamId = null)
        {
            string[] filePaths = null;
            try
            {
                if (_media != null)
                {
                    var mediaFileQuery = new Helpers.MediaFileQuery(_media, _publicMedia, _thumbnails, _currentResident.Id);
                    filePaths = mediaFileQuery.GetFilePaths(mediaPathTypeId, responseTypeId, streamId);
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.GetFilePaths: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }

            return filePaths;
        }

        // get filenames with no extensions or path (for displaying on the screen)
        // also include thumbnail images
        private IEnumerable<MediaFileThumbnail> GetMediaFiles(int mediaPathTypeId, int? responseTypeId = null)
        {
            IEnumerable<MediaFileThumbnail> files = null;
            try
            {
                if (_media != null)
                {
                    var mediaFileQuery = new Helpers.MediaFileQuery(_media, _publicMedia, _thumbnails, _currentResident.Id);
                    files = mediaFileQuery.GetMediaFiles(mediaPathTypeId, responseTypeId);
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.GetMediaFiles: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }

            return files;
        }

        #endregion

        #region play media

        private void DisplayImages(int mediaTypeId, Guid selectedStreamId, int? responseTypeId = null)
        {
            try
            {
                var images = GetFilePaths(mediaTypeId, responseTypeId, selectedStreamId);

                if (File.Exists(images[0]))
                {
                    _imageViewer = new ImageViewer
                    {
                        Images = images,
                        Timeout = _timeout
                    };

                    _imageViewer.FormClosed += ImageViewerFormClosed;
                    _imageViewer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom {MessageText = "This image is no longer available"};
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.DisplayImages: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void PlayMusic(int selectedIndex)
        {
            try
            {
                lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;

                _currentMusicIndex = selectedIndex;

                musicPlayer.URL = _musicPlaylist[_currentMusicIndex];
                musicPlayer.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.PlayMusic: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void PlayRadioShow(int selectedIndex)
        {
            try
            {
                lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlay;

                _currentRadioShowIndex = selectedIndex;

                radioShowPlayer.URL = _radioShowPlaylist[_currentRadioShowIndex];
                radioShowPlayer.Ctlcontrols.play();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.PlayRadioShow: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void PlayTVShows(Guid selectedStreamId)
        {
            try
            {
                var videos = GetFilePaths(MediaPathTypeId.TVShows, ResponseTypeId.Television, selectedStreamId);

                if (File.Exists(videos[0]))
                {
                    _videoPlayer = new VideoPlayer
                    {
                        Video = videos[0], // just play one at a time
                        Timeout = _timeout
                    };

                    StopAudio();
                    _videoPlayer.FormClosed += VideoPlayerFormClosed;
                    _videoPlayer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom {MessageText = "This video is no longer available"};
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.PlayTVShows: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void PlayHomeMovies(Guid selectedStreamId)
        {
            try
            {
                var videos = GetFilePaths(MediaPathTypeId.HomeMovies, ResponseTypeId.Television, selectedStreamId);

                if (File.Exists(videos[0]))
                {
                    _videoPlayer = new VideoPlayer
                    {
                        Video = videos[0], // just play one at a time
                        Timeout = _timeout
                    };

                    StopAudio();
                    _videoPlayer.FormClosed += VideoPlayerFormClosed;
                    _videoPlayer.ShowDialog();
                }
                else
                {
                    var messageBox = new MessageBoxCustom {MessageText = "This video is no longer available"};
                    messageBox.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.PlayHomeMovies: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"Caregiver.StopMusic: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                SystemEventLogger.WriteEntry($"Caregiver.StopRadioShows: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void PlayInteractiveActivity(int interactiveActivityId, string swfFile, int difficultyLevel)
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

                        _activityPlayer = new InteractiveActivityPlayer
                        {
                            InteractiveActivityId = interactiveActivityId,
                            ResidentId = _currentResident.Id,
                            Shapes = totalShapes,
                            Sounds = totalSounds,
                            DifficultyLevel = difficultyLevel,
                            IsActiveEventLog = _config.IsActiveEventLog,
                            SwfFile = swfFile
                        };
                        StopAudio();
                        _activityPlayer.FormClosed += InteractiveActivityFormClosed;
                        _activityPlayer.ShowDialog();
                        break;
                    default:
                        _activityPlayer = new InteractiveActivityPlayer
                        {
                            InteractiveActivityId = interactiveActivityId,
                            ResidentId = _currentResident.Id,
                            IsActiveEventLog = _config.IsActiveEventLog,
                            SwfFile = swfFile
                        };
                        StopAudio();
                        _activityPlayer.FormClosed += InteractiveActivityFormClosed;
                        _activityPlayer.ShowDialog();
                        break;
                }

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.PlayInteractiveActivity: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
            }
        }

        #endregion

        #region event handlers

        // list views
        private void TVShowsListViewClick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                var selectedStreamId = new Guid(lvTVShows.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                PlayTVShows(selectedStreamId);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.TVShowsListViewClick: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void HomeMoviesListViewClick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                var selectedStreamId = new Guid(lvHomeMovies.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                PlayHomeMovies(selectedStreamId);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.HomeMoviesListViewClick: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
            }
        }

        private void MusicListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvMusic.SelectedIndices[0];

                if (File.Exists(_musicPlaylist[selectedIndex]))
                {
                    _timer.Stop();

                    if (_currentMusicIndex == selectedIndex)
                    {
                        if (musicPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            musicPlayer.Ctlcontrols.pause();
                            lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlayActive;
                            lvMusic.Items[_currentMusicIndex].SubItems[ListViewAudioColumnStatus].Text = "Paused";
                            _timer.Start();
                        }
                        else
                        {
                            StopRadioShows();

                            if (musicPlayer.playState != WMPPlayState.wmppsPaused)
                                musicPlayer.URL = _musicPlaylist[selectedIndex];
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
                    var messageBox = new MessageBoxCustom {MessageText = "This song is no longer available"};
                    messageBox.ShowDialog();
                }

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.MusicListViewClick: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void RadioShowListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvRadioShows.SelectedIndices[0];

                if (File.Exists(_radioShowPlaylist[selectedIndex]))
                {
                    _timer.Stop();

                    if (_currentRadioShowIndex == selectedIndex)
                    {
                        if (radioShowPlayer.playState == WMPPlayState.wmppsPlaying)
                        {
                            radioShowPlayer.Ctlcontrols.pause();
                            lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPlayActive;
                            lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text =
                                "Paused";
                            _timer.Start();
                        }
                        else
                        {
                            StopMusic();

                            if (radioShowPlayer.playState != WMPPlayState.wmppsPaused)
                                radioShowPlayer.URL = _radioShowPlaylist[selectedIndex];
                            radioShowPlayer.Ctlcontrols.play();
                            lvRadioShows.Items[_currentRadioShowIndex].ImageIndex = ImageIndexPause;
                            lvRadioShows.Items[_currentRadioShowIndex].SubItems[ListViewAudioColumnStatus].Text =
                                "Playing...";
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
                    var messageBox = new MessageBoxCustom {MessageText = "This show is no longer available"};
                    messageBox.ShowDialog();
                }

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.MusicListViewClick: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void ImagesGeneralListViewClick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                var selectedStreamId = new Guid(lvImagesGeneral.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                DisplayImages(MediaPathTypeId.ImagesGeneral, selectedStreamId, ResponseTypeId.SlideShow);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.ImagesGeneralListViewClick: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
            }
        }

        private void ImagesPersonalListViewClick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                var selectedStreamId = new Guid(lvImagesPersonal.SelectedItems[0].SubItems[ListViewColumnStreamId].Text);

                DisplayImages(MediaPathTypeId.ImagesPersonal, selectedStreamId);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.ImagesPersonalListViewClick: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
            }
        }

        private void InteractivitiesActivitiesListViewClick(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();
                var interactiveActivityId = Convert.ToInt32(lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnId].Text);

                var swfFile = lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnFile].Text;

                var difficultyLevel = Convert.ToInt32(lvActivities.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnDifficultyLevel].Text);

                PlayInteractiveActivity(interactiveActivityId, swfFile, difficultyLevel);
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.InteractivitiesActivitiesListViewClick: {ex.Message}"
                    , SystemEventLogType.Display
                    , EventLogEntryType.Error);
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
                    case (int) WMPPlayState.wmppsPlaying:
                        lvMusic.Items[index].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                        lvMusic.Items[index].ImageIndex = ImageIndexPause;
                        lvMusic.Items[index].Selected = true;
                        break;

                    case (int) WMPPlayState.wmppsMediaEnded:
                        lvMusic.Items[index].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                        lvMusic.Items[index].ImageIndex = ImageIndexPlay;

                        if (_currentMusicIndex < _totalSongs - 1)
                        {
                            _currentMusicIndex++;
                            // don't automatically advance to the next song
                            //musicPlayer.URL = _musicPlaylist[_currentMusicIndex];
                        }
                        _timer.Start();
                        break;

                    case (int) WMPPlayState.wmppsReady:
                        try
                        {
                            musicPlayer.Ctlcontrols.play();
                        }
                        catch{ }
                        
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
                SystemEventLogger.WriteEntry($"Caregiver.PlayStateChangeMusic: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
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
                        lvRadioShows.Items[index].SubItems[ListViewAudioColumnStatus].Text = "Playing...";
                        lvRadioShows.Items[index].ImageIndex = ImageIndexPause;
                        lvRadioShows.Items[index].Selected = true;
                        break;

                    case (int)WMPPlayState.wmppsMediaEnded:
                        lvRadioShows.Items[index].SubItems[ListViewAudioColumnStatus].Text = string.Empty;
                        lvRadioShows.Items[index].ImageIndex = ImageIndexPlay;

                        if (_currentRadioShowIndex < _totalRadioShows - 1)
                        {
                            _currentRadioShowIndex++;
                            // don't automatically advance to the next radio show
                            //radioShowPlayer.URL = _radioShowPlaylist[_currentRadioShowIndex];
                        }
                        _timer.Start();
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
                SystemEventLogger.WriteEntry($"Caregiver.PlayStateChangeMusic: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        // configuration/management
        private void MediaSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var frmSplash = new Splash();
                frmSplash.Show();
                Application.DoEvents();

                var residentId = Convert.ToInt32(cboMediaSource.SelectedValue.ToString());

                LoadResidentMedia(residentId);
                            
                StopAudio();
                LoadTabs(residentId);

                frmSplash.Close();

                LoadMusicPlaylist();
                LoadRadioShowPlaylist();
            }
            catch (Exception ex)
            {
                SystemEventLogger.WriteEntry($"Caregiver.MediaSourceSelectedIndexChanged: {ex.Message}", SystemEventLogType.Display, EventLogEntryType.Error);
            }
        }

        private void MediaSourceClick(object sender, EventArgs e)
        {
            ResetTimer();
        }

        // caregiver
        private void CaregiverInterfaceShown(object sender, EventArgs e)
        {
            _timer = new Timer { Interval = _timeout };
            _timer.Tick += TimerTick;
            _timer.Start();

            LoadResidentDropDown();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void CaregiverInterfaceFormClosing(object sender, FormClosingEventArgs e)
        {
            StopAudio();
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

        private void VideoPlayerFormClosed(object sender, EventArgs e)
        {
            var isTimeoutExpired = _videoPlayer.IsTimeoutExpired;
            _videoPlayer.Dispose();
            _videoPlayer = null;

            if (isTimeoutExpired)
            {
                Close();
            }
            else // closed manually
            {
                ResetTimer();
            }
        }

        private void InteractiveActivityFormClosed(object sender, EventArgs e)
        {
            var isTimeoutExpired = _activityPlayer.IsTimeoutExpired;
            _activityPlayer.Dispose();
            _activityPlayer = null;

            if (isTimeoutExpired)
            {
                Close();
            }
            else // closed manually
            {
                ResetTimer();
            }
        }

        private void ImageViewerFormClosed(object sender, EventArgs e)
        {
            var isTimeoutExpired = _imageViewer.IsTimeoutExpired;
            _imageViewer.Dispose();
            _imageViewer = null;

            if (isTimeoutExpired)
            {
                Close();
            }
            else // closed manually
            {
                ResetTimer();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Close();
        }

        private void MediaTabSelectedIndexChanged(object sender, EventArgs e)
        {
            ResetTimer();
        }

        #endregion

        #region helpers

        private void ResetTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

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
    }
}
