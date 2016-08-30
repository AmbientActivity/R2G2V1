---- Alter Database - Setup FileStream Share folder ----

ALTER DATABASE KeebeeAAT
SET FILESTREAM (NON_TRANSACTED_ACCESS = FULL, DIRECTORY_NAME = N'KeebeeAATFilestream')

---- Create the Media FileTable ----

CREATE TABLE Media AS FileTable