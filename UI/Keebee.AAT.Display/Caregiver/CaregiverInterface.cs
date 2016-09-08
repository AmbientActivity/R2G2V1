using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Caregiver.CustomControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WMPLib;

namespace Keebee.AAT.Display.Caregiver
{
    public partial class CaregiverInterface : Form
    {
        // event handler
        public event EventHandler CaregiverCompleteEvent;

        // delegate
        private delegate void RaiseCaregiverCompleteEventDelegate();

        // background workers
        private BackgroundWorker _bgwImageThumbnails;
        private BackgroundWorker _bgwPictureThumbnails;
        private BackgroundWorker _bgwVideoThumbnails;

        // image lists
        private ImageList _imageListImages;
        private ImageList _imageListPictures;
        private ImageList _imageListVideos;
        private readonly ImageList _imageListMusic;

        private SystemEventLogger _systemEventLogger;
        public SystemEventLogger EventLogger
        {
            set { _systemEventLogger = value; }
        }

        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private Profile _genericProfile;
        public Profile GenericProfile
        {
            set { _genericProfile = value; }
        }

        private const string PlaylistCaregiver = "caregiver";
        private IWMPPlaylist _playlist;
        private int _currentMusicIndex;

        private Resident _resident;
        private int _currentResidentId = -1;
        private string[] _currentImageFiles;
        private string[] _currentPictureFiles;
        private string[] _currentVideoFiles;

        // image indices
        private const int ImageIndexPlay = 0;
        private const int ImageIndexPlayActive = 1;
        private const int ImageIndexPause = 2;

        // list view column indices
        private const int ListViewMediaColumnStreamId = 2;
        private const int ListViewMusicColumnStatus = 1;

        private const int ListViewIActivitiesColumnDifficultyLevel = 1;
        private const int ListViewIActivitiesColumnName = 2;
        private const int ListViewIActivitiesColumnResponseTypeId = 3;

        //TODO: find a way to compute these values dynamically
#if DEBUG
        private const int ThumbnailDimensions = 16;
        private const int ListViewMusicColWidthStatus = 70;
        private const int ListViewMusicColWidthName = 507;
        private const int ListViewMediaColWidthName = 577;
        private const int ListViewInteractiveResponseColWidthName = 593;

        private const int LabelMediaSourceFontSize = 10;
        private const int LabelMediaSourceMarginTop = 25;
        private const int ComboBoxResidentWidth = 375;

        private const int TableLayoutPanelColOneWidth = 100;
        private const int TableLayoutPanelColTwoWidth = 375;

        private const int TabPaddingX = 3;
        private const int TabPaddingY = 3;

        private const int TabPageFontSize = 10;
        private const int TabFontSize = 10;
#endif
#if !DEBUG
        private const int ThumbnailDimensions = 64;
        private const int ListViewMusicColWidthStatus = 150;
        private const int ListViewMusicColWidthName = 1659;
        private const int ListViewMediaColWidthName = 1873;
        private const int ListViewInteractiveResponseColWidthName = 1873;

        private const int LabelMediaSourceFontSize = 20;
        private const int LabelMediaSourceMarginTop = 20;
        private const int ComboBoxResidentWidth = 900;

        private const int TableLayoutPanelColOneWidth = 200;
        private const int TableLayoutPanelColTwoWidth = 900;

        private const int TabPaddingX = 30;
        private const int TabPaddingY = 15;

        private const int TabFontSize = 26;
        private const int TabPageFontSize = 20;
#endif

        // to prevent backgrund workers from lingering
        private bool _formIsClosing;

