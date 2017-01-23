USE [KeebeeAAT]

/* 
TRUNCATE TABLE ResidentMediaFiles
*/

-- insert demo resident
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (1, 'Demo', 'Resident', 'M', 1, 1, GETDATE(), GETDATE())

DECLARE @pathProfile varchar(max)
SET @pathProfile = FileTableRootPath() + '\Media\Profiles\'

--- 1 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\audio\music\' AND [FileType] = 'mp3'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\home-movies\' AND [FileType] = 'mp4'