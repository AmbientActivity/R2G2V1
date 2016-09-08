USE [KeebeeAAT]

IF OBJECT_ID('__MigrationHistory', 'U') IS NOT NULL DROP TABLE __MigrationHistory
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users
IF OBJECT_ID('Caregivers', 'U') IS NOT NULL DROP TABLE Caregivers
IF OBJECT_ID('ActivityEventLogs', 'U') IS NOT NULL DROP TABLE ActivityEventLogs
IF OBJECT_ID('GameEventLogs', 'U') IS NOT NULL DROP TABLE GameEventLogs
IF OBJECT_ID('RfidEventLogs', 'U') IS NOT NULL DROP TABLE RfidEventLogs
IF OBJECT_ID('GameTypes', 'U') IS NOT NULL DROP TABLE GameTypes
IF OBJECT_ID('Responses', 'U') IS NOT NULL DROP TABLE Responses
IF OBJECT_ID('AmbientResponses', 'U') IS NOT NULL DROP TABLE AmbientResponses
IF OBJECT_ID('PersonalPictures', 'U') IS NOT NULL DROP TABLE PersonalPictures
IF OBJECT_ID('ConfigurationDetails', 'U') IS NOT NULL DROP TABLE ConfigurationDetails
IF OBJECT_ID('Configurations', 'U') IS NOT NULL DROP TABLE [Configurations]
IF OBJECT_ID('ResponseTypes', 'U') IS NOT NULL DROP TABLE ResponseTypes
IF OBJECT_ID('ResponseTypeCategories', 'U') IS NOT NULL DROP TABLE ResponseTypeCategories
IF OBJECT_ID('ActivityTypes', 'U') IS NOT NULL DROP TABLE ActivityTypes
IF OBJECT_ID('Residents', 'U') IS NOT NULL DROP TABLE Residents
IF OBJECT_ID('Profiles', 'U') IS NOT NULL DROP TABLE Profiles

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaFiles' AND TABLE_TYPE != 'VIEW'))
BEGIN
	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_dbo.Responses_dbo.MediaFiles_StreamId'))
		ALTER TABLE [dbo].[Responses] DROP CONSTRAINT [FK_dbo.Responses_dbo.MediaFiles_StreamId]

	IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_NAME = 'FK_dbo.AmbientResponses_dbo.MediaFiles_StreamId'))
		ALTER TABLE [dbo].[AmbientResponses] DROP CONSTRAINT [FK_dbo.AmbientResponses_dbo.MediaFiles_StreamId]

	DROP TABLE MediaFiles; 
END


IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'MediaFiles'))
  DROP VIEW MediaFiles; 
