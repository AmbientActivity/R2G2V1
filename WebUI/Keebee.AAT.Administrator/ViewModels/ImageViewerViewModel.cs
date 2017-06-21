namespace Keebee.AAT.Administrator.ViewModels
{
    public class ImageViewerViewModel
    {
        public string FilePath{ get; set; }
        public string FileType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string PaddingLeft { get; set; }
        public string Base64String { get; set; }
    }
}