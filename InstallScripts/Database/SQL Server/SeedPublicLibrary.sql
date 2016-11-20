USE [KeebeeAAT]

/* 
TRUNCATE TABLE PublicMediaFiles
*/

DECLARE @pathProfile varchar(max)
SET @pathProfile = FileTableRootPath() + '\Media\Profiles\'

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 3, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\system\' AND [Filename] IN ('Cats.mp4')

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\audio\music\' AND [FileType] = 'mp3'

INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\tv-shows\' AND [Filename] NOT IN ('Bird-Feeding.mp4', 'Nature-Sounds.mp4', 'Cats.mp4')

--- ResponseType "Ambient" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 8, 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\system\'  AND [Filename] IN ('Bird-Feeding.mp4', 'Nature-Sounds.mp4')


-- VIEW THE RESULTS --

/*
SELECT 
	pt.[Description] AS [PhidgetType],
	rt.[Description] AS [ResponseType],
	mp.[Description] AS MediaPath,
	mf.[FileType],
	mf.[Filename],
	mf.[Path]
FROM 
	[Configs] c 
	JOIN ConfigDetails cd ON c.Id = cd.ConfigId
	JOIN ResponseTypes rt ON cd.ResponseTypeId = rt.Id
	JOIN PhidgetTypes pt ON cd.PhidgetTypeId = pt.Id
	JOIN PublicMediaFiles rm ON cd.ResponseTypeId = rm.ResponseTypeId
	JOIN MediaPathTypes mp ON mp.Id = rm.MediaPathTypeId
	JOIN MediaFiles mf ON rm.StreamId = mf.StreamId
WHERE c.IsActive = 1
ORDER BY cd.Id, pt.[Description], mf.FileType, mf.[Filename]
*/