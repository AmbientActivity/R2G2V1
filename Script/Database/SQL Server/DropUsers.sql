USE [KeebeeAAT]

---- Drop Users ----

DECLARE @databaseName AS VARCHAR(20) = N'KeebeeAAT'
DECLARE @sqlString AS VARCHAR(MAX) 

IF db_id(@databaseName) IS NOT NULL
BEGIN
	IF EXISTS (SELECT * FROM sys.database_principals WHERE name = 'Keebee')
	BEGIN
		SET @sqlString = 'DROP USER keebee'
		EXEC (@sqlString)

		SET @sqlString = 'DROP LOGIN keebee'
		EXEC (@sqlString)
	END

	DECLARE @serverName AS VARCHAR(20) = (SELECT HOST_NAME())

	IF EXISTS (SELECT * FROM sys.database_principals WHERE name = @serverName + '\webuser')
	BEGIN
		DECLARE @webuser AS VARCHAR(20) = (SELECT quotename(@serverName + '\webuser'))

		SET @sqlString = 'DROP USER ' + @webuser
		EXEC (@sqlString)
	END
END