        public CaregiverInterface()
        {
            InitializeComponent();
#if DEBUG
            _imageListMusic = imageListMusicDebug;
#endif
#if !DEBUG
            _imageListMusic = imageListMusic;
#endif
            ConfigureControls();
            ConfigureBackgroundWorkers();
            InitializeStartupPosition();
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
            ConfigureListViewMedia(lvImages);
            ConfigureListViewMedia(lvMusic);
            ConfigureListViewMedia(lvVideos);
            ConfigureListViewMedia(lvPictures);
            ConfigureListViewInteractiveResponses();

            ConfigureTableLayout();
            ConfigureDropdown();
            ConfigureTabLayout();

            axWindowsMediaPlayer1.Hide();
            axWindowsMediaPlayer1.settings.volume = 100;
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

            tbMedia.TabPages[0].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[1].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[2].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[3].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
            tbMedia.TabPages[4].Font = new Font("Microsoft Sans Serif", TabPageFontSize);
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
                case "lvMusic":
                    lv.SmallImageList = _imageListMusic;
                    lv.Columns.Add("", ThumbnailDimensions);
                    lv.Columns.Add("Status", ListViewMusicColWidthStatus);
                    break;
                case "lvImages":
                    _imageListImages = new ImageList {ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListImages;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvPictures":
                    _imageListPictures = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListPictures;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
                case "lvVideos":
                    _imageListVideos = new ImageList { ImageSize = new Size(ThumbnailDimensions, ThumbnailDimensions) };
                    lv.SmallImageList = _imageListVideos;
                    lv.Columns.Add("", ThumbnailDimensions);
                    break;
            }

            lv.Columns.Add("Name", lv.Name == "lvMusic" ? ListViewMusicColWidthName : ListViewMediaColWidthName);
            lv.Columns.Add("StreamId", 0);
        }

        private void ConfigureListViewInteractiveResponses()
        {
            lvInteractiveResponses.GridLines = true;
            lvInteractiveResponses.MultiSelect = false;
            lvInteractiveResponses.FullRowSelect = true;
            lvInteractiveResponses.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvInteractiveResponses.View = View.Details;

            lvInteractiveResponses.SmallImageList = _imageListMusic;
            lvInteractiveResponses.Columns.Add("", 0);
            lvInteractiveResponses.Columns.Add("GameDifficultyLevel", 0);
            lvInteractiveResponses.Columns.Add("Name", ListViewInteractiveResponseColWidthName);
            lvInteractiveResponses.Columns.Add("ResponseId", 0);
        }

        private void ConfigureBackgroundWorkers()
        {
            // image thumbnails
            _bgwImageThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwImageThumbnails.DoWork += LoadImagesListViewThumbnails;
            _bgwImageThumbnails.ProgressChanged += UpdateImagesListViewImage;
            _bgwImageThumbnails.RunWorkerCompleted += LoadImagesListViewThumbnailsCompleted;

            // picture thumbnails
            _bgwPictureThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwPictureThumbnails.DoWork += LoadPicturesListViewThumbnails;
            _bgwPictureThumbnails.ProgressChanged += UpdatePicturesListViewImage;
            _bgwPictureThumbnails.RunWorkerCompleted += LoadPicturesListViewThumbnailsCompleted;

            // video thumbnails
            _bgwVideoThumbnails = new BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true };
            _bgwVideoThumbnails.DoWork += LoadVideosListViewThumbnails;
            _bgwVideoThumbnails.ProgressChanged += UpdateVideosListViewImage;
            _bgwVideoThumbnails.RunWorkerCompleted += LoadVideosListViewThumbnailsCompleted;
        }

        #endregion

        #region loaders

        private void LoadResident(int residentId)
        {
            _resident = _opsClient.GetResidentMedia(residentId);
        }

        private void LoadResidentDropDown()
        {
            try
            {
                var residents = _opsClient.GetResidents().ToList();
                var arrayList = new ArrayList();

                var residentList = new List<Resident> { new Resident { Id = 0, FirstName = _genericProfile.Description } }
                    .Union(residents
                    .OrderBy(o => o.LastName).ThenBy(o => o.FirstName))
                    .ToArray();

                foreach (var r in residentList)
                {
                    arrayList.Add(r.Id > 0
                        ? new {r.Id, Name = $"{r.LastName}, {r.FirstName}"}
                        : new {r.Id, Name = r.FirstName});
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

        private IEnumerable<Response> GetMediaResponses(int residentId, int responseTypeId)
        {
            IEnumerable<Response> list = new List<Response>();

            try
            {
                list =(residentId > 0)
                    ? _resident
                        .Profile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == responseTypeId)
                        .SelectMany(r => r.ResponseType.Responses)
                        .OrderBy(o => o.Filename)
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())

                    : _genericProfile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == responseTypeId)
                        .SelectMany(r => r.ResponseType.Responses)
                        .OrderBy(o => o.Filename)
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .ToArray();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetMediaResponses: {ex.Message}", EventLogEntryType.Error);
            }

            return list;
        }

