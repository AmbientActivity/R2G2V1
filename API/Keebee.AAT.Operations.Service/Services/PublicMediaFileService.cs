using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IPublicMediaFileService
    {
        IEnumerable<PublicMediaFile> Get();
        IEnumerable<PublicMediaFile> Get(bool isSystem);
        PublicMediaFile Get(int id);
        IEnumerable<PublicMediaFile> GetForResponseType(int responseTypdId);
        IEnumerable<PublicMediaFile> GetForMediaPathType(int mediaPathTypdId);
        IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId);
        IEnumerable<PublicMediaFile> GetIdsForStreamId(Guid streamId);
        IEnumerable<PublicMediaFile> GetLinked();
        IEnumerable<PublicMediaFile> GetLinked(Guid streamId);
        PublicMediaFile GetForResponseTypeFilename(int responseTypdId, string filename);
        int Post(PublicMediaFile publicMediaFile);
        void Patch(int id, PublicMediaFile publicMediaFile);
        void Delete(int id);
    }

    public class PublicMediaFileService : IPublicMediaFileService
    {
        public IEnumerable<PublicMediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .Expand(
                    "MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public PublicMediaFile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles.ByKey(id)
                .Expand(
                    "MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)");

            PublicMediaFile result;
            try { result = media.GetValue(); }
            catch { result = null; }

            return result;
        }

        public IEnumerable<PublicMediaFile> GetForResponseType(int responseTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"ResponseTypeId eq {responseTypdId}")
                .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<PublicMediaFile> GetForMediaPathType(int mediaPathTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"MediaPathTypeId eq {mediaPathTypdId}")
                .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<PublicMediaFile> GetForStreamId(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"StreamId eq {streamId}")
                .Expand("MediaPathType($expand=MediaPathTypeCategory),ResponseType")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<PublicMediaFile> GetIdsForStreamId(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"StreamId eq {streamId}")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<PublicMediaFile> Get(bool isSystem)
        {
            var container = new Container(new Uri(ODataHost.Url));

            if (isSystem)
                return container.PublicMediaFiles
                    .AddQueryOption("$filter", "MediaPathType/IsSystem")
                    .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                    .AsEnumerable();
            else
                return container.PublicMediaFiles
                    .AddQueryOption("$filter", "MediaPathType/IsSystem eq false")
                    .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                    .AsEnumerable();
        }

        public IEnumerable<PublicMediaFile> GetLinked()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", "IsLinked")
                .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<PublicMediaFile> GetLinked(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"IsLinked and StreamId eq {streamId}")
                .Expand("MediaFile,MediaPathType($expand=MediaPathTypeCategory),ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public PublicMediaFile GetForResponseTypeFilename(int responseTypdId, string filename)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.PublicMediaFiles
                .AddQueryOption("$filter", $"ResponseTypeId eq {responseTypdId} and " +
                                           $"MediaFile/Filename eq '{filename.Replace("'", "''").Replace("&", "%26")}'")
                .ToList();

            return media.Any() ? media.Single() : null;
        }

        public int Post(PublicMediaFile publicMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            publicMediaFile.DateAdded = DateTime.Now;
            container.AddToPublicMediaFiles(publicMediaFile);
            container.SaveChanges();

            return publicMediaFile.Id;
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