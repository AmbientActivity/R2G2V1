﻿using Keebee.AAT.Shared;
using Keebee.AAT.Display.Extensions;
using Keebee.AAT.ApiClient.Clients;
using System.Linq;

namespace Keebee.AAT.Display.Helpers
{
    public static class MatchingGameConfig
    {
        public const int MinNumShapes = 23;
        public const int MinNumSounds = 8;
        public const string WouldYouListToMatchThePictures = "would-you-like-to-match-the-pictures.mp3";
        public const string WouldYouListToMatchThePairs = "would-you-like-to-match-the-pairs.mp3";
        public const string Correct = "correct.mp3";
        public const string GoodJob = "good-job.mp3";
        public const string WellDone = "well-done.mp3";
        public const string TryAgain = "try-again.mp3";
        public const string LetsTryAgain = "lets-try-again.mp3";
        public const string LetsTrySomethingDifferent = "lets-try-something-different.mp3";
    }

    public class MatchingGameSetup
    {
        private readonly PublicMediaFilesClient _publicMediaFilesClient = new PublicMediaFilesClient();

        private readonly MediaSourcePath _mediaPath = new MediaSourcePath();

        public string[] GetTotalShapes(string[] residentShapes)
        {
            var shapeCount = residentShapes.Length;
            if (shapeCount >= MatchingGameConfig.MinNumShapes) return residentShapes;

            var mediaResponseTypes = _publicMediaFilesClient.GetForMediaPathType(MediaPathTypeId.MatchingGameShapes)
                .ToArray();

            var mediaPaths = mediaResponseTypes
                .Single(x => x.ResponseType.Id == ResponseTypeId.MatchingGame)
                .Paths.Where(p => p.MediaPathType.Id == MediaPathTypeId.MatchingGameShapes).ToArray();

            var mediaPathType = mediaPaths
                .Single(x => x.MediaPathType.Id == MediaPathTypeId.MatchingGameShapes)
                .MediaPathType.Path;

            var publicShapes = mediaResponseTypes.SelectMany(m => m.Paths)
                .SelectMany(p => p.Files)
                .Select(f =>
                    {
                        var pathRoot = f.IsLinked
                            ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                            : $@"{_mediaPath.ProfileRoot}\{MediaSourceTypeId.Public}";

                        return $@"{pathRoot}\{mediaPathType}\{f.Filename}";
                    }).Except(residentShapes)
                .ToArray();

            publicShapes.Shuffle();

            var additionalShapes = publicShapes.Take(MatchingGameConfig.MinNumShapes - shapeCount);

            return residentShapes.Concat(additionalShapes).ToArray();
        }

        public string[] GetTotalSounds(string[] residentSounds)
        {
            var soundCount = residentSounds.Length;
            if (soundCount >= MatchingGameConfig.MinNumSounds) return residentSounds;

            var mediaFiles = _publicMediaFilesClient.GetForMediaPathType(MediaPathTypeId.MatchingGameSounds)
                .ToArray();

            var mediaPaths = mediaFiles
                .Single(x => x.ResponseType.Id == ResponseTypeId.MatchingGame).Paths
                .Where(p => p.MediaPathType.Id == MediaPathTypeId.MatchingGameSounds)
                .ToArray();

            var mediaPathType = mediaPaths
                .Single(x => x.MediaPathType.Id == MediaPathTypeId.MatchingGameSounds)
                .MediaPathType.Path;

            var publicSounds = mediaFiles.SelectMany(m => m.Paths)
                .SelectMany(p => p.Files)
                .Select(f =>
                {
                    var pathRoot = f.IsLinked
                        ? $@"{_mediaPath.MediaRoot}\{_mediaPath.SharedLibrary}"
                        : $@"{_mediaPath.ProfileRoot}\{MediaSourceTypeId.Public}";

                    return $@"{pathRoot}\{mediaPathType}\{f.Filename}";
                }).ToArray();

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.WouldYouListToMatchThePictures)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.WouldYouListToMatchThePictures)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.WouldYouListToMatchThePairs)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.WouldYouListToMatchThePairs)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.Correct)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.Correct)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.GoodJob)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.GoodJob)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.WellDone)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.WellDone)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.TryAgain)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.TryAgain)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.LetsTryAgain)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.LetsTryAgain)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            if (residentSounds.All(s => !s.Contains(MatchingGameConfig.LetsTrySomethingDifferent)))
            {
                var sound = publicSounds.Where(s => s.Contains(MatchingGameConfig.LetsTrySomethingDifferent)).ToArray();
                residentSounds = residentSounds.Concat(sound).ToArray();
            }

            return residentSounds;
        }
    }
}
