USE [KeebeeAAT]

-------------- REMOVE ALL RESIDENT MEDIA FILES -----------------
/* 
TRUNCATE TABLE ResidentMediaFiles
TRUNCATE TABLE PublicMediaFiles
*/

-------------------------------------- SET UP PATHS -----------------------------------

DECLARE @pathProfile varchar(max)
SET @pathProfile = FileTableRootPath() + '\Media\Profiles\'

---------------------------------- PUBLIC PROFILE (START) -------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\shapes\' AND [FileType] = 'png'
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\sounds\' AND [FileType] = 'mp3'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 3, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\' AND [Filename] IN ('Cats.mp4')

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\' AND [Filename] IN ('Charlie bit my finger.mp4')

--- ResponseType "Ambient" ---
INSERT INTO PublicMediaFiles (ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 8, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '0\videos\'  AND [Filename] IN ('Bird-Feeding.mp4', 'Nature-Sounds.mp4')


--------------------------------- GENERIC PROFILE (END) --------------------------------------


----------------------------------- ALMA PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 1, 1, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 2, 5, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 2, 6, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 5, 3, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 6, 2, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 1, 1, 4, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- ALMA PROFILE (END) ----------------------------------------

----------------------------------- EARL PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 1, 1, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 2, 5, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 2, 6, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 5, 3, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 6, 2, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 2, 1, 4, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- EARL PROFILE (END) ----------------------------------------

----------------------------------- RUTH PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 1, 1, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 2, 5, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 2, 6, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 5, 3, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 6, 2, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO ResidentMediaFiles (ResidentId, ResponseTypeId, MediaPathTypeId, IsPublic, StreamId)
SELECT 3, 1, 4, 0, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- RUTH PROFILE (END) --------------------------------------


-- VIEW THE RESULTS --

/*
SELECT 
	r.Id,
	r.[FirstName] AS [Resident],
	pt.[Description] AS [PhidgetType],
	rt.[Description] AS [ResponseType],
	mp.[Description] AS MediaPath,
	mf.[IsPublic],
	mf.[FileType],
	mf.[Filename],
	mf.[Path]
FROM 
	Residents r
	JOIN [Configs] c ON c.IsActive = 1
	JOIN ConfigDetails cd ON c.Id = cd.ConfigId
	JOIN ResponseTypes rt ON cd.ResponseTypeId = rt.Id
	JOIN PhidgetTypes pt ON cd.PhidgetTypeId = pt.Id
	JOIN ResidentMediaFiles rm ON cd.ResponseTypeId = rm.ResponseTypeId AND r.Id = rm.ResidentId
	JOIN MediaPathTypes mp ON mp.Id = rm.MediaPathTypeId
	JOIN MediaFiles mf ON rm.StreamId = mf.StreamId
ORDER BY r.Id, cd.Id, pt.[Description], mf.FileType, mf.[Filename]
*/