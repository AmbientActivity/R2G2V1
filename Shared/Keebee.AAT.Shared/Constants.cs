namespace Keebee.AAT.Shared
{
    public enum RotationSensorStep
    {
        Value1 = 200,
        Value2 = 400, 
        Value3 = 600,
        Value4 = 800,
        Value5 = 999
    }

    public static class CustomEventLog
    {
        public const string Path = @"sqlexpress\KeebeeAATFilestream\Media\Exports\EventLog";
    }

    //TODO: add an asociatian to this file in the database somehow
    public static class MediaPath
    {
        public const string ProfileRoot = @"sqlexpress\KeebeeAATFilestream\Media\Profiles";
        public const string CatsVideo = @"sqlexpress\KeebeeAATFilestream\Media\Cats\Cats.mp4";
    }

    public static class ProfileId
    {
        public const int Generic = 1;
    }

    public static class GameTypeId
    {
        public const int MatchThePictures = 1;
        public const int MatchThePairs = 2;
    }

    public static class ActivityTypeId
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
    }
}
