using System;

namespace Keebee.AAT.RESTClient
{
    public class Response
    {
        public int Id { get; set; }

        public int ProfileDetailId { get; set; }

        public Guid StreamId { get; set; }

        public int GameDifficultyLevel { get; set; }

        public string Filename { get; set; }

        public string FilePath { get; set; }

        public string FileType { get; set; }

        public int FileSize { get; set; }
    }
}
