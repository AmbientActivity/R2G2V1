USE [KeebeeAAT]

/* 
TRUNCATE TABLE SystemMediaFiles
*/

DECLARE @pathSystemLibrary varchar(max)
SET @pathSystemLibrary = FileTableRootPath() + '\Media\SystemLibrary\'

--- ResponseType "Ambient" ---
INSERT INTO SystemMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 8, 9, StreamId FROM MediaFiles WHERE [Path] = @pathSystemLibrary + 'videos\ambient\' AND [FileType] = 'mp4'

--- ResponseType "Cats" ---
INSERT INTO SystemMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 3, 10, StreamId FROM MediaFiles WHERE [Path] = @pathSystemLibrary + 'videos\cats\' AND [FileType] = 'mp4'

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
	JOIN SystemMediaFiles pm ON cd.ResponseTypeId = pm.ResponseTypeId
	JOIN MediaPathTypes mp ON mp.Id = pm.MediaPathTypeId
	JOIN MediaFiles mf ON pm.StreamId = mf.StreamId
--WHERE c.Id = 1
ORDER BY cd.Id, pt.[Description], mf.FileType, mf.[Filename]
*/