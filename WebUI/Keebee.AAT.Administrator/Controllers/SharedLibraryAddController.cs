using Keebee.AAT.Administrator.ViewModels;
using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.BusinessRules;
using Keebee.AAT.Administrator.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Keebee.AAT.Administrator.Controllers
{
    public class SharedLibraryAddController : Controller
    {
        private readonly IThumbnailsClient _thumbnailsClient;

        public SharedLibraryAddController()
        {
            _thumbnailsClient = new ThumbnailsClient();
        }

        [HttpGet]
        [Authorize]
        public JsonResult GetView(int profileId, int mediaPathTypeId, string mediaPathTypeDesc, string mediaPathTypeCategory)
        {
            string errMsg = null;
            string html = null;

            try
            {
                html = this.RenderPartialViewToString("_Index", new SharedLibraryAddViewModel
                {
                    ProfileId = profileId,
                    MediaPathTypeId = mediaPathTypeId,
                    MediaPathTypeDesc = mediaPathTypeDesc,
                    MediaPathTypeCategory = mediaPathTypeCategory
                });
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                Html = html,
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize]
        public JsonResult LoadData(int profileId, int mediaPathTypeId)
        {
            string errMsg = null;
            var fileList = new SharedLibraryFileViewModel[0];

            try
            {
                var rules = new SharedLibraryAddRules();

                var files = rules.GetAvailableSharedMediaFiles(profileId, mediaPathTypeId);
                var thumbnails = _thumbnailsClient.Get().ToArray();

                fileList = files.Select(f =>
                {
                    var thumb = thumbnails.FirstOrDefault(x => x.StreamId == f.StreamId);

                    return new SharedLibraryFileViewModel
                    {
                        StreamId = f.StreamId,
                        Filename = f.Filename.Replace($".{f.FileType}", string.Empty),
                        FileType = f.FileType,
                        Thumbnail = thumb?.Image != null
                        ? $"data:image/jpg;base64,{Convert.ToBase64String(thumb.Image)}"
                        : string.Empty
                    };
                }).ToArray();

            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }

            return Json(new
            {
                Success = string.IsNullOrEmpty(errMsg),
                ErrorMessage = errMsg,
                FileList = fileList
            }, JsonRequestBehavior.AllowGet);
        }
    }
}