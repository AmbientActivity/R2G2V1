USE [KeebeeAAT]
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_dbo.Responses_dbo.MediaFiles_StreamId'))
	ALTER TABLE [dbo].[Responses] DROP CONSTRAINT [FK_dbo.Responses_dbo.MediaFiles_StreamId]
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_dbo.AmbientResponses_dbo.MediaFiles_StreamId'))
	ALTER TABLE [dbo].[AmbientResponses] DROP CONSTRAINT [FK_dbo.AmbientResponses_dbo.MediaFiles_StreamId]
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_dbo.PersonalPictures_dbo.MediaFiles_StreamId'))
	ALTER TABLE [dbo].[PersonalPictures] DROP CONSTRAINT [FK_dbo.PersonalPictures_dbo.MediaFiles_StreamId]
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaFiles'))
	DROP TABLE [dbo].[MediaFiles]
GO

CREATE VIEW [dbo].[MediaFiles]
AS
SELECT
	stream_id AS StreamId, 
	name as [Filename], 
	file_type as [FileType],
	cached_file_size AS [FileSize],
	is_directory as IsFolder,
	FileTableRootPath() + REPLACE(file_stream.GetFileNamespacePath(), [name], '') AS [Path]
FROM dbo.Media
GO

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaFileStreams'))
	DROP TABLE [dbo].[MediaFileStreams]
GO

CREATE VIEW [dbo].[MediaFileStreams]
AS
SELECT
	stream_id AS StreamId, 
	name as [Filename], 
	file_type as [FileType],
	cached_file_size AS [FileSize],
	is_directory as IsFolder,
	FileTableRootPath() + REPLACE(file_stream.GetFileNamespacePath(), [name], '') AS [Path],
	file_stream as Stream
FROM dbo.Media
GO