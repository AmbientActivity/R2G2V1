USE [KeebeeAAT]

/* 
TRUNCATE TABLE PublicMediaFiles
*/

DECLARE @pathProfiles varchar(max)
DECLARE @pathSharedLibrary varchar(max)
DECLARE @mediaPathType varchar(max)

SET @pathProfiles = FileTableRootPath() + '\Media\Profiles\'
SET @pathSharedLibrary = FileTableRootPath() + '\Media\SharedLibrary\'

--- Activity 1 - ResponseType "SlideShow" ---
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 3 -- images/general
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'gif')
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' +  @mediaPathType + '\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
-- link from shared library
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 7 -- activities\matching-game\shapes\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'png'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' + @mediaPathType + '\' AND [FileType] = 'png'

-- link from shared library
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 8 -- activities\matching-game\sounds\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp3'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' + @mediaPathType + '\' AND [FileType] = 'mp3'

--- Activity 3 - ResponseType "Cats" ---
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 10 -- videos\cats\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 3, 10, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp4'

--- Activity 5 - ResponseType "Radio" ---
--- link from shared library
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 1 -- audio\music\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp3'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' + @mediaPathType + '\' AND [FileType] = 'mp3'

--- link from shared library
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 2 -- audio\radio-shows\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp3'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' + @mediaPathType + '\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 5 -- videos\tv-shows\
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp4'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfiles + '0\' + @mediaPathType + '\' AND [FileType] = 'mp4'

--- Activity 7 - ResponseType "Ambient" ---
SELECT @mediaPathType = [Path] FROM MediaPathTypes WHERE Id = 9 -- videos\ambient\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 8, 9, StreamId FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND [FileType] = 'mp4'

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