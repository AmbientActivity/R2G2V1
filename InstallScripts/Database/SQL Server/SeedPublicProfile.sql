USE [KeebeeAAT]

/* 
TRUNCATE TABLE PublicMediaFiles
*/

DECLARE @pathProfiles varchar(max)
DECLARE @pathSharedLibrary varchar(max)
SET @pathProfiles = FileTableRootPath() + '\Media\Profiles\'
SET @pathSharedLibrary = FileTableRootPath() + '\Media\SharedLibrary\'

--- Activity 1 - ResponseType "SlideShow" ---
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
-- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 3, 10, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\cats\' AND [FileType] = 'mp4'

--- Activity 5 - ResponseType "Radio" ---
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\music\' AND [FileType] = 'mp3'
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\radio-shows\' AND [FileType] = 'mp3'

--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\audio\music\' AND [FileType] = 'mp3'
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\tv-shows\'

--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\videos\tv-shows\'

--- Activity 7 - ResponseType "Ambient" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 8, 9, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\ambient\' AND [FileType] = 'mp4'

-- VIEW THE RESULTS --

/*
SELECT 
	pt.[Description] AS [PhidgetType],
	rt.[Description] AS [ResponseType],
	mp.[Description] AS MediaPath,
	mf.[FileType],
	mf.[Filename],
	mf.[Path],
	pm.[IsLinked]
FROM 
	[Configs] c 
	JOIN ConfigDetails cd ON c.Id = cd.ConfigId
	JOIN ResponseTypes rt ON cd.ResponseTypeId = rt.Id
	JOIN PhidgetTypes pt ON cd.PhidgetTypeId = pt.Id
	JOIN PublicMediaFiles pm ON cd.ResponseTypeId = pm.ResponseTypeId
	JOIN MediaPathTypes mp ON mp.Id = pm.MediaPathTypeId
	JOIN MediaFiles mf ON pm.StreamId = mf.StreamId
--WHERE c.Id = 1
ORDER BY cd.Id, pt.[Description], mf.FileType, mf.[Filename]
*/