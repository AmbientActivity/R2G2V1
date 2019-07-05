using Keebee.AAT.ApiClient.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Helpers;
using NReco.VideoConverter;

namespace Keebee.AAT.ThumbnailGeneration
{
    public static class Helpers
    {
        private const int Dimensions = 96;
        public static byte[] GetVideoThumbnail(string filePath, out string errorMessage)
        {
            errorMessage = null;
            byte[] byteArray = null;

            try
            {
                var ffMpeg = new FFMpegConverter();

                var stream = new MemoryStream();
                ffMpeg.GetVideoThumbnail(filePath, stream, 2.0f);

                byteArray = GetCroppedImage(stream, "jpg", out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return byteArray;
        }

        public static byte[] GetImageThumbnail(MediaFileStream file, out string errorMessage)
        {
            byte[] byteArray = null;
            errorMessage = null;

            try
            {
                // convert to image and orient correctly
                var stream = new MemoryStream(file.Stream);
                var image = Image.FromStream(stream);

                var cropped = image.Orient();
                byteArray = GetCroppedImage(cropped, file.FileType, out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return byteArray;
        }

        private static byte[] GetCroppedImage(Stream stream, string fileType, out string errorMessage)
        {
            errorMessage = null;
            byte[] byteArray = null;

            try
            {
                byteArray = CropImage(stream);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return byteArray;
        }

        private static byte[] GetCroppedImage(Image image, string fileType, out string errorMessage)
        {
            errorMessage = null;
            byte[] byteArray = null;

            try
            {
                // convert image to stream
                var stream = new MemoryStream();
                image.Save(stream, GetImageFormat(fileType));

                byteArray = CropImage(stream);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            return byteArray;
        }

        private static byte[] CropImage(Stream stream)
        {
            // convert to web image and resize
            var webImg = new WebImage(stream);

            var webImgCropped = webImg.CustomCrop(1);
            webImgCropped.Resize(Dimensions, Dimensions, true, true);

            return webImgCropped.GetBytes();
        }

        private static ImageFormat GetImageFormat(string fileType)
        {
            switch (fileType.ToLower().Replace(".", string.Empty))
            {
                case "jpg":
                    return ImageFormat.Jpeg;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                case "bmp":
                    return ImageFormat.Bmp;
                default:
                    return ImageFormat.Jpeg;
            }
        }

        // OLD WAY OF GENERATING IMAGE THUMBNAILS
        //public static Image Get(string fileName, int dimensions)
        //{
        //    Bitmap bitmap;
        //    using (Stream bmpStream = File.Open(fileName, FileMode.Open))
        //    {
        //        var image = Image.FromStream(bmpStream);
        //        bitmap = new Bitmap(image);
        //    }

        //    return bitmap.GetThumbnailImage(dimensions, dimensions, () => true, new IntPtr());
        //}
    }
}
