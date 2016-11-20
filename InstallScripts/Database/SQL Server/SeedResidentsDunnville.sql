USE [KeebeeAAT]

/* 
TRUNCATE TABLE ResidentMediaFiles
*/

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (1, 'Edwin', 'Ranta', 'M', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (2, 'Earl', 'McConachie', 'M', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (3, 'Ruth', 'Abbey', 'F', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (4, 'Judy', 'Swicks', 'F', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (5, 'Jack', 'Cranes', 'M', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (6, 'Wilbert', 'Ings', 'M', 1, 1, GETDATE(), GETDATE())
SET IDENTITY_INSERT [dbo].[Residents] OFF

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

--- 2 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\audio\music\' AND [FileType] = 'mp3'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\home-movies\' AND [FileType] = 'mp4'


--- 3 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\audio\music\' AND [FileType] = 'mp3'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\home-movies\' AND [FileType] = 'mp4'


--- 4 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\audio\music\' AND [FileType] IN ('mp3')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\audio\radio-shows\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\home-movies\' AND [FileType] = 'mp4'


--- 5 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\audio\music\' AND [FileType] IN ('mp3')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\audio\radio-shows\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\videos\home-movies\' AND [FileType] = 'mp4'


--- 6 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 1, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 1, 4, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\images\personal\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 2, 8, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\activities\matching-game\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 2, 9, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 5, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\audio\music\' AND [FileType] = 'mp3'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 5, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\audio\radio-shows\' AND [FileType] = 'mp3'

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 6, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\videos\tv-shows\' AND [FileType] = 'mp4'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 6, 6, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '6\videos\home-movies\' AND [FileType] = 'mp4'