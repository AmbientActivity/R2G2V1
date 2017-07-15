namespace Keebee.AAT.ApiClient.Models
{
    public class MediaPathType
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Path { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string AllowedExts { get; set; }
        public string AllowedTypes { get; set; }
        public long MaxFileBytes { get; set; }
        public int MaxFileUploads { get; set; }
        public bool IsSystem { get; set; }
        public bool IsSharable { get; set; }
    }
}
