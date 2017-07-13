using Keebee.AAT.ApiClient.Clients;
using Keebee.AAT.ApiClient.Models;
using Keebee.AAT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Keebee.AAT.BusinessRules
{
    public class SharedLibraryAddRules
    {
        private readonly IPublicMediaFilesClient _publicMediaFilesClient;
        private readonly IResidentMediaFilesClient _residentMediaFilesClient;
        private readonly IMediaPathTypesClient _mediaPathTypesClient;
        private readonly IMediaFilesClient _mediaFilesClient;

        public SharedLibraryAddRules()
        {
            _publicMediaFilesClient = new PublicMediaFilesClient();
            _mediaFilesClient = new MediaFilesClient();
            _residentMediaFilesClient = new ResidentMediaFilesClient();
            _mediaPathTypesClient = new MediaPathTypesClient();
        }

        public IEnumerable<MediaFile> GetAvailableSharedMediaFiles(int profileId, int mediaPathTypeId)
        {
            return profileId > 0
                ? GetAvailableSharedMediaFilesResident(profileId, mediaPathTypeId)
                : GetAvailableSharedMediaFilesPublic(mediaPathTypeId);
        }

        private IEnumerable<MediaFile> GetAvailableSharedMediaFilesPublic(int mediaPathTypeId)
        {
            var mediaSource = new MediaSourcePath();
            var mediaPath = GetMediaPath(mediaPathTypeId);
            var sharedPaths = _mediaFilesClient.GetForPath($@"{mediaSource.SharedLibrary}\{mediaPath}").ToArray();
            var mediaResponseTypes = _publicMediaFilesClient.GetForMediaPathType(mediaPathTypeId).ToArray();
            IEnumerable<Guid> existingStreamIds = new List<Guid>();

            if (mediaResponseTypes.Any())
            {
                existingStreamIds = mediaResponseTypes.SelectMany(p => p.Paths)
                    .SelectMany(f => f.Files)
                    .Where(f => f.IsLinked)
                    .Select(f => f.StreamId);
            }

            var availableFiles = sharedPaths
                .SelectMany(f => f.Files)
                .Where(f => !existingStreamIds.Contains(f.StreamId));

            return availableFiles;
        }

        private IEnumerable<MediaFile> GetAvailableSharedMediaFilesResident(int residentId, int mediaPathTypeId)
        {
            var mediaSource = new MediaSourcePath();
            var mediaPath = GetMediaPath(mediaPathTypeId);

            var sharedPaths = _mediaFilesClient.GetForPath($@"{mediaSource.SharedLibrary}\{mediaPath}").ToArray();
            var responseTypeId = GetResponseTypeId(mediaPathTypeId);

            var paths = _residentMediaFilesClient.GetForResidentResponseType(residentId, responseTypeId);
            IEnumerable<Guid> existingStreamIds = new List<Guid>();

            if (paths != null)
            {
                existingStreamIds = paths
                    .SelectMany(f => f.Files)
                    .Where(f => f.IsLinked)
                    .Select(f => f.StreamId);
            }

            var availableFiles = sharedPaths
                .SelectMany(f => f.Files)
                .Where(f => !existingStreamIds.Contains(f.StreamId));

            return availableFiles;
        }

        public static int GetResponseTypeId(int mediaPathTypeId)
        {
            var responseTypeId = -1;

            switch (mediaPathTypeId)
            {
                case MediaPathTypeId.ImagesPersonal:
                case MediaPathTypeId.ImagesGeneral:
                    responseTypeId = ResponseTypeId.SlideShow;
                    break;
                case MediaPathTypeId.HomeMovies:
                case MediaPathTypeId.TVShows:
                    responseTypeId = ResponseTypeId.Television;
                    break;
                case MediaPathTypeId.Music:
                case MediaPathTypeId.RadioShows:
                    responseTypeId = ResponseTypeId.Radio;
                    break;
                case MediaPathTypeId.MatchingGameShapes:
                case MediaPathTypeId.MatchingGameSounds:
                    responseTypeId = ResponseTypeId.MatchingGame;
                    break;
            }

            return responseTypeId;
        }

        public string GetMediaPath(int? mediaPathTypeId)
        {
            return mediaPathTypeId != null
                ? _mediaPathTypesClient.Get((int)mediaPathTypeId).Path
                : _mediaPathTypesClient.Get(MediaPathTypeId.ImagesGeneral).Path;
        }
    }
}
