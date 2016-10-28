using System;

namespace Keebee.AAT.Shared
{
    public static class ApplicationName
    {
        public const string DisplayApp = "Keebee.AAT.Display";
    }

    public static class KeepAliveUrl
    {
        public const string OperationsApi = "http://localhost/Keebee.AAT.Operations/api/status";
        public const string AdministratorHome = "http://localhost/Keebee.AAT.Administrator";
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
        public string ExportEventLogRoot = $@"\\{Environment.MachineName}\{SqlServerFilestream}\Exports\EventLog";
    }

    public static class ServiceName
    {
        public const string StateMachine = "StateMachineService";
        public const string Phidget = "PhidgetService";
        public const string RfidReader = "RfidReaderService";
        public const string VideoCapture = "VideoCaptureService";
        public const string KeepIISAlive = "KeepIISAliveService";

        public const string StateMachineExe = "Keebee.AAT.StateMachineService.exe";
        public const string PhidgetExe = "Keebee.AAT.PhidgetService.exe";
        public const string RfidReaderExe = "Keebee.AAT.RfidReaderService.exe";
        public const string VideoCaptureExe = "Keebee.AAT.VideoCaptureService.exe";
        public const string KeepIISAliveExe = "Keebee.AAT.KeepIISAliveService.exe";
    }

    public static class PlaylistName
    {
        public const string Ambient = "ambient";
        public const string Profile = "profile";
        public const string Caregiver = "caregiver";
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
        public const int Images = 1;
        public const int Videos = 2;
        public const int Music = 3;
        public const int Pictures = 4;
        public const int Shapes = 5;
        public const int Sounds = 6;
    }

    public static class MediaSourceType
    {
        public const string Personal = "Personal Content";
        public const string Public = "Public Library";
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

    public static class PublicMediaSource
    {
        public const int Id = 0;
        public const string Description = "Public Library";
    }

    public static class GameTypeId
    {
        public const int MatchThePictures = 1;
        public const int MatchThePairs = 2;
    }

    public static class PhidgetStyleTypeIdId
    {
        public const int Touch = 1;
        public const int MultiTurn = 2;
        public const int StopTurn = 3;
        public const int Slider = 4;
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
        public const int SlidShow = 1;
        public const int MatchingGame = 2;
        public const int Cats = 3;
        public const int KillDisplay = 4;
        public const int Radio = 5;
        public const int Television = 6;
        public const int Caregiver = 7;
        public const int Ambient = 8;
        public const int OffScreen = 9;
        public const int VolumeControl = 10;
    }
}
