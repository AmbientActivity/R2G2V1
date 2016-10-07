USE [KeebeeAAT]
GO

CREATE USER [R2G2\webuser] FOR LOGIN [R2G2\webuser] WITH DEFAULT_SCHEMA=[dbo]

ALTER ROLE [db_datareader] ADD MEMBER [R2G2\webuser]

ALTER ROLE [db_datawriter] ADD MEMBER [R2G2\webuser]

