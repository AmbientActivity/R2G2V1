﻿using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.Display.Caregiver.Helpers
{
    public class MediaFileQuery
    {
        private readonly IEnumerable<ResponseTypePaths> _mediaFiles;
        private readonly int _currentResidentId;
        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();
        private readonly IEnumerable<ResponseTypePaths> _publicMediaFiles;
        private readonly IEnumerable<Thumbnail> _thumbnails;

        public MediaFileQuery(IEnumerable<ResponseTypePaths> mediaFiles, 
            IEnumerable<ResponseTypePaths> publicMediaFiles, 
            IEnumerable<Thumbnail> thumbnails, int currentResidentId)
        {
            _mediaFiles = mediaFiles;
            _publicMediaFiles = publicMediaFiles;
            _currentResidentId = currentResidentId;
            _thumbnails = thumbnails;

        }

        // get filenames with full path
        public string[] GetFilePaths(int mediaPathTypeId, int? responseTypeId = null, Guid? streamId = null)
        {
            string[] files = null;

            var isPublic = _currentResidentId == PublicProfileSource.Id;
            var mediaFiles = _mediaFiles.ToArray();

            var paths = mediaFiles
                .Where(x => x.ResponseType.Id == responseTypeId || responseTypeId == null)
                .SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            if (!paths.Any()) return new string[0];

            // get the path type
            var mediaPath = paths.Single(x => x.MediaPathType.Id == mediaPathTypeId);

            // get the description
            var mediaPathType = mediaPath.MediaPathType.Path;

            if (streamId != null)
            {
                // organize the files so that the selected appears first in the list
                var selectedFile = mediaPath.Files
                    .Single(f => f.StreamId == streamId);

                var pathRoot = (selectedFile.IsLinked)
                    ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                    : $@"{_mediaPath.ProfileRoot}\{_currentResidentId}";

                var selectedFilename = mediaPath.Files
                    .Single(f => f.StreamId == streamId).Filename;

                var filesAfterSelected = new[] { $@"{pathRoot}\{mediaPathType}\{selectedFilename}" }
                    .Union(mediaPath.Files
                    .OrderBy(f => f.Filename)
                    .SkipWhile(f => f.Filename != selectedFilename)
                    .Select(f =>
                    {
                        var root = (f.IsLinked)
                            ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                            : $@"{_mediaPath.ProfileRoot}\{_currentResidentId}";

                        return $@"{root}\{mediaPathType}\{f.Filename}";
                    }))
                    .ToArray();

                var filesBeforeSelected = mediaPath.Files
                        .Where(f => f.IsLinked == false || isPublic)
                        .OrderBy(f => f.Filename)
                        .Select(f =>
                        {
                            var root = (f.IsLinked)
                                ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                                : $@"{_mediaPath.ProfileRoot}\{_currentResidentId}";

                            return $@"{root}\{mediaPathType}\{f.Filename}";
                        })
                        .Except(filesAfterSelected)
                        .ToArray();

                files = filesAfterSelected.Concat(filesBeforeSelected).ToArray();
            }
            else
            {
                files = mediaPath.Files
                    .OrderBy(f => f.Filename)
                    .Select(f =>
                    {
                        var pathRoot = (f.IsLinked)
                            ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                            : $@"{_mediaPath.ProfileRoot}\{_currentResidentId}";

                        return $@"{pathRoot}\{mediaPath.MediaPathType.Path}\{f.Filename}";
                    })
                .ToArray();
            }

            return files;
        }

        // get filenames with no extensions or path
        public IEnumerable<MediaFileThumbnail> GetMediaFiles(int mediaPathTypeId, int? responseTypeId = null)
        {
            var mediaFiles = _mediaFiles.ToArray();
            
            var paths = mediaFiles
                .Where(x => x.ResponseType.Id == responseTypeId || responseTypeId == null)
                .SelectMany(x => x.Paths)
                .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                .ToArray();

            if (!paths.Any())
            {
                if (responseTypeId == ResponseTypeId.MatchingGame)
                {
                    paths = _publicMediaFiles
                        .Where(x => x.ResponseType.Id == responseTypeId)
                        .SelectMany(x => x.Paths)
                        .Where(x => x.MediaPathType.Id == mediaPathTypeId)
                        .ToArray();
                }
                else return new List<MediaFileThumbnail>();
            }

            return paths
                .Single(x => x.MediaPathType.Id == mediaPathTypeId).Files
                .Select(f =>
                {
                    var thumbnail = _thumbnails.FirstOrDefault(x => x.StreamId == f.StreamId);
                    return new MediaFileThumbnail
                    {
                        StreamId = f.StreamId,
                        Filename = f.Filename.Replace($".{f.FileType}", string.Empty),
                        Thumbnail = thumbnail?.Image
                    };
                }).OrderBy(f => f.Filename);
        }
    }
}
