﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IMediaPathTypeService
    {
        IEnumerable<MediaPathType> Get();
        MediaPathType Get(int id);
        IEnumerable<MediaPathType> Get(bool isSystem);
        void Post(MediaPathType mediaPathType);
        void Patch(int id, MediaPathType mediaPathType);
        void Delete(int id);
    }

    public class MediaPathTypeService : IMediaPathTypeService
    {
        public IEnumerable<MediaPathType> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathTypes = container.MediaPathTypes
                .Expand("MediaPathTypeCategory")
                .AsEnumerable();

            return mediaPathTypes;
        }

        public MediaPathType Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathType = container.MediaPathTypes.ByKey(id)
                .Expand("MediaPathTypeCategory");

            MediaPathType result;
            try { result = mediaPathType.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<MediaPathType> Get(bool isSystem)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathTypes = (isSystem)
                ? container.MediaPathTypes
                    .Expand("MediaPathTypeCategory")
                    .AddQueryOption("$filter", "IsSystem")
                    .AsEnumerable()
                : container.MediaPathTypes
                    .Expand("MediaPathTypeCategory")
                    .AddQueryOption("$filter", "IsSystem eq false")
                    .AsEnumerable();

            return mediaPathTypes;
        }

        public void Post(MediaPathType mediaPathType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToMediaPathTypes(mediaPathType);
            container.SaveChanges();
        }

        public void Patch(int id, MediaPathType mediaPathType)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.MediaPathTypes.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (mediaPathType.MediaPathTypeCategoryId > 0)
                el.MediaPathTypeCategoryId = mediaPathType.MediaPathTypeCategoryId;

            if (!string.IsNullOrEmpty(mediaPathType.Path))
                el.Path = mediaPathType.Path;

            if (!string.IsNullOrEmpty(mediaPathType.Description))
                el.Description = mediaPathType.Description;

            if (!string.IsNullOrEmpty(mediaPathType.ShortDescription))
                el.ShortDescription = mediaPathType.ShortDescription;

            if (!string.IsNullOrEmpty(mediaPathType.AllowedTypes))
                el.AllowedTypes = mediaPathType.AllowedTypes;

            if (mediaPathType.MaxFileBytes > 0)
                el.MaxFileBytes = mediaPathType.MaxFileBytes;

            if (mediaPathType.MaxFileUploads > 0)
                el.MaxFileUploads = mediaPathType.MaxFileUploads;

            if (!string.IsNullOrEmpty(mediaPathType.AllowedExts))
                el.AllowedExts = mediaPathType.AllowedExts;

            if (!string.IsNullOrEmpty(mediaPathType.AllowedTypes))
                el.AllowedTypes = mediaPathType.AllowedTypes;

            el.IsSystem = mediaPathType.IsSystem;
            el.IsSharable = mediaPathType.IsSharable;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var mediaPathType = container.MediaPathTypes.Where(e => e.Id == id).SingleOrDefault();
            if (mediaPathType == null) return;

            container.DeleteObject(mediaPathType);
            container.SaveChanges();
        }
    }
}