        private void LoadListViewVideos(int residentId)
        {
            try
            {
                lvVideos.Items.Clear();

                var responses = GetMediaResponses(residentId, ResponseTypeId.Television).ToArray();
                var rowIndex = 0;
                foreach (var r in responses)
                {
                    lvVideos.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        r.Filename,
                                        r.StreamId.ToString()
                                    })
                                    {
                                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                                    });
                    rowIndex++;
                }
                _currentVideoFiles = responses.Select(x => x.FilePath).ToArray();

                if (_bgwVideoThumbnails.IsBusy) return;
                lvVideos.SmallImageList?.Images.Clear();
                _bgwVideoThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewVideos: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewImages(int residentId)
        {
            try
            {
                lvImages.Items.Clear();

                var responses = GetMediaResponses(residentId, ResponseTypeId.SlidShow).ToArray();

                var rowIndex = 0;
                foreach (var r in responses)
                {
                    lvImages.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        r.Filename,
                                        r.StreamId.ToString()
                                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }
                _currentImageFiles = responses.Select(x => x.FilePath).ToArray();

                if (_bgwImageThumbnails.IsBusy) return;
                lvImages.SmallImageList?.Images.Clear();
                _bgwImageThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewImages: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewMusic(int residentId)
        {
            try
            {
                lvMusic.Items.Clear();

                var responses = GetMediaResponses(residentId, ResponseTypeId.Radio);
                var rowIndex = 0;
                foreach (var r in responses)
                {
                    lvMusic.Items.Add(new ListViewItem(new[]
                                    {
                                        string.Empty,
                                        string.Empty,
                                        r.Filename,
                                        r.StreamId.ToString()
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
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListView: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewPictures(int residentId)
        {
            try
            {
                lvPictures.Items.Clear();

                var pictures = _resident.PersonalPictures
                    .Where(p => p.ResidentId == residentId)
                    .ToArray();

                var rowIndex = 0;

                foreach (var p in pictures)
                {
                    lvPictures.Items.Add(new ListViewItem(new[]
                    {
                        string.Empty,
                        p.Filename,
                        p.StreamId.ToString()
                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });

                    rowIndex++;
                }

                _currentPictureFiles = pictures.Select(x => x.FilePath).ToArray();

                if (_bgwPictureThumbnails.IsBusy) return;

                lvPictures.SmallImageList?.Images.Clear();
                _bgwPictureThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewPictures: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewInteractiveResponses(int residentId)
        {
            try
            {
                lvInteractiveResponses.Items.Clear();

                var gameDifficulatyLevel = (residentId > 0)
                    ? _resident
                        .Profile.GameDifficultyLevel
                    : _genericProfile.GameDifficultyLevel; 

                var interactiveResponseTypes = (residentId > 0)
                    ? _resident.Profile.ConfigDetails
                        .Where(rt => rt.ResponseType.IsInteractive)
                        .Select(rt => rt.ResponseType)
                        .GroupBy(rt => rt.Id, (key, r) => r.FirstOrDefault())
                        .ToArray()

                    : _genericProfile.ConfigDetails
                        .Where(rt => rt.ResponseType.IsInteractive)
                        .Select(rt => rt.ResponseType)
                        .GroupBy(rt => rt.Id, (key, r) => r.FirstOrDefault())
                        .ToArray();

                var rowIndex = 0;
                foreach (var rt in interactiveResponseTypes)
                {
                    lvInteractiveResponses.Items.Add(new ListViewItem(new[]
                    {
                        string.Empty,
                        gameDifficulatyLevel.ToString(),
                        rt.Description,
                        rt.Id.ToString()
                    })
                    {
                        BackColor = ((rowIndex & 1) == 0) ? Color.AliceBlue : Color.White
                    });
                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewInteractiveResponses: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadTabs(int residentId)
        {
            ShowPicturesTab(residentId > 0);

            LoadListViewImages(residentId);
            LoadListViewMusic(residentId);
            LoadListViewVideos(residentId);
            LoadListViewInteractiveResponses(residentId);

            if (TabPageExists(tabPictures))
                LoadListViewPictures(residentId);
        }

        private void LoadMusicPlaylist(int residentId)
        {
            var music = GetMusicFiles(residentId);
            _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistCaregiver, music);

            axWindowsMediaPlayer1.currentPlaylist = _playlist;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        #endregion

        #region file queries

        // images and videos
        private string GetFilePath(int residentId, Guid streamId)
        {
            string filePath = null;
            try
            {
                filePath = (residentId > 0)
                    ? _resident.Profile.ConfigDetails
                        .SelectMany(r => r.ResponseType.Responses.Where(x => x.StreamId == streamId))
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath).FirstOrDefault()

                    : _genericProfile.ConfigDetails
                        .SelectMany(r => r.ResponseType.Responses.Where(x => x.StreamId == streamId))
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetFilePath: {ex.Message}", EventLogEntryType.Error);
            }

            return filePath;
        }

        private string[] GetFiles(int residentId, int responseTypeId, Guid streamId)
        {
            string[] files = null;
            try
            {
                var selectedFilePath = GetFilePath(residentId, streamId);

                files = (residentId > 0)
                     ? new[] { selectedFilePath }
                     .Union(_resident.Profile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == responseTypeId)
                        .SelectMany(r => r.ResponseType.Responses.Where(x => x.StreamId != streamId)
                        .OrderBy(o => o.Filename))
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath)).ToArray()

                    : new [] { selectedFilePath }
                    .Union(_genericProfile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == responseTypeId)
                        .SelectMany(r => r.ResponseType.Responses.Where(x => x.StreamId != streamId)
                        .OrderBy(o => o.Filename))
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath)).ToArray();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        // music
        private IEnumerable<string> GetMusicFiles(int residentId)
        {
            string[] musicFiles = null;
            try
            {
                musicFiles = (residentId > 0)
                    ? _resident.Profile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == ResponseTypeId.Radio)
                        .SelectMany(r => r.ResponseType.Responses)
                        .OrderBy(o => o.Filename)
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath).ToArray()

                    : _genericProfile.ConfigDetails
                        .Where(cd => cd.ResponseType.Id == ResponseTypeId.Radio)
                        .SelectMany(r => r.ResponseType.Responses)
                        .OrderBy(o => o.Filename)
                        .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                        .Select(f => f.FilePath).ToArray();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetMusicFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return musicFiles;
        }

        // personal pictures
        private string GetPersonalPictureFilePath(Guid streamId)
        {
            string filePath = null;
            try
            {
                filePath = _resident.PersonalPictures
                    .Where(p => p.StreamId == streamId)
                    .Select(f => f.FilePath).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetPersonalPictureFilePath: {ex.Message}", EventLogEntryType.Error);
            }

            return filePath;
        }

        private string[] GetPersonalPictureFiles(int residentId, Guid streamId)
        {
            string[] files = null;
            try
            {
                var selectedFilePath = GetPersonalPictureFilePath(streamId);

                files = new[] { selectedFilePath }
                    .Union(_resident.PersonalPictures
                        .Where(p => p.ResidentId == residentId)
                        .Where(p => p.StreamId != streamId)
                        .OrderBy(o => o.Filename)
                        .Select(f => f.FilePath)).ToArray();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetPersonalPictureFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        // interactive responses
        private string[] GetInteractiveResponseFiles(int residentId, int responseTypeId)
        {
            string[] files = null;
            try
            {
                switch (responseTypeId)
                {
                    case ResponseTypeId.MatchingGame:
                        files = (residentId > 0)
                            ? _resident.Profile.ConfigDetails
                                .Where(cd => cd.ResponseType.Id == responseTypeId)
                                .SelectMany(r => r.ResponseType.Responses)
                                .OrderBy(o => o.Filename)
                                .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                                .Select(f => f.FilePath).ToArray()

                            : _genericProfile.ConfigDetails
                                .Where(cd => cd.ResponseType.Id == responseTypeId)
                                .SelectMany(r => r.ResponseType.Responses)
                                .OrderBy(o => o.Filename)
                                .GroupBy(r => r.StreamId, (key, r) => r.FirstOrDefault())
                                .Select(f => f.FilePath).ToArray();
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetInteractiveResponseFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        #endregion

        #region play media

        private void PlayVideos(int residentId, Guid selectedStreamId)
        {
            try
            {
                var videos = GetFiles(residentId, ResponseTypeId.Television, selectedStreamId);

                var videoPlayer = new VideoPlayer
                {
                    EventLogger = _systemEventLogger,
                    Videos = videos
                };

                StopMusic();
                videoPlayer.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayVideos: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlaySong(int selectedIndex)
        {
            try
            {
                lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = string.Empty;
                lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;
                var media = _playlist.Item[selectedIndex];
                axWindowsMediaPlayer1.Ctlcontrols.playItem(media);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlaySong: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void StopMusic()
        {
            try
            {
                if (axWindowsMediaPlayer1.playState != WMPPlayState.wmppsPlaying &&
                    axWindowsMediaPlayer1.playState != WMPPlayState.wmppsPaused)
                    return;

                lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;
                lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = string.Empty;
                axWindowsMediaPlayer1.Ctlcontrols.stop();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlaySong: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void DisplayImages(int residentId, Guid selectedStreamId)
        {
            try
            {
                var images = GetFiles(residentId, ResponseTypeId.SlidShow, selectedStreamId);

                var imageViewer = new ImageViewer
                {
                    EventLogger = _systemEventLogger,
                    Images = images
                };

                StopMusic();
                imageViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ShowImages: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ShowPersonalPictures(int residentId, Guid selectedStreamId)
        {
            try
            {
                var pictures = GetPersonalPictureFiles(residentId, selectedStreamId);

                var imageViewer = new ImageViewer
                {
                    EventLogger = _systemEventLogger,
                    Images = pictures
                };

                StopMusic();
                imageViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ShowPersonalPictures: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayActivity(int responseId, int responseTypeId, int difficultyLevel, string activityName)
        {
            try
            {
                var files = GetInteractiveResponseFiles(responseId, responseTypeId);

                var activityPlayer = new ActivityPlayer
                {
                    ResidentId = _currentResidentId,
                    SystemEventLogger = _systemEventLogger,
                    OperationsClient = _opsClient,
                    Files = files,
                    DifficultyLevel = difficultyLevel,
                    ActivityName = activityName
            };

                StopMusic();
                activityPlayer.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayActivity: {ex.Message}", EventLogEntryType.Error);
            }
        }

        #endregion

        #region event handlers

        // for the main Display app
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
        private void VideosListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvVideos.SelectedItems[0].SubItems[ListViewMediaColumnStreamId].Text);
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                PlayVideos(residentId, selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.VideosListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void MusicListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedIndex = lvMusic.SelectedIndices[0];

                if (_currentMusicIndex == selectedIndex)
                {
                    if (axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.pause();
                        lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlayActive;
                        lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = "Paused";
                    }
                    else
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                        lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPause;
                        lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = "Playing...";
                    }
                }
                else
                {
                    StopMusic();
                    PlaySong(selectedIndex);
                }

                // remove focus from the selected item in the ListView
                lblMediaSource.Focus();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.MusicListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ImagesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvImages.SelectedItems[0].SubItems[ListViewMediaColumnStreamId].Text);
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                DisplayImages(residentId, selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.ImagesListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PicturesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvPictures.SelectedItems[0].SubItems[ListViewMediaColumnStreamId].Text);
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                ShowPersonalPictures(residentId, selectedStreamId);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PicturesListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void InteractiveResponsesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());
                var difficultyLevel = Convert.ToInt32(lvInteractiveResponses.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnDifficultyLevel].Text);
                var responseTypeId = Convert.ToInt32(lvInteractiveResponses.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnResponseTypeId].Text);
                var activityname = lvInteractiveResponses.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnName].Text;

                PlayActivity(residentId, responseTypeId, difficultyLevel, activityname);
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.InteractiveResponsesListViewClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // media player
        private void PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            try
            {
                switch (e.newState)
                {
                    case (int)WMPPlayState.wmppsPlaying:
                        _currentMusicIndex = axWindowsMediaPlayer1.CurrentIndex(_playlist);
                        lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = "Playing...";
                        lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPause;
                        lvMusic.Items[_currentMusicIndex].Selected = true;
                        break;
                    case (int)WMPPlayState.wmppsMediaEnded:
                        lvMusic.Items[_currentMusicIndex].SubItems[ListViewMusicColumnStatus].Text = string.Empty;
                        lvMusic.Items[_currentMusicIndex].ImageIndex = ImageIndexPlay;
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayStateChange: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // configuration/management
        private void VolumeAdjusterButtonClick(object sender, EventArgs e)
        {
            try
            {
                var frmVolumeAdjuster = new VolumeAdjuster
                {
                    EventLogger = _systemEventLogger,
                    IsMusicPlaying = axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying
                };
                frmVolumeAdjuster.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.VolumeAdjusterButtonClick: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void ResidentSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                if (residentId == _currentResidentId) return;

                if (residentId > 0) LoadResident(residentId);

                CancelBackgroundWorkers();
                StopMusic();
                LoadTabs(residentId);
                LoadMusicPlaylist(residentId);

                _currentResidentId = residentId;
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
            StopMusic();
            RaiseCaregiverCompleteEvent();
        }

        // disable column resizing for all list views
        private void ImagesListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvImages.Columns[e.ColumnIndex].Width;
        }

        private void MusicListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvMusic.Columns[e.ColumnIndex].Width;
        }

        private void VideosListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvVideos.Columns[e.ColumnIndex].Width;
        }

        private void InteractiveResponsesListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvInteractiveResponses.Columns[e.ColumnIndex].Width;
        }

        private void PicturesListViewColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lvPictures.Columns[e.ColumnIndex].Width;
        }

        #endregion

        #region helper

        private void ShowPicturesTab(bool show)
        {
            if (show)
            {
                if (!TabPageExists(tabPictures))
                    tbMedia.TabPages.Add(tabPictures);
            }
            else
                tbMedia.TabPages.Remove(tabPictures);
        }

        private bool TabPageExists(TabPage tabPage)
        {
            return tbMedia.TabPages.Cast<TabPage>().Contains(tabPage);
        }

        #endregion

        #region background workers (for thumbnails)

        // image thumbnails
        private void UpdateImagesListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_bgwImageThumbnails.CancellationPending) return;

                _imageListImages.Images.Add((Image) e.UserState);
                lvImages.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdatImagesListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesListViewThumbnails(object sender, DoWorkEventArgs e)
        {
            try
            {
                var rowIndex = 0;
                var files = _currentImageFiles;

                foreach (var file in files)
                {
                    if (_bgwImageThumbnails.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var thumbnail = Thumbnail.Picture.Get(file, ThumbnailDimensions);
                    _bgwImageThumbnails.ReportProgress(rowIndex, thumbnail);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesListViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadImagesListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvImages.SmallImageList?.Images.Clear();
                _bgwImageThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadImagesListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // picture thumbnails
        private void UpdatePicturesListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_bgwPictureThumbnails.CancellationPending) return;

                _imageListPictures.Images.Add((Image)e.UserState);
                lvPictures.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdatPicturesListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadPicturesListViewThumbnails(object sender, DoWorkEventArgs e)
        {
            var rowIndex = 0;
            var files = _currentPictureFiles;

            foreach (var file in files)
            {
                if (_bgwPictureThumbnails.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                var thumbnail = Thumbnail.Picture.Get(file, ThumbnailDimensions);
                _bgwPictureThumbnails.ReportProgress(rowIndex, thumbnail);

                rowIndex++;
            }
        }

        private void LoadPicturesListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvPictures.SmallImageList?.Images.Clear();
                _bgwImageThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadPicturesListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // video thumbnails
        private void UpdateVideosListViewImage(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                if (_bgwVideoThumbnails.CancellationPending) return;

                _imageListVideos.Images.Add((Image)e.UserState);
                lvVideos.Items[e.ProgressPercentage].ImageIndex = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.UpdatVideosListViewImage: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadVideosListViewThumbnails(object sender, DoWorkEventArgs e)
        {
            try
            {
                var rowIndex = 0;
                var files = _currentVideoFiles;

                foreach (var file in files)
                {
                    if (_bgwVideoThumbnails.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    var thumbnail = Thumbnail.Video.Get(file, 2, 5);
                    _bgwVideoThumbnails.ReportProgress(rowIndex, thumbnail);

                    rowIndex++;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadVideosListViewThumbnails: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadVideosListViewThumbnailsCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (!e.Cancelled || _formIsClosing) return;

                lvVideos.SmallImageList?.Images.Clear();
                _bgwVideoThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.LoadVideosListViewThumbnailsCompleted: {ex.Message}", EventLogEntryType.Error);

            }
        }

        // cancel workers
        private void CancelBackgroundWorkers(bool dispose = false)
        {
            if (_bgwImageThumbnails != null)
            {
                if (_bgwImageThumbnails.IsBusy)
                {
                    _bgwImageThumbnails.CancelAsync();
                }
            }

            if (_bgwPictureThumbnails != null)
            {
                if (_bgwPictureThumbnails.IsBusy)
                {
                    _bgwPictureThumbnails.CancelAsync();
                }
            }

            if (_bgwVideoThumbnails != null)
            {
                if (_bgwVideoThumbnails.IsBusy)
                {
                    _bgwVideoThumbnails.CancelAsync();
                }
            }

            if (!dispose) return;
            _bgwImageThumbnails?.Dispose();
            _bgwPictureThumbnails?.Dispose();
            _bgwVideoThumbnails?.Dispose();
        }

        #endregion
    }
}
