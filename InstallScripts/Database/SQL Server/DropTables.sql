USE [KeebeeAAT]

IF OBJECT_ID('__MigrationHistory', 'U') IS NOT NULL DROP TABLE __MigrationHistory
IF OBJECT_ID('ActivityEventLogs', 'U') IS NOT NULL DROP TABLE ActivityEventLogs
IF OBJECT_ID('ActiveResidents', 'U') IS NOT NULL DROP TABLE ActiveResidents
IF OBJECT_ID('AmbientInvitations', 'U') IS NOT NULL DROP TABLE AmbientInvitations
IF OBJECT_ID('InteractiveActivityEventLogs', 'U') IS NOT NULL DROP TABLE InteractiveActivityEventLogs
IF OBJECT_ID('ActiveResidentEventLogs', 'U') IS NOT NULL DROP TABLE ActiveResidentEventLogs
IF OBJECT_ID('ConfigDetails', 'U') IS NOT NULL DROP TABLE ConfigDetails
IF OBJECT_ID('Configs', 'U') IS NOT NULL DROP TABLE [Configs]
IF OBJECT_ID('ResidentMediaFiles', 'U') IS NOT NULL DROP TABLE [ResidentMediaFiles]
IF OBJECT_ID('PublicMediaFiles', 'U') IS NOT NULL DROP TABLE [PublicMediaFiles]
IF OBJECT_ID('Thumbnails', 'U') IS NOT NULL DROP TABLE [Thumbnails]
IF OBJECT_ID('ResponseTypes', 'U') IS NOT NULL DROP TABLE ResponseTypes
IF OBJECT_ID('ResponseTypeCategories', 'U') IS NOT NULL DROP TABLE ResponseTypeCategories
IF OBJECT_ID('InteractiveActivityTypes', 'U') IS NOT NULL DROP TABLE InteractiveActivityTypes
IF OBJECT_ID('MediaPathTypes', 'U') IS NOT NULL DROP TABLE MediaPathTypes
IF OBJECT_ID('MediaPathTypeCategories', 'U') IS NOT NULL DROP TABLE MediaPathTypeCategories
IF OBJECT_ID('PhidgetTypes', 'U') IS NOT NULL DROP TABLE PhidgetTypes
IF OBJECT_ID('PhidgetStyleTypes', 'U') IS NOT NULL DROP TABLE PhidgetStyleTypes
IF OBJECT_ID('Residents', 'U') IS NOT NULL DROP TABLE Residents
IF OBJECT_ID('UserRoles', 'U') IS NOT NULL DROP TABLE UserRoles
IF OBJECT_ID('Roles', 'U') IS NOT NULL DROP TABLE Roles
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaFiles' AND TABLE_TYPE != 'VIEW'))
	DROP TABLE MediaFiles; 

-- MediaFileStreams
IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'MediaFiles'))
	DROP VIEW MediaFiles; 

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'MediaFileStreams' AND TABLE_TYPE != 'VIEW'))
	DROP TABLE MediaFileStreams; 

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_NAME = 'MediaFileStreams'))
	DROP VIEW MediaFileStreams; 