using Keebee.AAT.Shared;
using Keebee.AAT.ThumbnailGeneration;
using Keebee.AAT.ApiClient.Clients;
using System;
using System.Linq;

namespace Keebee.AAT.GenerateThumbnails
{
    public class Engine
    {
        private readonly IMediaFilesClient _mediaFilesClient;
        private readonly IThumbnailsClient _thumbnailsClient;
        private readonly ThumbnailGenerator _generator;

        public Engine()
        {
            _thumbnailsClient = new ThumbnailsClient();
            _mediaFilesClient = new MediaFilesClient();
            _generator = new ThumbnailGenerator();
        }

        public void GenerateThumbnails(bool overwrite, bool deleteOrphans)
        {
            try
            {
#if DEBUG
                Console.WriteLine("---------------------");
                Console.WriteLine("Generating Thumbnails");
                Console.WriteLine("---------------------");
                Console.WriteLine();
#endif
                var files = _mediaFilesClient.Get()
                    .SelectMany(x => x.Files)
                    .Where(x => x.FileType != "db")
                    .OrderBy(x => x.FileType);

                foreach (var file in files)
                {
                    try
                    {
                        if (!overwrite)
                        {
                            if (_generator.ThumbnailExists(file.StreamId)) continue;
                        }

                        var categoryId = GetCategoryId(file.FileType);
                        if (categoryId == MediaPathTypeCategoryId.Audio) continue;
#if DEBUG
                        var desc = GetCategoryDescription(categoryId).ToLower();
                        Console.Write($"Generating thumbnail for {desc} file: {file.Filename}...");
#endif
                        // generate
                        string errorMessage = null;
                        var image = _generator.Get(file.StreamId, out errorMessage);
                        if (errorMessage != null) throw new Exception(errorMessage);

                        // save
                        errorMessage = _generator.Save(file.StreamId, image, overwrite);
                        if (errorMessage != null) throw new Exception(errorMessage);
#if DEBUG
                        Console.WriteLine("done.");
#endif
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("error.");
                        Console.WriteLine($"Description: {ex.Message}");
                        Console.WriteLine();
#endif
                    }
                }
#if DEBUG
                Console.WriteLine("Complete.");

                if (deleteOrphans)
                {
                    DeleteThumbnails(true);
                }
                else
                {
                    Console.WriteLine("Press any key to exit");
                    Console.ReadKey();
                }
#endif
            }
            catch (Exception)
            {
                // log error
            }
        }

        public void DeleteThumbnails(bool orphansOnly)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("----------------");
                Console.WriteLine("Deleting Orphans");
                Console.WriteLine("----------------");
                Console.WriteLine();

                var thumbnails = _thumbnailsClient.Get();

                foreach (var thumb in thumbnails)
                {
                    try
                    {

                        var mediaFile = _mediaFilesClient.Get(thumb.StreamId);
                        var isOrphan = (mediaFile == null);

                        if (isOrphan && orphansOnly)
                        {
#if DEBUG
                            Console.Write($"Deleting thumbnail for StreamId: {thumb.StreamId}...");
#endif
                            // delete
                            var errorMessage = _generator.Delete(thumb.StreamId);
                            if (errorMessage != null) throw new Exception(errorMessage);

#if DEBUG
                            Console.WriteLine("done.");
#endif
                        }
                        else if (!isOrphan && !orphansOnly)
                        {
#if DEBUG
                            var categoryId = GetCategoryId(mediaFile.FileType);
                            var desc = GetCategoryDescription(categoryId).ToLower();
                            Console.Write($"Deleting thumbnail for {desc} file: {mediaFile.Filename}...");
#endif
                            // delete
                            var errorMessage = _generator.Delete(thumb.StreamId);
                            if (errorMessage != null) throw new Exception(errorMessage);

#if DEBUG
                            Console.WriteLine("done.");
#endif
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("error.");
                        Console.WriteLine($"Description: {ex.Message}");
                        Console.WriteLine();
#endif
                    }
                }
#if DEBUG
                Console.WriteLine("Complete.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
#endif
            }
            catch (Exception)
            {
                // log error
            }
        }

        private static int GetCategoryId(string fileType)
        {
            switch (fileType.ToLower())
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "bmp":
                    return MediaPathTypeCategoryId.Image;
                case "mp3":
                    return MediaPathTypeCategoryId.Audio;
                case "mp4":
                    return MediaPathTypeCategoryId.Video;
                default:
                    return -1;
            }
        }

        private static string GetCategoryDescription(int categoryTypeId)
        {
            switch (categoryTypeId)
            {
                case MediaPathTypeCategoryId.Image:
                    return MediaPathTypeCategoryDescription.Image;
                case MediaPathTypeCategoryId.Audio:
                    return MediaPathTypeCategoryDescription.Audio;
                case MediaPathTypeCategoryId.Video:
                    return MediaPathTypeCategoryDescription.Video;
                default:
                    return string.Empty;
            }
        }
    }
}
