﻿using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IPublicMediaFileService
    {
        IEnumerable<PublicMediaFile> Get();
        PublicMediaFile Get(int id);
        IEnumerable<PublicMediaFile> GetForResponseType(int responseTypdId);
        void Post(PublicMediaFile publicMediaFile);
        void Patch(int id, PublicMediaFile publicMediaFile);
        void Delete(int id);
    }

    public class PublicMediaFileService : IPublicMediaFileService
    {
        public IEnumerable<PublicMediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public PublicMediaFile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles.ByKey(id)
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)");

            return media.GetValue();
        }

        public IEnumerable<PublicMediaFile> GetForResponseType(int responseTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"ResponseTypeId eq {responseTypdId}")
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public void Post(PublicMediaFile publicMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToPublicMediaFiles(publicMediaFile);
            container.SaveChanges();
        }

        public void Patch(int id, PublicMediaFile publicMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.PublicMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var publicMediaFile = container.PublicMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (publicMediaFile == null) return;

            container.DeleteObject(publicMediaFile);
            container.SaveChanges();
        }

    }
}