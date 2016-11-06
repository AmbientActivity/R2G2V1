using Keebee.AAT.RESTClient;
using Keebee.AAT.SystemEventLogging;
using Keebee.AAT.Shared;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.Display.Caregiver.CustomControls;
using Keebee.AAT.Display.Helpers;
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

        private OperationsClient _opsClient;
        public OperationsClient OperationsClient
        {
            set { _opsClient = value; }
        }

        private IEnumerable<MediaResponseType> _publicMediaFiles;
        public IEnumerable<MediaResponseType> PublicMediaFiles
        {
            set { _publicMediaFiles = value; }
        }

        private Config _config;
        public Config Config
        {
            set { _config = value; }
        }

        private readonly Resident _publicLibrary = new Resident
        {
            Id = PublicMediaSource.Id,
            FirstName = PublicMediaSource.Description,
            LastName = string.Empty,
            GameDifficultyLevel = 1
        };

        #region local variables

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

        private IEnumerable<MediaResponseType> _mediaFiles;

        // media path
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        // playlist
        private const string PlaylistCaregiver = PlaylistName.Caregiver;
        private IWMPPlaylist _playlist;
        private int _totalSongs;

        // current values
        private Resident _currentResident;
        private int _currentMusicIndex;
        private string[] _currentImageFiles;
        private string[] _currentPictureFiles;
        private string[] _currentVideoFiles;

        // image indices
        private const int ImageIndexPlay = 0;
        private const int ImageIndexPlayActive = 1;
        private const int ImageIndexPause = 2;

        // list view column indices
        private const int ListViewMusicColumnStatus = 1;
        private const int ListViewMusicColumnName = 2;
        private const int ListViewMediaColumnStreamId = 2;

        private const int ListViewIActivitiesColumnDifficultyLevel = 1;
        private const int ListViewIActivitiesColumnName = 2;
        private const int ListViewIActivitiesColumnId = 3;

        //TODO: find a way to compute these values dynamically
