using Keebee.AAT.Operations.Service.Keebee.AAT.DataAccess.Models;
using Keebee.AAT.Operations.Service.KeebeeAAT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Operations.Service.Services
{
    public interface IResidentMediaFileService
    {
        IEnumerable<ResidentMediaFile> Get();
        ResidentMediaFile Get(int id);
        IEnumerable<ResidentMediaFile> GetForResident(int residentId);
        IEnumerable<ResidentMediaFile> GetForResidentResponseType(int residentId, int responseTypdId);
        IEnumerable<ResidentMediaFile> GetLinked();
        IEnumerable<ResidentMediaFile> GetLinked(Guid streamId);
        IEnumerable<ResidentMediaFile> GetIdsForStreamId(Guid streamId);
        int Post(ResidentMediaFile residentMediaFile);
        void Patch(int id, ResidentMediaFile residentMediaFile);
        void Delete(int id);
    }

    public class ResidentMediaFileService : IResidentMediaFileService
    {
        public IEnumerable<ResidentMediaFile> Get()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public ResidentMediaFile Get(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles.ByKey(id)
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)");

            return media.GetValue();
        }

        public IEnumerable<ResidentMediaFile> GetForResident(int residentId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .AddQueryOption("$filter",$"ResidentId eq {residentId}")
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<ResidentMediaFile> GetForResidentResponseType(int residentId, int responseTypdId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .AddQueryOption("$filter", $"ResidentId eq {residentId} and ResponseTypeId eq {responseTypdId}")
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<ResidentMediaFile> GetLinked()
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .AddQueryOption("$filter", "IsLinked")
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<ResidentMediaFile> GetLinked(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .AddQueryOption("$filter", $"IsLinked and StreamId eq {streamId}")
                .Expand("Resident,MediaFile,MediaPathType,ResponseType($expand=ResponseTypeCategory)")
                .AsEnumerable();

            return media;
        }

        public IEnumerable<ResidentMediaFile> GetIdsForStreamId(Guid streamId)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var media = container.ResidentMediaFiles
                .AddQueryOption("$filter", $"StreamId eq {streamId}")
                .AsEnumerable();

            return media;
        }

        public int Post(ResidentMediaFile residentMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            container.AddToResidentMediaFiles(residentMediaFile);
            container.SaveChanges();

            return residentMediaFile.Id;
        }

        public void Patch(int id, ResidentMediaFile residentMediaFile)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var el = container.ResidentMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (el == null) return;

            if (el.ResidentId != null)
                el.ResidentId = residentMediaFile.ResidentId;

            container.UpdateObject(el);
            container.SaveChanges();
        }

        public void Delete(int id)
        {
            var container = new Container(new Uri(ODataHost.Url));

            var residentMediaFile = container.ResidentMediaFiles.Where(e => e.Id == id).SingleOrDefault();
            if (residentMediaFile == null) return;

            container.DeleteObject(residentMediaFile);
            container.SaveChanges();
        }

    }
}