﻿using Keebee.AAT.DataAccess.Models;
using System;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Keebee.AAT.DataAccess.Controllers
{
    public class MediaFilesController : ODataController
    {
        private KeebeeAATContext db = new KeebeeAATContext();

        // GET: odata/MediaFiles
        [EnableQuery]
        public IQueryable<MediaFile> GetMediaFiles()
        {
            return db.MediaFiles;
        }

        // GET: odata/MediaFiles(4b8bde1f-8175-e611-8a92-90e6bac7161a)
        [EnableQuery]
        public SingleResult<MediaFile> GetMediaFile([FromODataUri] Guid key)
        {
            return SingleResult.Create(db.MediaFiles.Where(mediaFile => mediaFile.StreamId == key));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