#if DEBUG
        private const int ThumbnailDimensions = 16;
        private const int ListViewMusicColWidthStatus = 70;
        private const int ListViewMusicColWidthName = 507;
        private const int ListViewMediaColWidthName = 577;
        private const int ListViewInteractiveResponseColWidthName = 593;

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
        private const int ListViewMusicColWidthStatus = 150;
        private const int ListViewMusicColWidthName = 1659;
        private const int ListViewMediaColWidthName = 1873;
        private const int ListViewInteractiveResponseColWidthName = 1873;

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

        public CaregiverInterface()
        {
            InitializeComponent();
#if DEBUG
            _imageListMusic = imageListMusicDebug;
#elif !DEBUG
            _imageListMusic = imageListMusic;
#endif
            ConfigureControls();
            ConfigureBackgroundWorkers();
            InitializeStartupPosition();
            _currentResident = _publicLibrary;
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
            axWindowsMediaPlayer1.settings.volume = MediaPlayerControl.DefaultVolume;
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

        private void LoadResidentMedia(int residentId)
        {
            _currentResident = residentId == PublicMediaSource.Id
                ? _publicLibrary
                : _opsClient.GetResident(residentId);

            if (residentId == PublicMediaSource.Id)
            {
                _mediaFiles = _publicMediaFiles;
            }
            else
            {
                var media = _opsClient.GetResidentMediaFilesForResident(_currentResident.Id);

                _mediaFiles = media != null 
                    ? _opsClient.GetResidentMediaFilesForResident(_currentResident.Id).MediaFiles 
                    : new List<MediaResponseType>();
            }
        }

        private void LoadResidentDropDown()
        {
            try
            {
                var residents = _opsClient.GetResidents().ToList();
                var arrayList = new ArrayList();

                var residentList = new List<Resident> { _publicLibrary }
                    .Union(residents
                    .OrderBy(o => o.LastName).ThenBy(o => o.FirstName))
                    .ToArray();

                foreach (var r in residentList)
                {
                    var name = (r.LastName.Length > 0)
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

        private void LoadListViewVideos()
        {
            try
            {
                lvVideos.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.Videos, ResponseTypeId.Television);

                var rowIndex = 0;
                foreach (var f in files)
                {
                    lvVideos.Items.Add(new ListViewItem(new[]
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

                _currentVideoFiles = GetFilePaths(MediaPathTypeId.Videos, ResponseTypeId.Television);

                if (_bgwVideoThumbnails.IsBusy) return;
                lvVideos.SmallImageList?.Images.Clear();
                _bgwVideoThumbnails.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListViewVideos: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewImages()
        {
            try
            {
                lvImages.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.Images, ResponseTypeId.SlideShow).ToArray();
                
                var rowIndex = 0;
                foreach (var f in files)
                {
                    lvImages.Items.Add(new ListViewItem(new[]
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

                _currentImageFiles = GetFilePaths(MediaPathTypeId.Images, ResponseTypeId.SlideShow);

                if (_bgwImageThumbnails.IsBusy) return;
                lvImages.SmallImageList?.Images.Clear();
                _bgwImageThumbnails.RunWorkerAsync();
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
                _systemEventLogger?.WriteEntry($"Caregiver.LoadListView: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void LoadListViewPictures()
        {
            try
            {
                lvPictures.Items.Clear();

                var files = GetMediaFiles(MediaPathTypeId.Pictures);

                var rowIndex = 0;

                foreach (var f in files)
                {
                    lvPictures.Items.Add(new ListViewItem(new[]
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

                _currentPictureFiles = GetFilePaths(MediaPathTypeId.Pictures);

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
                    ? _currentResident.GameDifficultyLevel : 1;

                var interactiveResponseTypes = _config.ConfigDetails
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

            LoadListViewImages();
            LoadListViewMusic();
            LoadListViewVideos();
            LoadListViewInteractiveResponses(residentId);

            if (TabPageExists(tabPictures))
                LoadListViewPictures();
        }

        private void LoadMusicPlaylist()
        {
            var music = GetFilePaths(MediaPathTypeId.Music, ResponseTypeId.Radio);
            _playlist = axWindowsMediaPlayer1.LoadPlaylist(PlaylistCaregiver, music);

            axWindowsMediaPlayer1.currentPlaylist = _playlist;
            axWindowsMediaPlayer1.Ctlcontrols.stop();
        }

        #endregion

        #region file queries

        // get filenames with full path for (for usage)
        private string[] GetFilePaths(int mediaPathTypeId, int? responseTypeId = null, Guid? streamId = null)
        {
            string[] files = null;
            try
            {
                var isPublic = _currentResident.Id == PublicMediaSource.Id;
                var pathRoot = $@"{_mediaPath.ProfileRoot}\{_currentResident.Id}";
                var mediaFiles = _mediaFiles.ToArray();

                var paths = mediaFiles
                    .Where(x => x.ResponseType.Id == responseTypeId || responseTypeId == null)
                    .SelectMany(x => x.Paths)
                    .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                    .ToArray();

                if (!paths.Any()) return new string[0];

                // get the path type
                var mediaPath = paths.Single(x => x.MediaPathType.Id == mediaPathTypeId);

                // get the description
                var mediaPathType = mediaPath.MediaPathType.Description;

                if (streamId != null)
                {
                    // organize the files so that the selected appears first in the list
                    var selectedFile = mediaPath.Files
                        .Single(f => f.StreamId == streamId).Filename;

                    var filesAfterSelected = new[] {$@"{pathRoot}\{mediaPathType}\{selectedFile}"}
                                .Union(mediaPath.Files
                                .Where(f => f.IsPublic == false || isPublic)
                                .OrderBy(f => f.Filename)
                                .SkipWhile(f => f.Filename != selectedFile)
                                .Select(f => $@"{pathRoot}\{mediaPathType}\{f.Filename}"))
                                .ToArray();

                    var filesBeforeSelected = mediaPath.Files
                            .Where(f => f.IsPublic == false || isPublic)
                            .OrderBy(f => f.Filename)
                            .Select(f => $@"{pathRoot}\{mediaPathType}\{f.Filename}")
                            .Except(filesAfterSelected)
                            .ToArray();

                    files = filesAfterSelected.Concat(filesBeforeSelected).ToArray();
                }
                else
                {
                    files = mediaPath.Files
                        .Where(f => f.IsPublic == false || isPublic)
                        .OrderBy(f => f.Filename)
                        .Select(f => $@"{pathRoot}\{mediaPathType}\{f.Filename}").ToArray();
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetFilePaths: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        // get files with no extensions or path (for display)
        private IEnumerable<MediaFile> GetMediaFiles(int mediaPathTypeId, int? responseTypeId = null)
        {
            IEnumerable<MediaFile> files = null;
            try
            {
                var mediaFiles = _mediaFiles.ToArray();
                var isPublic = _currentResident.Id == PublicMediaSource.Id;

                var paths = mediaFiles
                    .Where(x => x.ResponseType.Id == responseTypeId || responseTypeId == null)
                    .SelectMany(x => x.Paths)
                    .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                    .ToArray();

                if (!paths.Any())
                {
                    if (responseTypeId == ResponseTypeId.MatchingGame)
                    {
                        paths = _publicMediaFiles
                            .Where(x => x.ResponseType.Id == responseTypeId)
                            .SelectMany(x => x.Paths)
                            .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                            .ToArray();
                    }
                    else return new List<MediaFile>();
                }

                files = paths
                    .Single(x => x.MediaPathType.Id == mediaPathTypeId).Files
                    .Where(f => f.IsPublic == false || isPublic)
                    .Select(f => new MediaFile
                                 {
                                     StreamId = f.StreamId,
                                     Filename = f.Filename.Replace($".{f.FileType}", string.Empty)
                                 })
                    .OrderBy(f => f.Filename);
            }
            catch (Exception ex)
            {
                _systemEventLogger?.WriteEntry($"Caregiver.GetMediaFiles: {ex.Message}", EventLogEntryType.Error);
            }

            return files;
        }

        #endregion

        #region play media

        private void PlayVideos(Guid selectedStreamId)
        {
            try
            {
                var videos = GetFilePaths(MediaPathTypeId.Videos, ResponseTypeId.Television, selectedStreamId);

                if (File.Exists(videos[0]))
                {
                    var videoPlayer = new VideoPlayer
                    {
                        EventLogger = _systemEventLogger,
                        Videos = videos
                    };

                    StopMusic();
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
                _systemEventLogger.WriteEntry($"Caregiver.ShowImages: {ex.Message}", EventLogEntryType.Error);
            }
        }

        private void PlayMatchingGame(int difficultyLevel, string activityName)
        {
            try
            {
                var shapes = GetFilePaths(MediaPathTypeId.Shapes, ResponseTypeId.MatchingGame);
                var sounds = GetFilePaths(MediaPathTypeId.Sounds, ResponseTypeId.MatchingGame);

                // ensure there are enough shapes and sounds to play the game
                var gameSetup = new MatchingGameSetup { OperationsClient = _opsClient };
                var totalShapes = gameSetup.GetTotalShapes(shapes);
                var totalSounds = gameSetup.GetTotalSounds(sounds);

                var matchingGamePlayer = new MatchingGamePlayer
                {
                    ResidentId = _currentResident.Id,
                    SystemEventLogger = _systemEventLogger,
                    OperationsClient = _opsClient,
                    Shapes = totalShapes,
                    Sounds = totalSounds,
                    DifficultyLevel = difficultyLevel,
                    ActivityName = activityName,
                    IsActiveEventLog = _config.IsActiveEventLog
                };

                StopMusic();
                matchingGamePlayer.ShowDialog();
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayMatchingGame: {ex.Message}", EventLogEntryType.Error);
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

                PlayVideos(selectedStreamId);
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
                if (File.Exists(_playlist.Item[selectedIndex].sourceURL))
                {
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

        private void ImagesListViewClick(object sender, EventArgs e)
        {
            try
            {
                var selectedStreamId = new Guid(lvImages.SelectedItems[0].SubItems[ListViewMediaColumnStreamId].Text);

                DisplayImages(MediaPathTypeId.Images, selectedStreamId, ResponseTypeId.SlideShow);
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

                DisplayImages(MediaPathTypeId.Pictures, selectedStreamId);
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
                var difficultyLevel = Convert.ToInt32(lvInteractiveResponses.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnDifficultyLevel].Text);

                var activityname = lvInteractiveResponses.SelectedItems[0]
                    .SubItems[ListViewIActivitiesColumnName].Text;

                var activitid = Convert.ToInt32(lvInteractiveResponses.SelectedItems[0].SubItems[ListViewIActivitiesColumnId].Text);

                switch (activitid)
                {
                    case ResponseTypeId.MatchingGame:
                        PlayMatchingGame(difficultyLevel, activityname);
                        break;
                }
                
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

                    case (int)WMPPlayState.wmppsTransitioning:
                        if (axWindowsMediaPlayer1.currentMedia != null)
                        {
                            if (!File.Exists(axWindowsMediaPlayer1.currentMedia.sourceURL))
                            {
                                // song was not found - get the name of it
                                var name = lvMusic.Items[_totalSongs - 1].SubItems[ListViewMusicColumnName].Text;

                                // if it is not the last song then go to the next one
                                if (axWindowsMediaPlayer1.currentMedia.name != name)
                                    axWindowsMediaPlayer1.Ctlcontrols.next();
                                else
                                    // otherwise stop the player
                                    axWindowsMediaPlayer1.Ctlcontrols.stop();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _systemEventLogger.WriteEntry($"Caregiver.PlayStateChange: {ex.Message}", EventLogEntryType.Error);
            }
        }

        // configuration/management
        private void ResidentSelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var residentId = Convert.ToInt32(cboResident.SelectedValue.ToString());

                LoadResidentMedia(residentId);

                CancelBackgroundWorkers();
                StopMusic();
                LoadTabs(residentId);
                LoadMusicPlaylist();
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
            LoadTabs(_currentResident.Id);
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
            axWindowsMediaPlayer1.ClearPlaylist(PlaylistCaregiver);
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
