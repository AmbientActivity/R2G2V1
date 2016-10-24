USE [KeebeeAAT]

/* 
TRUNCATE TABLE ResidentMediaFiles
*/

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (1, 'Earl', 'McConachie', 'M', 1, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (2, 'Ruth', 'Abbey', 'F', 2, 1, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (3, 'Edwin', 'Ranta', 'M', 1, 0, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (4, 'John', 'Allen', 'M', 2, 0, GETDATE(), GETDATE())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (5, 'Jane', 'Doe', 'F', 1, 0, GETDATE(), GETDATE())
SET IDENTITY_INSERT [dbo].[Residents] OFF


DECLARE @pathProfile varchar(max)
SET @pathProfile = FileTableRootPath() + '\Media\Profiles\'

--- 1 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\music\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 1, 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\videos\'


--- 2 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\music\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 2, 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '2\videos\'


--- 3 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\music\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 3, 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '3\videos\'


--- 4 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '1\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\music\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 4, 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '4\videos\'


--- 5 ---
--- Activity 1 - ResponseType "SlideShow" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 1, 1, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\images\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- Activity 2 - ResponseType "MatchingGame" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, 5, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\shapes\' AND [FileType] = 'png'
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 2, 6, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\sounds\' AND [FileType] = 'mp3'

--- Activity 5 - ResponseType "Radio" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 5, 3, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\music\' AND [FileType] IN ('mp3')

--- Activity 6 - ResponseType "Television" ---
INSERT INTO ResidentMediaFiles (IsPublic, ResidentId, ResponseTypeId, MediaPathTypeId, StreamId)
SELECT 0, 5, 6, 2, StreamId FROM MediaFiles WHERE [Path] = @pathProfile + '5\videos\'