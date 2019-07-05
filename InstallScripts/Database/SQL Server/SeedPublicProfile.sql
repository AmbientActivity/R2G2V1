USE [KeebeeAAT]

/* 
TRUNCATE TABLE PublicMediaFiles
*/

DECLARE @profileId char(1) = '0'
DECLARE @pathProfiles varchar(100)
DECLARE @pathSharedLibrary varchar(100)
DECLARE @mediaPathType varchar(100)
DECLARE @allowedExts varchar(100)

SET @pathProfiles = FileTableRootPath() + '\Media\Profiles\'
SET @pathSharedLibrary = FileTableRootPath() + '\Media\SharedLibrary\'

--- ResponseType 1 - "SlideShow" ---
--- link from shared library
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + '''' FROM MediaPathTypes WHERE Id = 3 -- images/general
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 1, 3, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND (@allowedExts) LIKE '%' + [FileType] + '%'

--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 1, 3, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' +  @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 2 - "MatchingGame" ---
-- link from shared library
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 7 -- activities\matching-game\shapes\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 2, 7, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 2, 7, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

-- link from shared library
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 8 -- activities\matching-game\sounds\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 2, 8, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 2, 8, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 3 - ResponseType "Cats" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 10 -- videos\cats\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 3, 10, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 5 - ResponseType "Radio" ---
--- link from shared library
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 1 -- audio\music\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 5, 1, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 5, 1, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- link from shared library
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 2 -- audio\radio-shows\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 5, 2, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 5, 2, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 6 - ResponseType "Television" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 5 -- videos\tv-shows\
--- link from shared library
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 6, 5, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'
--- add from local profile
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 0, 6, 5, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathProfiles + @profileId + '\' + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 8 - ResponseType "Ambient" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 9 -- videos\ambient\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 8, 9, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 13 - ResponseType "Nature" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 11 -- videos\nature\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 13, 11, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 14 - ResponseType "Sports" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 12 -- videos\sports\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 14, 12, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 15 - ResponseType "Machinery" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 13 -- videos\machinery\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 15, 13, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 16 - ResponseType "Animals" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 14 -- videos\animals\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 16, 14, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

--- ResponseType 17 - ResponseType "Cute" ---
SELECT @mediaPathType = [Path], @allowedExts = '''' + REPLACE(AllowedExts, ', ', ''', ''') + ''''  FROM MediaPathTypes WHERE Id = 15 -- videos\cute\
INSERT INTO PublicMediaFiles (IsLinked, ResponseTypeId, MediaPathTypeId, StreamId, DateAdded)
SELECT 1, 17, 15, StreamId, GETDATE() FROM MediaFiles WHERE [Path] = @pathSharedLibrary + @mediaPathType + '\' AND  (@allowedExts) LIKE '%' + [FileType] + '%'

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