USE [master]
GO

IF db_id('KeebeeAAT') IS NOT NULL
BEGIN
	IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Keebee')
	BEGIN
		CREATE LOGIN [keebee] WITH PASSWORD=N'aat', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF

		USE [KeebeeAAT]

		CREATE USER [keebee] FOR LOGIN [keebee] WITH DEFAULT_SCHEMA=[dbo]

		ALTER ROLE [db_owner] ADD MEMBER [keebee]
	END
END