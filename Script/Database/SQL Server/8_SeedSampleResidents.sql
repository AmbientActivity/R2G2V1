USE [KeebeeAAT]
GO

-- residents
SET IDENTITY_INSERT [dbo].[Residents] ON 
INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (1, N'Alma', N'Robinson', N'F', 1, 0, GetDate(), GetDate())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (2, N'Earl', N'Smith', N'M', 2, 1, GetDate(), GetDate())

INSERT [dbo].[Residents] ([Id], [FirstName], [LastName], [Gender], [GameDifficultyLevel], [AllowVideoCapturing], [DateCreated], [DateUpdated]) 
VALUES (3, N'Ruth', N'Chambers', N'F', 3, 0, GetDate(), GetDate())
SET IDENTITY_INSERT [dbo].[Residents] OFF