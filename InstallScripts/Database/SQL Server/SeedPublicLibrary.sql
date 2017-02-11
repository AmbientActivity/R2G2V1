USE [KeebeeAAT]

/* 
TRUNCATE TABLE PublicMediaFiles
*/

DECLARE @pathProfiles varchar(max)
DECLARE @pathSharedLibrary varchar(max)
SET @pathProfiles = FileTableRootPath() + '\Media\Profiles\'
SET @pathSharedLibrary = FileTableRootPath() + '\Media\SharedLibrary\'

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\music\' AND [FileType] = 'mp3'

INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\tv-shows\'


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
WHERE pm.IsLinked = 0
ORDER BY cd.Id, pt.[Description], mf.FileType, mf.[Filename]
*/