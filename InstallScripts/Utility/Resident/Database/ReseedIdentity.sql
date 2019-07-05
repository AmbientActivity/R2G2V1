DECLARE @maxId INT
SELECT @maxId = MAX(Id) FROM Residents
DBCC CHECKIDENT ('Residents', RESEED, @maxId);