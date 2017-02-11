using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface ISystemMediaFileService
    {
        IEnumerable<SystemMediaFile> Get();
        SystemMediaFile Get(int id);
        IEnumerable<SystemMediaFile> GetForResponseType(int responseTypdId);
        IEnumerable<SystemMediaFile> GetForMediaPathType(int mediaPathTypdId);
        SystemMediaFile GetForResponseTypeFilename(int responseTypdId, string filename);
        void Post(SystemMediaFile systemMediaFile);
        void Patch(int id, SystemMediaFile systemMediaFile);
        void Delete(int id);
    }

    public class SystemMediaFileService : ISystemMediaFileService
    {
        public IEnumerable<SystemMediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.SystemMediaFiles
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public SystemMediaFile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.SystemMediaFiles.ByKey(id)
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)");

            return media.GetValue();
        }

        public IEnumerable<SystemMediaFile> GetForResponseType(int responseTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.SystemMediaFiles
                .AddQueryOption("$filter", $"ResponseTypeId eq {responseTypdId}")
                .Expand("MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<SystemMediaFile> GetForMediaPathType(int mediaPathTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.SystemMediaFiles
                .AddQueryOption("$filter", $"MediaPathTypeId eq {mediaPathTypdId}")
                .Expand("MediaFile,MediaPathType,ResponseType")
                .AsEnumerable();

            return media;
        }

        public SystemMediaFile GetForResponseTypeFilename(int responseTypdId, string filename)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.SystemMediaFiles
                .AddQueryOption("$filter", $"ResponseTypeId eq {responseTypdId} and " +
                                           $"MediaFile/Filename eq '{filename.Replace("'", "''").Replace("&", "%26")}'")
                .ToList();

            return media.Any() ? media.Single() : null;
        }

        public void Post(SystemMediaFile systemMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToSystemMediaFiles(systemMediaFile);
            container.SaveChanges();
        }

        public void Patch(int id, SystemMediaFile systemMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.SystemMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var systemMediaFile = container.SystemMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (systemMediaFile == null) return;

            container.DeleteObject(systemMediaFile);
            container.SaveChanges();
        }

    }
}