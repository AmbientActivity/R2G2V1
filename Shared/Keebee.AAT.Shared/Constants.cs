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
    }
}
