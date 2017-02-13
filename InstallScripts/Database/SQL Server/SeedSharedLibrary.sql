USE [KeebeeAAT]

/* 
TRUNCATE TABLE SharedMediaFiles

1, 'audio\music', 'Audio (Music)', 'Music', 0, 0, 1)
2, 'audio\radio-shows', 'Audio (Radio Shows)', 'Radio Shows', 0, 0, 1)
3, 'images\general', 'Images (General)', 'Images', 1, 0, 1)
5, 'videos\tv-shows', 'Videos (TV Shows)', 'TV Shows', 0, 0, 1)
7, 'activities\matching-game\shapes', 'Matching Game (Shapes)', 'Shapes', 1, 0, 1)
8, 'activities\matching-game\sounds', 'Matching Game (Sounds)', 'Sounds', 0, 0, 1)
9, 'videos\ambient', 'Videos (Ambient)', 'Videos', 0, 1, 1)
10, 'videos\cats', 'Videos (Cats)', 'Videos', 0, 1, 1)
*/

DECLARE @pathSharedLibrary varchar(max)
SET @pathSharedLibrary = FileTableRootPath() + '\Media\SharedLibrary\'

--- Media Path Type 1 - "Music" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 1, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\music\' AND [FileType] = 'mp3'

--- Media Path Type 2 - "Radio Shows" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 2, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'audio\radio-shows\' AND [FileType] = 'mp3'

--- Media Path Type 3 - "Images (General)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 3, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'images\general\' AND [FileType] IN ('jpg', 'jpeg', 'png', 'bmp', 'gif')

--- MediaPath Type 5 - "Videos (TV Shows)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 5, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\tv-shows\' AND [FileType] = 'mp4'

--- MediaPath Type 7 - "Matching Game (Shapes)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 7, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\shapes\' AND [FileType] = 'png'

--- MediaPath Type 8 - "Matching Game (Sounds)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 8, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'activities\matching-game\sounds\' AND [FileType] = 'mp3'

--- Media Path Type 9 - "Videos (Ambient)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 9, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\ambient\' AND [FileType] = 'mp4'

--- Media Path Type 10 - "Videos (Cats)" ---
INSERT INTO SharedMediaFiles (StreamId, IsSharable)
SELECT 10, StreamId, 1 FROM MediaFiles WHERE [Path] = @pathSharedLibrary + 'videos\cats\' AND [FileType] = 'mp4'