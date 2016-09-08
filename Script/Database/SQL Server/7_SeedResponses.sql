USE [KeebeeAAT]

-------------- REMOVE ALL AMBIENT RESPONSES AND PROFILE RESPONE DETAILS -----------------
/* 
TRUNCATE TABLE Responses
TRUNCATE TABLE PersonalPictures
TRUNCATE TABLE AmbientResponses
*/

-------------------------------------- SET UP PATHS -----------------------------------

DECLARE @pathProfile varchar(max)
SET @pathProfile = FileTableRootPath() + '\Media\Profiles\'

DECLARE @pathAmbient varchar(max)
SET @pathAmbient = FileTableRootPath() + '\Media\Ambient\videos\'

DECLARE @pathCats varchar(max)
SET @pathCats = FileTableRootPath() + '\Media\Cats\'

---------------------------------------- AMBIENT (START) -------------------------------------

--- Insert Ambient Videos ---
INSERT INTO AmbientResponses (StreamId, ResponseTypeId)
SELECT StreamId, 1 FROM MediaFiles WHERE [Path] = @pathAmbient

-- VIEW THE RESULTS --
/*
SELECT rt.[Description] AS AmbientResponse, mf.[FileType], mf.[Filename], mf.[Path] 
FROM AmbientResponses a 
JOIN ResponseTypes rt ON a.ResponseTypeId = rt.Id
JOIN MediaFiles mf ON a.StreamId = mf.StreamId
*/
----------------------------------------- AMBIENT (END) ----------------------------------------

---------------------------------- GENERIC PROFILE (START) -------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 1, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 1, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 1, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\' AND [FileType] IN ('mp4', 'wmv')


--------------------------------- GENERIC PROFILE (END) --------------------------------------


----------------------------------- ALMA PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 2, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 2, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 2, 3, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- ALMA PROFILE (END) ----------------------------------------

----------------------------------- EARL PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 3, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 3, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 3, 3, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 3, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 3, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- EARL PROFILE (END) ----------------------------------------

----------------------------------- RUTH PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 4, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 4, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 4, 3, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 4, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileId, ResponseTypeId, StreamId)
SELECT 4, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- RUTH PROFILE (END) --------------------------------------


-- VIEW THE RESULTS --

/*
SELECT 
	r.Id,
	p.[Description] AS [Profile],
	at.Id AS ActivityTypeId,
	at.[Description] AS [ActivityType],
	rt.Id AS ResponseTypeId,
	rt.[Description] AS [ResponseType],
	mf.[FileType],
	mf.[Filename],
	mf.[Path]
FROM 
	Profiles p
	JOIN Configurations c ON c.IsActive = 1
	JOIN ConfigurationDetails cd ON c.Id = cd.ConfigurationId
	JOIN ResponseTypes rt ON cd.ResponseTypeId = rt.Id
	JOIN ActivityTypes at ON cd.ActivityTypeId = at.Id
	JOIN Responses r ON cd.ResponseTypeId = r.ResponseTypeId AND p.Id = r.ProfileId
	JOIN MediaFiles mf ON r.StreamId = mf.StreamId
ORDER BY p.Id, cd.Id, at.[Description], mf.FileType, mf.[Filename]
*/