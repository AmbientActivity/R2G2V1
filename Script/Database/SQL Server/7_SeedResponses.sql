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

----------------------------------------- SENSORS (BEGIN) ----------------------------------------

---------------------------------- GENERIC PROFILE (START) -------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 3, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\' AND [FileType] IN ('mp4', 'wmv')


--------------------------------- GENERIC PROFILE (END) --------------------------------------


----------------------------------- ALMA PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 7, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 8, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 10, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- ALMA PROFILE (END) ----------------------------------------

----------------------------------- EARL PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 11, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 12, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 13, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 14, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 15, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- EARL PROFILE (END) ----------------------------------------

----------------------------------- RUTH PROFILE (START) --------------------------------------

--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 16, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 17, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\shapes\' AND [FileType] = 'png'

--- Activity 3 - ResponseType "Cats" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 18, StreamId FROM MediaFiles WHERE [Path] = @pathCats

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 19, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 20, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\' AND [FileType] IN ('mp4', 'wmv')

--- Personal Pictures ---
INSERT INTO PersonalPictures (ResidentId, StreamId)
SELECT 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\pictures\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

----------------------------------- RUTH PROFILE (END) --------------------------------------

----------------------------------------- SENSORS (END) ----------------------------------------



----------------------------------------- INPUTS (BEGIN) ----------------------------------------

---------------------------------- GENERIC PROFILE (START) -------------------------------------

--- Activity 9 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 21, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 10 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 22, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\shapes\' AND [FileType] = 'png'

--- Activity 13 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 23, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 14 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 24, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\' AND [FileType] IN ('mp4', 'wmv')


--------------------------------- GENERIC PROFILE (END) --------------------------------------


----------------------------------- ALMA PROFILE (START) --------------------------------------

--- Activity 9 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 25, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 10 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 26, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\shapes\' AND [FileType] = 'png'

--- Activity 13 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 27, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 14 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 28, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\' AND [FileType] IN ('mp4', 'wmv')

----------------------------------- ALMA PROFILE (END) ----------------------------------------

----------------------------------- EARL PROFILE (START) --------------------------------------

--- Activity 9 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 29, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 10 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 30, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\shapes\' AND [FileType] = 'png'

--- Activity 13 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 31, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 14 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 32, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\' AND [FileType] IN ('mp4', 'wmv')

----------------------------------- EARL PROFILE (END) ----------------------------------------

----------------------------------- RUTH PROFILE (START) --------------------------------------

--- Activity 0 - ResponseType "SlideShow" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 33, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 10 - ResponseType "MatchingGame" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 34, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\shapes\' AND [FileType] = 'png'

--- Activity 13 - ResponseType "Radio" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 35, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\music\' AND [FileType] IN ('mp3', 'wav')

--- Activity 14 - ResponseType "Television" ---
INSERT INTO Responses (ProfileDetailId, StreamId)
SELECT 36, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\' AND [FileType] IN ('mp4', 'wmv')

----------------------------------- RUTH PROFILE (END) --------------------------------------

----------------------------------------- INPUTS (END) ----------------------------------------

-- VIEW THE RESULTS --

/*
SELECT 
	r.Id,
	p.[Description] AS [Profile],
	at.[Description] AS [ActivityType],
	rt.[Description] AS [ResponseType],
	mf.[FileType],
	mf.[Filename],
	mf.[Path]
FROM 
	Responses r
	JOIN MediaFiles mf ON r.StreamId = mf.StreamId
	JOIN ProfileDetails pd ON r.ProfileDetailId = pd.Id
	JOIN ResponseTypes rt ON pd.ResponseTypeId = rt.Id
	JOIN ActivityTypes at ON pd.ActivityTypeId = at.Id
	JOIN Profiles p ON pd.ProfileId = p.Id
ORDER BY p.Id, pd.Id, at.[Description], mf.FileType, mf.[Filename]
*/