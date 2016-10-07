USE [KeebeeAAT]
GO

CREATE USER [WIN10\webuser] FOR LOGIN [WIN10\webuser] WITH DEFAULT_SCHEMA=[dbo]

ALTER ROLE [db_datareader] ADD MEMBER [WIN10\webuser]

ALTER ROLE [db_datawriter] ADD MEMBER [WIN10\webuser]

