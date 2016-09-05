namespace Keebee.AAT.Constants
{
    public enum RotationSensorStep
    {
        Value1 = 200,
        Value2 = 400, 
        Value3 = 600,
        Value4 = 800,
        Value5 = 999
    }

    public static class ActivityLog
    {
        public const string Path = @"sqlexpress\KeebeeAATFilestream\Media\ActivityLog";
    }

    //TODO: add an asociatian to this file in the database somehow
    public static class MediaPath
    {
        public const string CatsVideo = @"sqlexpress\KeebeeAATFilestream\Media\Cats\Cats.mp4";
    }

    public static class UserProfile
    {
        public const int Generic = 1;
    }

    public static class UserResponse
    {
        public const int Ambient = 0;
    }

    public static class UserResponseTypeCategory
    {
        public const int Image = 1;
        public const int Music = 2;
        public const int Video = 3;
        public const int Game = 4;
        public const int System = 6;
    }

    public static class UserResponseType
    {
        public const int Ambient = 0;
        public const int SlidShow = 1;
        public const int MatchingGame = 2;
        public const int Cats = 3;
        public const int Radio = 5;
        public const int Television = 6;
    }

    public static class SystemResponseType
    {
        public const int Ambient = -1;
        public const int Caregiver = -2;
        public const int KillDisplay = -3;
    }

    public static class UserEventLogEntryType
    {
        public const int SensorActivated = 1;
        public const int PicturesMatched = 2;
        public const int PairsMatched = 3;
    }

    public class ResponseMessage
    {
        public int ResidentId { get; set; }
        public int ProfileDetailId { get; set; }
        public int GameDifficultyLevel { get; set; }
        public int ActivityTypeId { get; set; }
        public int ResponseTypeId { get; set; }
        public int ResponseValue { get; set; }
    }

    public class DisplayMessage
    {
        public bool IsActive { get; set; }
    }

    public class RfidMonitorMessage
    {
        public bool IsFinal { get; set; }
        public int ReadCount { get; set; }
        public int ResidentId { get; set; }
    }
}
