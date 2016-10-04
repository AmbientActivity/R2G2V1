USE [KeebeeAAT]
GO

CREATE USER [AAT\webus] FOR LOGIN [AAT\webus] WITH DEFAULT_SCHEMA=[dbo]

ALTER ROLE [db_datareader] ADD MEMBER [AAT\webus]

ALTER ROLE [db_datawriter] ADD MEMBER [AAT\webus]

