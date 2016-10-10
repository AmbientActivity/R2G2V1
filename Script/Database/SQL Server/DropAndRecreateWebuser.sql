---- Create webuser ----
USE [KeebeeAAT]

IF db_id('KeebeeAAT') IS NOT NULL
BEGIN
	DECLARE @sqlString AS VARCHAR(MAX)
	DECLARE @serverName AS VARCHAR(20) = (SELECT HOST_NAME())
	DECLARE @webuser AS VARCHAR(20) = (SELECT quotename(@serverName + '\webuser'))

	SET @sqlString = 'DROP USER ' + @webuser
	EXEC (@sqlString)

	SET @sqlString = 'CREATE USER ' + @webuser + ' FOR LOGIN ' + @webuser 
	EXEC (@sqlString)

	SET @sqlString = 'ALTER ROLE [db_datareader] ADD MEMBER ' + @webuser
	EXEC (@sqlString)

	SET @sqlString = 'ALTER ROLE [db_datawriter] ADD MEMBER ' + @webuser
	EXEC (@sqlString)
END
