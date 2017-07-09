using System;

namespace Keebee.AAT.Shared
{
    public static class AppSettings
    {
        public const string Namespace = "Keebee.AAT";
        public const string DisplayAppName = "Display";
    }

    public static class VideoCaptures
    {
        public const string Path = @"C:\VideoCaptures";
    }

    public class MediaSourcePath
    {
        private const string SqlServerFilestream = @"sqlexpress\KeebeeAATFilestream\Media";

        public string MediaRoot = $@"\\{Environment.MachineName}\{SqlServerFilestream}";
        public string ProfileRoot = $@"\\{Environment.MachineName}\{SqlServerFilestream}\Profiles";
        public string SharedLibrary = @"SharedLibrary";
        public string ExportEventLogRoot = $@"\\{Environment.MachineName}\{SqlServerFilestream}\Exports\EventLog";
    }

    public static class ServiceName
    {
        public const string StateMachine = "StateMachineService";
        public const string Phidget = "PhidgetService";
        public const string BluetoothBeaconWatcher = "BluetoothBeaconWatcherService";
        public const string VideoCapture = "VideoCaptureService";
        public const string KeepIISAlive = "KeepIISAliveService";
    }

    public enum RotationSensorStep
    {
        Value1 = 200,
        Value2 = 400, 
        Value3 = 600,
        Value4 = 800,
        Value5 = 999
    }

    public static class ScreenSize
    {
        public const int Width = 1920;
        public const int Height = 1080;
    }

    public static class Exports
    {
        public const string EventLogPath = @"Exports\EventLog";
    }

    public static class MediaPathTypeId
    {
        public const int Music = 1;
        public const int RadioShows = 2;
        public const int ImagesGeneral = 3;
        public const int ImagesPersonal = 4;
        public const int TVShows = 5;
        public const int HomeMovies = 6;
        public const int MatchingGameShapes = 7;
        public const int MatchingGameSounds = 8;
        public const int Ambient = 9;
        public const int Cats = 10;
    }

    public static class MediaPathTypeCategoryId
    {
        public const int Audio = 1;
        public const int Image = 2;
        public const int Video = 3;
    }

    public static class MediaPathTypeCategoryDescription
    {
        public const string Audio = "Audio";
        public const string Image = "Image";
        public const string Video = "Video";
    }

    public static class SystemMediaPathType
    {
        public const string AmbientDescription = "System (Ambient)";
        public const string CatsDescription = "System (Cats)";
        public const string AmbientShortDescription = "Videos";
        public const string CatsShortDescription = "Videos";
    }
    public static class MediaSourceType
    {
        public const string Personal = "Personal Content";
        public const string Public = "Public Profile";
    }

    public static class MediaSourceTypeId
    {
        public const int Public = 0;
        public const int Personal = 1;
    }

    public static class ConfigId
    {
        public const int Default = 1;
    }

    public static class PublicProfileSource
    {
        public const int Id = 0;
        public const string Name = "Public";
        public const string Description = "Public Profile";
        public const string DescriptionSystem = "System Profile";
        public const int GameDifficultyLevel = 1;
    }

    public static class InteractiveActivityTypeId
    {
        public const int MatchingGame = 1;
        public const int PaintingActivity = 2;
        public const int BalloonPoppingGame= 3;
    }

    public static class PhidgetStyleTypeIdId
    {
        public const int Touch = 1;
        public const int MultiTurn = 2;
        public const int StopTurn = 3;
        public const int Slider = 4;
        public const int OnOff = 5;
    }

    public static class PhidgetTypeId
    {
        public const int Sensor0 = 1;
        public const int Sensor1 = 2;
        public const int Sensor2 = 3;
        public const int Sensor3 = 4;
        public const int Sensor4 = 5;
        public const int Sensor5 = 6;
        public const int Sensor6 = 7;
        public const int Sensor7 = 8;

        public const int Input0 = 9;
        public const int Input1 = 10;
        public const int Input2 = 11;
        public const int Input3 = 12;
        public const int Input4 = 13;
        public const int Input5 = 14;
        public const int Input6 = 15;
        public const int Input7 = 16;
    }

    public static class ResponseTypeCategoryId
    {
        public const int Image = 1;
        public const int Music = 2;
        public const int Video = 3;
        public const int Game = 4;
        public const int System = 6;
    }

    public static class ResponseTypeId
    {
        public const int SlideShow = 1;
        public const int MatchingGame = 2;
        public const int Cats = 3;
        public const int KillDisplay = 4;
        public const int Radio = 5;
        public const int Television = 6;
        public const int Caregiver = 7;
        public const int Ambient = 8;
        public const int OffScreen = 9;
        public const int VolumeControl = 10;
        public const int PaintingActivity = 11;
        public const int BalloonPoppingGame = 12;
    }

    public static class MediaPlayerControl
    {
        public static int DefaultVolume = 70;
    }

    public static class ImagesBase64
    {
        public const string ProfilePicturePlaceholder = "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";
        public const string MatchingGameThumbnail = "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";
        public const string PaintingActivityThumbnail = "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";
        public const string BalloonPoppingGameThumbnail = "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";
    }
}
