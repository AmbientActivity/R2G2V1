using System;

namespace Keebee.AAT.Shared
{
    public static class AppSettings
    {
        public const string Namespace = "Keebee.AAT";
        public const string DisplayAppName = "Display";
    }

    public static class UserDefaultPassword
    {
        public static string Administrator = "@dmin";
        public static string Caregiver = "1234";
    }
    public static class UserId
    {
        public static int Administrator = 1;
        public static int Caregiver = 2;
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
        public const string MatchingGameThumbnail = "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQNDAsLDBkSEw8UHRofHh0aHBwgJC4nICIsIxwcKDcpLDAxNDQ0Hyc5PTgyPC4zNDL/2wBDAQkJCQwLDBgNDRgyIRwhMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjIyMjL/wAARCABgAGADASIAAhEBAxEB/8QAHwAAAQUBAQEBAQEAAAAAAAAAAAECAwQFBgcICQoL/8QAtRAAAgEDAwIEAwUFBAQAAAF9AQIDAAQRBRIhMUEGE1FhByJxFDKBkaEII0KxwRVS0fAkM2JyggkKFhcYGRolJicoKSo0NTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqDhIWGh4iJipKTlJWWl5iZmqKjpKWmp6ipqrKztLW2t7i5usLDxMXGx8jJytLT1NXW19jZ2uHi4+Tl5ufo6erx8vP09fb3+Pn6/8QAHwEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoL/8QAtREAAgECBAQDBAcFBAQAAQJ3AAECAxEEBSExBhJBUQdhcRMiMoEIFEKRobHBCSMzUvAVYnLRChYkNOEl8RcYGRomJygpKjU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6goOEhYaHiImKkpOUlZaXmJmaoqOkpaanqKmqsrO0tba3uLm6wsPExcbHyMnK0tPU1dbX2Nna4uPk5ebn6Onq8vP09fb3+Pn6/9oADAMBAAIRAxEAPwD0rStK05tHsmbT7Uk28ZJMK/3R7Vc/sjTf+gfaf9+V/wAKNI/5Atj/ANe8f/oIq5X4pUqT53q9z0ktCn/ZGm/9A+0/78r/AIUf2Rpv/QPtP+/K/wCFXKKj2k+7CyKf9kab/wBA+0/78r/hR/ZGm/8AQPtP+/K/4VcqOdzHbyOvVVJH5U1Um3a7CyK/9kab/wBA+0/78r/hR/ZGm/8AQPtP+/K/4VYt3aS2ikbG5kDHHuKkoc5p2uwsin/ZGm/9A+0/78r/AIUf2Rpv/QPtP+/K/wCFXKKXtJ92FkU/7I03/oH2n/flf8KP7I03/oH2n/flf8KuUUe0n3YWRzkd9qtpY6eLTThdWoskZiGw2/bwo+vHP1qzDqGryXESPpqomXWQlunGVI9s8frV7SP+QLY/9e8f/oIq5XTOvBNrkV9ddf8AMSXmYMOrawqSG60ghVjkkDo/XCghcYzknI/D8KP7W1hYy50dzj5im7kjDHA7dQBz61f1O+ex+x7FVvPukhbd2Bzkj34q/TlVgkpOmtfX/MLPuULG7vbieRLmwNuioGV/M3biSRjp2AB/GrV1/wAek3/XNv5VLUV1/wAek3/XNv5VzuSlUTSsPoMtSV0+EhSxESkAd+K801jx5LDq14lk86xTCNSkylHtpFbD8ehUdv8AGvTbP/jxt/8Armv8q4jU/AV5rGrNqGo38MwO7MMMXlfKB8q5ySTnqT/9avWyqeEhWm8Vt0++/by7o566qOK9mdfpOq2+s2X2y0WT7OXKo8ibd4HG4D0z/Kr1Q2cCW1lBBGHCRxhVDnLAAdz61NXkVeTnfIrLodCvbU4Dx54kngeTRYYjHvVXebeDuU54A6jkfpW14P8AEU2v2couIdstvtV5Aww5I647dKi8XeGBrRju0mlE0SiNY44wd2WHJPXjP5Vp+HtCj8P2D2sc7TbpC5ZlAP04/rXs1quCeWxhFfvPnvpd/NfL7jtnOh9XUUveLekf8gWx/wCveP8A9BFZmpXl3o2rLdFpJ9PucK8XUxOB1T6gZx3wccnnQ02WOHQrF5ZFRRbx5ZjgfdFUtU1bRrqzmtZL1X3DgwKZCpHII2g8g8159FSdZ+7dO6ehxwV2ip4kvI5oNOnt5BJHuM6spyCFx/jV3Wtb+w/6LZqJr58BV6hM8DPuew/pzXGRWYvId9xLvVnZ4xH8qjOBkA884B5rR0Se1s9Uml1O5LOjGSM+UzElursQMZ6gfj7V6MsLTjFL4uS+lt9f6udUqDjBSa0/Pt6HZ6fby2lhDDPcPcTKP3krnJZjyfwyeB2GKkuv+PSb/rm38qhttUsLsgW95BIx/hWQZ/LrU11/x6Tf9c2/lXjtT9peas2zkEs/+PG3/wCua/yqaqLNcJoitaIHuFgUop/iOBxWPfePND05o0unuY5nPzRNbsGj92BHT6Zq44erVk/Zq+r2HGLlojpqKht7q3ulZreZJVUgEo2QCQD/ACIP41NXO007MAorE1zUHtUeIYdZl2psPzI49fYjv7e9XdNvTfxtPlVjY/u0/iCju3ufTtWzw81TVTp/X9fIfK7XIrSytb/w9YwXlvFPF5EZ2SIGGdowee9UtQ0HSbWylnJuLeONSSIpm59gDkZPTpWppH/IFsf+veP/ANBFZWqx3Ws6ounQB4rW2w885XguRkKuepA59ASD2xXRRlP2rXNaKu2EG097HKW93HbQ+TOpg8tzGgJLA9wAQOTgir+iW1tqepSrdi5gdsrFtk27wnVSMZBByevr6Vo+I9OgtrbT7eCJRHl4gCMklsc5PUnHWr2taJJLJ9v03CXqEOU6CQjofZu2e44PqPRliaUopr3XO+vaz/X5/qdUq7lBRbsun6XLkHh/SredJ1s42mjOUklJkZT6gtnB+lXbr/j0m/65t/KotPvBf2ENz5bxFx80cikMjA4IIPoQaluv+PSb/rm38q8aUqjqWqO7T6nI3fUSz/48bf8A65r/ACrK8VaImu6FPbiCOS5Ub7cucYce/bPT8a1bP/jxt/8Armv8qmpRqypVvaR3TCLa1Rk+HtNtNN0uNLWwNkXAMsbDBLAYJOCRk465rWooqKlSVSbnLdg3c5/W9MEu+4hhjg8pd5lVRukY9vp6/hV3SdNWwV42gjLocLcBRukU88+/rWmQCMEZoraWJm6fs+n9f8H7yud8vL0Kekf8gWx/694//QRVyqekf8gWx/694/8A0EVcrGp8b9SFsZ2r2Ut79h8oKfJu0lfJx8ozmtGiiiU24qL6DCorr/j0m/65t/Kpaiuv+PSb/rm38qUPiQiKGVINLjmlbbHHCGZvQBcmuBPxh0zzXC6XeNEG+Rwy5YeuM8fSu0vYZ7nwtPBbIrzy2bJGrNgFimBzXmEPwt1GO1hdlD3EUIkkjMi7ZX8wfux6DYDye7D3r2svo4KanLFPW9lrYcbdT1XSNXsdc09L7T5vNgYkZwQQR1BB6Gr1ZWg6HZ6FbzpYxywxXEvnmB2BETEAFRjoOPU1q15FdU1UapfD0vuBzdz4qXTdYuba/jVLaMjbKmSQCAeR+Pap9F16XWNQuEWAR20aAqT9/JPft68Vm+INJn1LxJBFHlElh5mCZC4Jzn17dfWrfg+1ntrW886IxkzbcFcHIHP1FejUp4dYbnS96y/r8/6375woKhzr4rL89/w26GxpH/IFsf8Ar3j/APQRVyqek/8AIGsf+veP/wBBFXK8yp8b9Tz1sFFFFQMKjuFL20qqMsyEAfhUlFNOzuBFbI0drCjDDKigj3xUtFFDd3cAooopAFFFFAH/2Q==";
        public const string PaintingActivityThumbnail = "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";
        public const string BalloonPoppingGameThumbnail = "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";
    }
}
